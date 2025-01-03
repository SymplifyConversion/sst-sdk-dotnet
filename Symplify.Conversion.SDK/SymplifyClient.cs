﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Symplify.Conversion.SDK.Allocation.Config;
using Symplify.Conversion.SDK.Audience;
using Symplify.Conversion.SDK.Cookies;

namespace Symplify.Conversion.SDK
{
    /// <summary>
    /// Class <c>SymplifyClient</c> A client SDK for Symplify Server-Side Testing.
    /// The client maintains configuration for server-side tests for a website. It
    /// also provides functions for allocating variations and assigning visitor IDs.
    /// </summary>
    public class SymplifyClient
    {
        private readonly string cdnBaseURL;
        private readonly string currentWebsiteID;
        private readonly object syncLock = new object();
        private readonly int configUpdateIntervalMillis = 10000;
        private readonly HttpClient httpClient;
        private Timer timerHandle;

        private SymplifyConfig Config { get; set; }

        // TODO We should be able to use Microsoft.Extensions.Logging, it works on .NET Core
        private ILogger Logger { get; set; }

#pragma warning disable SA1201
        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyClient"/> class.
        /// </summary>
        public SymplifyClient(string websiteID, HttpClient httpClient)
        : this(new ClientConfig(websiteID), httpClient, new DefaultLogger())
        {
        }
#pragma warning disable SA1201

        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyClient"/> class.
        /// </summary>
        public SymplifyClient(ClientConfig clientConfig, HttpClient httpClient, ILogger logger, int configUpdateInterval = 60)
        {
            cdnBaseURL = clientConfig.CdnBaseURL;
            currentWebsiteID = clientConfig.WebsiteID;

            Uri checkURL = new Uri(cdnBaseURL);
            if (checkURL.Scheme == null || checkURL.Host == null)
            {
                throw new ArgumentException("malformed CDN base URL, cannot create SDK client");
            }

            this.httpClient = httpClient;

            Logger = logger;

            Config = null;

            if (configUpdateInterval < 1)
            {
                throw new ArgumentException("configUpdateInterval < 1");
            }

            configUpdateIntervalMillis = configUpdateInterval * 1000;
        }

        /// <summary>
        /// Download and use the latest version of the test configuration, await the update.
        /// </summary>
        public async Task LoadConfig()
        {
            SymplifyConfig config = await FetchConfig();

            if (timerHandle == null)
            {
                lock (syncLock)
                {
                    if (timerHandle == null)
                    {
                        Logger.Log(LogLevel.INFO, "no config update timer present, creating");
                        timerHandle = CreateConfigTimer();
                    }
                }
            }

            if (config == null)
            {
                Logger.Log(LogLevel.ERROR, "config fetch failed");
                return;
            }

            Config = config;
        }

        /// <summary>
        /// List all projects in the current test configuration.
        /// </summary>
        public List<string> ListProjects()
        {
            if (Config == null)
            {
                Logger.Log(LogLevel.ERROR, "listProjects called before config is available");
                return new List<string>();
            }

            List<string> projectList = new List<string>();

            foreach (ProjectConfig project in Config.Projects)
            {
                projectList.Add(project.Name);
            }

            return projectList;
        }

        /// <summary>
        /// Returns the name of the variation the visitor is part of in the project with the given name, or null if the visitor was not allocated.
        /// </summary>
        public string FindVariation(string projectName, ICookieJar cookieJar, dynamic customAttributes = null)
        {
            // TODO preview??
            if (Config == null)
            {
                Logger.Log(LogLevel.ERROR, "findVariation called before config is available");
                return null;
            }

            if (Config.Privacy_mode == 2 && cookieJar.GetCookie("sg_optin") != "1")
            {
                return null;
            }

            var sympCookie = SymplifyCookie.FromCookies(currentWebsiteID, cookieJar);
            if (!sympCookie.IsSupported())
            {
                // we don't know what this cookie is, so let's not touch anything
                return null;
            }

            ProjectConfig project = Config.FindProjectWithName(projectName);

            if (project == null)
            {
                Logger.Log(LogLevel.WARN, string.Format(CultureInfo.InvariantCulture, "project does not exist: {0}", projectName));
                return null;
            }

            if (project.State != ProjectState.Active)
            {
                return null;
            }

            if (sympCookie.GetPreviewData().Count > 0)
            {
                return HandlePreview(sympCookie, project, cookieJar, customAttributes);
            }

            switch (sympCookie.GetProjectAllocationStatus(project.ID))
            {
                case ProjectAllocationStatus.Allocated:
                    var variationID = sympCookie.GetAllocatedVariationID(project.ID);
                    return project.FindVariationWithId(variationID)?.Name;
                case ProjectAllocationStatus.NotAllocated:
                    return null;
                default:
                    // if we don't have any persisted allocation status, that
                    // means we continue below to get one!
                    break;
            }

            if (project.Audience_rules.Count != 0)
            {
                var audience = new SymplifyAudience(project.Audience_rules);

                if (!DoesAudienceApply(audience, customAttributes))
                {
                    return null;
                }
            }

            string visitorId = Visitor.EnsureVisitorID(sympCookie);
            VariationConfig variation = Allocation.Allocation.FindVariationForVisitor(project, visitorId);

            if (variation == null)
            {
                sympCookie.SetAllocatedNullVariation(project.ID);
            }
            else
            {
                sympCookie.SetAllocatedVariationID(project.ID, variation.ID);
            }

            cookieJar.SetCookie(SymplifyCookie.CookieName, sympCookie.ToJSON(), 90);

            return variation?.Name;
        }

        /// <summary>
        /// Get the URL to the SST config file.
        /// </summary>
        public string GetConfigURL()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}/sstConfig.json", cdnBaseURL, currentWebsiteID);
        }

        private static bool? DoesAudienceApply(SymplifyAudience audience, dynamic audienceAttributes)
        {
            var audienceEval = audience.Eval(audienceAttributes);

            if (audienceEval is string)
            {
                return null;
            }

            return audienceEval;
        }

        private string HandlePreview(SymplifyCookie sympCookie, ProjectConfig project, ICookieJar cookieJar, dynamic customAttributes)
        {
            if (project.Audience_rules.Count > 0)
            {
                SymplifyAudience audience = new SymplifyAudience(project.Audience_rules);

                dynamic audienceTrace = audience.Trace(customAttributes);

                if (audienceTrace is string)
                {
                    return null;
                }

                cookieJar.SetCookie("sg_audience_trace", JsonConvert.SerializeObject(audienceTrace), 1);

                if (!DoesAudienceApply(audience, customAttributes))
                {
                    return null;
                }
            }

            VariationConfig variation = null;
            if (sympCookie.GetPreviewData().ContainsKey("variationId"))
            {
                int variationId = sympCookie.GetPreviewData()["variationId"];
                variation = project.FindVariationWithId(variationId);
            }

            if (variation != null)
            {
                sympCookie.SetAllocatedVariationID(project.ID, variation.ID);
                cookieJar.SetCookie(SymplifyCookie.CookieName, sympCookie.ToJSON(), 90);
            }

            return variation.Name ?? null;
        }

        private async Task<SymplifyConfig> FetchConfig()
        {
            string url = GetConfigURL();
            string jsonResponse = await DownloadWithHttpClient(url);

            if (jsonResponse == null)
            {
                Logger.Log(LogLevel.ERROR, "no config JSON to parse");
                return null;
            }

            return new SymplifyConfig(jsonResponse);
        }

        private Timer CreateConfigTimer()
        {
            return new Timer(TimerCallback, null, 0, configUpdateIntervalMillis);
        }

        private void TimerCallback(object o)
        {
            _ = LoadConfig();
        }

        private async Task<string> DownloadWithHttpClient(string url)
        {
            try
            {
                var response = httpClient.GetStringAsync(url);
                return await response;
            }
            catch (Exception)
            {
                Logger.Log(LogLevel.ERROR, "Could not download " + url);
                return null;
            }
        }
    }
}
