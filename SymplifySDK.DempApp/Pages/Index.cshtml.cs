using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SymplifySDK.Allocation.Config;
using SymplifySDK.Cookies;

namespace SymplifySDK.DempApp.Pages
{
    public class IndexModel : PageModel, ICookieJar
    {
        private readonly ILogger<IndexModel> _logger;

        public string websiteId = "5620187";
        public SymplifyClient Client;
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
        public string GetCookie(string key)
        {
            return Request.Cookies[key];
        }

        public void SetCookie(string key, string value)
        {
            Response.Cookies.Append(key, value);
        }
    }
}

