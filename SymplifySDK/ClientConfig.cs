using System.Text.Json;

namespace SymplifySDK
{
    /// <summary>
    /// Class <c>ClientConfig</c> stores the websiteID, CdnBaseUrl and logger information used by the <c>SymplifyClient</c> 
    /// </summary>
    public class ClientConfig
    {
        readonly string DEFAULT_CDN_BASEURL = "https://cdn-sitegainer.com";

        public string WebsiteID { get; }
        public string CdnBaseURL { get; set; }

        public ClientConfig(string websiteID)
        {
            WebsiteID = websiteID;
            CdnBaseURL = DEFAULT_CDN_BASEURL;
        }

        public ClientConfig(string websiteID, string cdnBaseURL)
        {
            WebsiteID = websiteID;
            CdnBaseURL = cdnBaseURL;
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
