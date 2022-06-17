using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Symplify.Conversion.SDK.Cookies;
using Symplify.Conversion.SDK.DemoApp.Services;

namespace Symplify.Conversion.SDK.DemoApp.Pages
{
    public class IndexModel : PageModel, ICookieJar
    {
        public readonly ISymplifyService _service;
        public SymplifyClient client;

        public IndexModel(ISymplifyService service)
        {
            _service = service;
        }

        public string GetWebsiteID()
        {
            return _service.GetWebsiteID();
        }

        public void OnGet()
        {
            client = _service.GetClient();
        }

        // Needed because the index model is the cookieJar and have to  implement the GetCookie method 
        public string GetCookie(string name)
        {
            return Request.Cookies[name];
        }

        // Needed because the index model is the cookieJar and have to  implement the SetCookie method 
        public void SetCookie(string name, string value, uint expireInDays)
        {
            var opts = new CookieOptions();
            opts.Domain = ".localhost.test"; // assumes served as explaind in README
            opts.Expires = System.DateTimeOffset.Now.AddDays(expireInDays);
            Response.Cookies.Append(name, value, opts);
        }
    }
}
