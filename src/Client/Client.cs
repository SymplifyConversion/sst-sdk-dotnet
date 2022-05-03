using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Allocation.Config;
using Logger;

namespace Client
{
    public class Client
    {
        private string WebsiteID { get; set; }
        private string CdnBaseURL { get; set; }

        private HttpClient HttpClient = new();

        private SymplifyConfig Config { get; set; }

        private ILogger Logger { get; set; }

        public Client(ClientConfig clientConfig)
        {
            CdnBaseURL = clientConfig.CdnBaseURL;
            WebsiteID = clientConfig.WebsiteID;

            Uri cdnBaseUrlUrl = new Uri(CdnBaseURL);
            if (cdnBaseUrlUrl.Scheme == null || cdnBaseUrlUrl.Host == null)
            {
                throw new ArgumentException("malformed cdnBaseUrl, cannor create SDK client");
            }

            Config = null;
        }

        public static async Task<Client> WithDefault(string websiteID, bool autoLoadConfig = true)
        {
            Client client = new Client(new ClientConfig(websiteID));

            if (autoLoadConfig)
            {
                await client.LoadConfig();
            }

            return client;
        }

        public async Task LoadConfig()
        {
            SymplifyConfig config = await this.FetchConfig();

            if (config == null)
            {
                return;
            }

            this.Config = config;
        }

        public async Task<SymplifyConfig> FetchConfig()
        {
            string url = this.GetConfigURL();
            string WebResponse = await DownloadWithHttpClient(url);

            if (WebResponse == null)
            {
                return null;
            }

            return new SymplifyConfig(WebResponse);
        }

        private async Task<string> DownloadWithHttpClient(string url)
        {
            try
            {
                var response = this.HttpClient.GetStringAsync(url);
                return await response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetConfigURL()
        {
            return string.Format("{0}/{1}/sstConfig.json", this.CdnBaseURL, this.WebsiteID);
        }

        public List<string> listProjects()
        {
            if (this.Config != null)
            {
                return new List<string>();
            }

            List<string> projectList = new List<string>();

            foreach (ProjectConfig project in this.Config.Projects)
            {
                projectList.Add(project.Name);
            }

            return projectList;
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

}
