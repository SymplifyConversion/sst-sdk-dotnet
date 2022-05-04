using System.Text.Json;

namespace SymplifySDK.Allocation.Config
{
    public class ClientConfig
    {
        readonly string DEFAULT_CDN_BASEURL = "https://cdn-sitegainer.com";

        public string WebsiteID { get; }
        public string CdnBaseURL { get; set; }
        public ILogger Logger { get; set; }

        public ClientConfig(string websiteID)
        {
            WebsiteID = websiteID;
            CdnBaseURL = DEFAULT_CDN_BASEURL;
            Logger = new DefaultLogger();
        }

        public ClientConfig(string websiteID, string cdnBaseURL)
        {
            WebsiteID = websiteID;
            CdnBaseURL = cdnBaseURL;
            Logger = new DefaultLogger();
        }

        public ClientConfig(string websiteID, string cdnBaseURL, ILogger logger)
        {
            WebsiteID = websiteID;
            CdnBaseURL = cdnBaseURL;
            Logger = logger;
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
