using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SymplifySDK.Allocation.Config;

namespace SymplifySDK.DempApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string websiteId = "5620148";
        public SymplifyClient Client;

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
    }
}
