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
    public class SymplifyClient
    {
        public string WebsiteID { get; set; }
        public string CdnBaseURL { get; set; }

        private HttpClient HttpClient = new();

        public SymplifyConfig Config { get; set; }

        private ILogger Logger { get; set; }

        public SymplifyClient(ClientConfig clientConfig, int configUpdateInterval = 10)
        {
            CdnBaseURL = clientConfig.CdnBaseURL;
            WebsiteID = clientConfig.WebsiteID;

            Uri cdnBaseUrlUrl = new Uri(CdnBaseURL);
            if (cdnBaseUrlUrl.Scheme == null || cdnBaseUrlUrl.Host == null)
            {
                throw new ArgumentException("malformed cdnBaseUrl, cannor create SDK client");
            }

            Logger = clientConfig.Logger;

            Config = null;


            if (configUpdateInterval < 1)
            {
                throw new Exception("configUpdateInterval < 1");
            }

            // Create a Timer object that knows to call our TimerCallback
            // method once every configUpdateInterval * 1000 milliseconds.
            _ = new Timer(TimerCallback, null, 0, configUpdateInterval * 1000);
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
                Logger.Log(LogLevel.ERROR, "Could not downlaod the client");
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

        public string FindVariation(string projectName, string websiteID, ICookieJar cookieJar)
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

            string visitorId = Visitor.EnsureVisitorID(cookieJar, websiteID);
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
