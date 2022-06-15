namespace SymplifySDK
{
    /// <summary>
    /// Class <c>ClientConfig</c> stores the websiteID, CdnBaseUrl and logger information used by the <c>SymplifyClient</c>.
    /// </summary>
    public class ClientConfig
    {
        private const string DefaultCdnBaseUrl = "https://cdn-sitegainer.com";

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConfig"/> class.
        /// </summary>
        public ClientConfig(string websiteID)
        {
            WebsiteID = websiteID;
            CdnBaseURL = DefaultCdnBaseUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConfig"/> class.
        /// </summary>
        public ClientConfig(string websiteID, string cdnBaseURL)
        {
            WebsiteID = websiteID;
            CdnBaseURL = cdnBaseURL;
        }

        /// <summary>
        /// Gets the ID of the website using the SDK.
        /// </summary>
        public string WebsiteID { get; }

        /// <summary>
        /// Gets the base URL for test configuration files.
        /// </summary>
        public string CdnBaseURL { get; }
    }
}
