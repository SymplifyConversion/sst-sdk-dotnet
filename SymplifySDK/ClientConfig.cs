using System.Text.Json;

namespace SymplifySDK
{
    /// <summary>
    /// Class <c>ClientConfig</c> stores the websiteID, CdnBaseUrl and logger information used by the <c>SymplifyClient</c>
    /// </summary>
    public class ClientConfig
    {
        private const string DefaultCdnBaseUrl = "https://cdn-sitegainer.com";

        public ClientConfig(string websiteID)
        {
            WebsiteID = websiteID;
            CdnBaseURL = DefaultCdnBaseUrl;
        }

        public ClientConfig(string websiteID, string cdnBaseURL)
        {
            WebsiteID = websiteID;
            CdnBaseURL = cdnBaseURL;
        }

        public string WebsiteID { get; }

        public string CdnBaseURL { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
