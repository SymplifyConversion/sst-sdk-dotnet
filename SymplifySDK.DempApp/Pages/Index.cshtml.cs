using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SymplifySDK.Allocation.Config;
using SymplifySDK.Cookies;

namespace SymplifySDK.DempApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string websiteId = "5620187";
        public SymplifyClient Client;
        public ICookieJar CookieJar;
        public string Variation;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            string cdnBaseURL = "https://cdn-sitegainer.com";
            ClientConfig config = new(websiteId, cdnBaseURL);
            Client = new(config);
            await Client.LoadConfig();
        }

        public string Get(string key)
        {
            return Request.Cookies[key];
        }

        public string Set(string key, string value)
        {
            Response.Cookies.Append(key, value);
            return "";
        }
    }
}
