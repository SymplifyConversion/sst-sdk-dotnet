using System;
using System.Text.Json;
using System.Web;
using SymplifySDK.Cookies;

namespace SymplifySDK
{
    public class Visitor
    {
        const string COOKIE_NAME = "sg_sst_vid";
        const int SUPPORTED_COOKIE_VERSION = 1;


        public static string EnsureVisitorID(ICookieJar cookieJar, string websiteId, Func<string> makeID = null)
        {
            string visitorCookie = cookieJar.GetCookie(COOKIE_NAME);

            SymplifyCookie simpCookie;
            if (visitorCookie != null)
            {
                simpCookie = JsonSerializer.Deserialize<SymplifyCookie>(HttpUtility.UrlDecode(visitorCookie));
            }
            else
            {
                simpCookie = new();
                simpCookie.CookieVersionKey = SUPPORTED_COOKIE_VERSION;
            }

            if (simpCookie.CookieVersionKey != SUPPORTED_COOKIE_VERSION)
            {
                return null;
            }

            if (simpCookie.WebsiteId != null)
            {
                return simpCookie.WebsiteId;
            }

            string visitorID = makeID != null ? makeID() : uuidGenerator();
            simpCookie.WebsiteId = visitorID;

            cookieJar.SetCookie(COOKIE_NAME, HttpUtility.UrlEncode(JsonSerializer.Serialize(simpCookie, new JsonSerializerOptions { WriteIndented = true })));

            return visitorID;
        }

        public static string EnsureVisitorIDWithGetAndSetCookies(Func<string, string> getCookie, Func<string, string, string> setCookie, string websiteId, Func<string> makeID = null)
        {
            string visitorCookie = getCookie(COOKIE_NAME);
            SymplifyCookie simpCookie;

            if (visitorCookie != null)
            {
                simpCookie = JsonSerializer.Deserialize<SymplifyCookie>(HttpUtility.UrlDecode(visitorCookie));
            }
            else
            {
                simpCookie = new();
                simpCookie.CookieVersionKey = SUPPORTED_COOKIE_VERSION;
            }

            if (simpCookie.CookieVersionKey != SUPPORTED_COOKIE_VERSION)
            {
                return null;
            }

            if (simpCookie.WebsiteId != null)
            {
                return simpCookie.WebsiteId;
            }

            string visitorID = makeID != null ? makeID() : uuidGenerator();
            simpCookie.WebsiteId = visitorID;

            setCookie(COOKIE_NAME, HttpUtility.UrlEncode(JsonSerializer.Serialize(simpCookie, new JsonSerializerOptions { WriteIndented = true })));

            return visitorID;
        }

        private static string uuidGenerator()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
