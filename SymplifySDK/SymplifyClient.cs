using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using SymplifySDK.Allocation.Config;
using SymplifySDK.Cookies;

namespace SymplifySDK
{
    /// <summary>
    /// Class <c>SymplifyClient</c> A client SDK for Symplify Server-Side Testing.
    /// The client maintains configuration for server-side tests for a website. It
    /// also provides functions for allocating variations and assigning visitor IDs.
    /// </summary>
    public class SymplifyClient
    {
        private readonly object syncLock = new object();
        private readonly int configUpdateIntervalMillis = 10000;
        private readonly HttpClient HttpClient;

        private Timer _timer;

        public SymplifyClient(ClientConfig clientConfig, HttpClient httpClient, ILogger logger, int configUpdateInterval = 10)
        {
            CdnBaseURL = clientConfig.CdnBaseURL;
            WebsiteID = clientConfig.WebsiteID;

            Uri cdnBaseUrlUrl = new Uri(CdnBaseURL);
            if (cdnBaseUrlUrl.Scheme == null || cdnBaseUrlUrl.Host == null)
            {
                throw new ArgumentException("malformed CDN base URL, cannot create SDK client");
            }

            HttpClient = httpClient;

            Logger = logger;

            Config = null;

            if (configUpdateInterval < 1)
            {
                throw new ArgumentException("configUpdateInterval < 1");
            }

            configUpdateIntervalMillis = configUpdateInterval * 1000;
        }

        public string WebsiteID { get; set; }

        public string CdnBaseURL { get; set; }

        // TODO We should be able to use Microsoft.Extensions.Logging, it works on .NET Core
        private ILogger Logger { get; set; }

        private SymplifyConfig Config { get; set; }

        public static async Task<SymplifyClient> WithDefaults(string websiteID, HttpClient httpClient, bool autoLoadConfig = true)
        {
            SymplifyClient client = new SymplifyClient(new ClientConfig(websiteID), httpClient, new DefaultLogger());

            if (autoLoadConfig)
            {
                await client.LoadConfig();
            }

            return client;
        }

        public async Task LoadConfig()
        {
            SymplifyConfig config = await FetchConfig();

            if (_timer == null)
            {
                lock (syncLock)
                {
                    if (_timer == null)
                    {
                        Logger.Log(LogLevel.INFO, "no config update timer present, creating");
                        _timer = CreateConfigTimer();
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
        /// Returns the name of the variation the visitor is part of in the project with the given name.
        /// </summary>
        /// <param name="projectName">The name of the project</param>
        /// <param name="cookieJar">A implementation of a CookieJar, having getCookie and setCookie</param>
        /// <returns>
        /// string|null the name of the current visitor's assigned variation, if one was allocated.
        /// null if the project or visitor could not be identified, or the config led to a null allocation.
        /// </returns>
        public string FindVariation(string projectName, ICookieJar cookieJar)
        {
            if (Config == null)
            {
                Logger.Log(LogLevel.ERROR, "findVariation called before config is available");
                return null;
            }

            var sympCookie = SymplifyCookie.FromCookies(cookieJar);
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

            switch (sympCookie.GetProjectAllocationStatus(WebsiteID, project.ID))
            {
                case ProjectAllocationStatus.Allocated:
                    var variationID = sympCookie.GetAllocatedVariationID(WebsiteID, project.ID);
                    return project.FindVariationWithId(variationID)?.Name;
                case ProjectAllocationStatus.NotAllocated:
                    return null;
                case ProjectAllocationStatus.Unknown:
                default:
                    // if we don't have any persisted allocation status, that
                    // means we continue below to get one!
                    break;
            }

            string visitorId = Visitor.EnsureVisitorID(sympCookie, WebsiteID);
            VariationConfig variation = Allocation.Allocation.FindVariationForVisitor(project, visitorId);

            if (variation == null)
            {
                sympCookie.SetAllocatedNullVariation(WebsiteID, project.ID);
            }
            else
            {
                sympCookie.SetAllocatedVariationID(WebsiteID, project.ID, variation.ID);
            }

            // TODO(Fabian) persist allocated variation and project info
            cookieJar.SetCookie(SymplifyCookie.CookieName, sympCookie.Encode());

            return variation?.Name;
        }

        public string GetConfigURL()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}/sstConfig.json", CdnBaseURL, WebsiteID);
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
                var response = HttpClient.GetStringAsync(url);
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
