using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
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
        public string WebsiteID { get; set; }
        public string CdnBaseURL { get; set; }

        private readonly object syncLock = new object();
        private int configUpdateIntervalMillis = 10000;
        private Timer _timer;

        private HttpClient HttpClient = new();

        public SymplifyConfig Config { get; set; }

        // TODO We should be able to use Microsoft.Extensions.Logging, it works on .NET Core
        private ILogger Logger { get; set; }

        public SymplifyClient(ClientConfig clientConfig, int configUpdateInterval = 10)
        {
            CdnBaseURL = clientConfig.CdnBaseURL;
            WebsiteID = clientConfig.WebsiteID;

            Uri cdnBaseUrlUrl = new Uri(CdnBaseURL);
            if (cdnBaseUrlUrl.Scheme == null || cdnBaseUrlUrl.Host == null)
            {
                throw new ArgumentException("malformed CDN base URL, cannot create SDK client");
            }

            Logger = clientConfig.Logger;

            Config = null;

            if (configUpdateInterval < 1)
            {
                throw new Exception("configUpdateInterval < 1");
            }
            configUpdateIntervalMillis = configUpdateInterval * 1000;
        }

        private Timer CreateConfigTimer() {
            return new Timer(TimerCallback, null, 0, configUpdateIntervalMillis);
        }

        private void TimerCallback(Object o)
        {
            _ = LoadConfig();
        }

        public static async Task<SymplifyClient> WithDefault(string websiteID, bool autoLoadConfig = true)
        {
            SymplifyClient client = new SymplifyClient(new ClientConfig(websiteID));

            if (autoLoadConfig)
            {
                await client.LoadConfig();
            }

            return client;
        }

        public async Task LoadConfig()
        {
            SymplifyConfig config = await FetchConfig();

            if (null == _timer) {
                lock(syncLock) {
                    if (null == _timer) {
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

        public async Task<SymplifyConfig> FetchConfig()
        {
            string url = GetConfigURL();
            string WebResponse = await DownloadWithHttpClient(url);

            if (WebResponse == null)
            {
                Logger.Log(LogLevel.ERROR, "no config JSON to parse");
                return null;
            }

            return new SymplifyConfig(WebResponse);
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

        public string GetConfigURL()
        {
            return string.Format("{0}/{1}/sstConfig.json", CdnBaseURL, WebsiteID);
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
        /// string|null the name of the current visitor's assigned variation, 
        /// null if there is no matching project or no visitor ID was found.
        /// </returns>
        public string FindVariation(string projectName, ICookieJar cookieJar)
        {
            if (Config == null)
            {
                Logger.Log(LogLevel.ERROR, "findVariation called before config is available");
                return null;
            }

            ProjectConfig project = Config.FindProjectWithName(projectName);

            if (project == null)
            {
                Logger.Log(LogLevel.WARN, string.Format("project does not exist: {0}", projectName));
                return null;
            }

            string visitorId = Visitor.EnsureVisitorID(cookieJar, WebsiteID);
            VariationConfig variation = Allocation.Allocation.FindVariationForVisitor(project, visitorId);

            if (variation.State != VariationState.Active)
            {
                return null;
            }

            return variation.Name;
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

}
