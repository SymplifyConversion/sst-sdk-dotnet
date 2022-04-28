using System.Text.Json;

namespace Allocation.Config
{
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

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
