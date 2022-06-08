using System.Collections.Generic;
using System.Text.Json;
using System.Web;
using SymplifySDK.Cookies;
using Xunit;

namespace SymplifySDK.Tests
{
    public class EnsureVisitorID
    {
        // TODO: Change to correct cookie name
        const string COOKIE_NAME = "sg_sst_vid";

        [Fact]
        public void TestSetCookie()
        {
            CookieJar cookieJar = new();
            string returnedID = Visitor.EnsureVisitorID(cookieJar, "TestSite", () => "goober");

            SymplifyCookie simpCookie = JsonSerializer.Deserialize<SymplifyCookie>(HttpUtility.UrlDecode(cookieJar.GetCookie(COOKIE_NAME)));
            Assert.Equal("goober", returnedID);
            Assert.Equal("goober", simpCookie.WebsiteId);
        }

        [Fact]
        public void TestSetCookieWithOtherCookiesInside()
        {
            CookieJar cookieJar = new();
            cookieJar.SetCookie("testName", "testValue");
            string returnedID = Visitor.EnsureVisitorID(cookieJar, "TestSite", () => "goober");
            SymplifyCookie simpCookie = JsonSerializer.Deserialize<SymplifyCookie>(HttpUtility.UrlDecode(cookieJar.GetCookie(COOKIE_NAME)));

            Assert.Equal("goober", returnedID);
            Assert.Equal("goober", simpCookie.WebsiteId);
        }

        [Fact]
        public void TestReuseCookie()
        {
            CookieJar cookieJar = new();
            string returnedIDA = Visitor.EnsureVisitorID(cookieJar, "TestSite", () => "goober");
            string returnedIDB = Visitor.EnsureVisitorID(cookieJar, "TestSite", () => "foober");
            SymplifyCookie simpCookie = JsonSerializer.Deserialize<SymplifyCookie>(HttpUtility.UrlDecode(cookieJar.GetCookie(COOKIE_NAME)));

            Assert.Equal("goober", returnedIDA);
            Assert.Equal("goober", returnedIDB);
            Assert.Equal("goober", simpCookie.WebsiteId);
        }

        [Fact]
        public void TestGenerateUUID()
        {
            string returnedIDA = Visitor.EnsureVisitorID(new CookieJar(), "TestSite");
            string returnedIDB = Visitor.EnsureVisitorID(new CookieJar(), "TestSite");

            string uuidPattern = "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";
            Assert.Matches(uuidPattern, returnedIDA);
            Assert.Matches(uuidPattern, returnedIDB);
            Assert.NotEqual(returnedIDB, returnedIDA);
        }

        [Fact]
        public void TestUseOurJsonCookie()
        {
            CookieJar cookieJar = new();
            string rawCookie = "{%2210001%22:{%22100000002%22:[300001]%2C%22100000001%22:[300002]%2C%22100000002_ch%22:1%2C%22100000001_ch%22:1%2C%22lv%22:1650967549303%2C%22rf%22:%22%22%2C%22pv%22:2%2C%22pv_p%22:{%22100000002%22:2%2C%22100000001%22:2}%2C%22tv%22:2%2C%22tv_p%22:{%22100000002%22:2%2C%22100000001%22:2}%2C%22aud_p%22:[100000002%2C100000001]%2C%22visid%22:%2278ac2972-de5f-4262-bfdb-7296eb132a94%22%2C%22commid%22:%221be9f08d-c36c-4bce-b157-e057e050027c%22}%2C%22_g%22:1}";

            cookieJar.SetCookie(COOKIE_NAME, rawCookie);
            string returnedIDA = Visitor.EnsureVisitorID(cookieJar, "TestSite", () => "goober");
            SymplifyCookie simpCookie = JsonSerializer.Deserialize<SymplifyCookie>(HttpUtility.UrlDecode(cookieJar.GetCookie(COOKIE_NAME)));

            Assert.Equal("goober", returnedIDA);
            Assert.Equal("goober", simpCookie.WebsiteId);
        }

        [Fact]
        public void TestAbortOnUnkonwCookieVersion()
        {
            CookieJar cookieJar = new();
            string rawCookie = "{%2210001%22:{%22100000002%22:[300001]%2C%22100000001%22:[300002]%2C%22100000002_ch%22:1%2C%22100000001_ch%22:1%2C%22lv%22:1650967549303%2C%22rf%22:%22%22%2C%22pv%22:2%2C%22pv_p%22:{%22100000002%22:2%2C%22100000001%22:2}%2C%22tv%22:2%2C%22tv_p%22:{%22100000002%22:2%2C%22100000001%22:2}%2C%22aud_p%22:[100000002%2C100000001]%2C%22visid%22:%2278ac2972-de5f-4262-bfdb-7296eb132a94%22%2C%22commid%22:%221be9f08d-c36c-4bce-b157-e057e050027c%22}%2C%22_g%22:1000000}";

            cookieJar.SetCookie(COOKIE_NAME, rawCookie);
            string returnedIDA = Visitor.EnsureVisitorID(cookieJar, "TestSite", () => "goober");
            SymplifyCookie simpCookie = JsonSerializer.Deserialize<SymplifyCookie>(HttpUtility.UrlDecode(cookieJar.GetCookie(COOKIE_NAME)));

            Assert.Null(returnedIDA);
            Assert.Null(simpCookie.WebsiteId);
        }
    }

    public class CookieJar : ICookieJar
    {
        public Dictionary<string, string> Cookies = new Dictionary<string, string>();

        public string GetCookie(string name)
        {
            if (Cookies.TryGetValue(name, out string value))
            {
                return value;
            }

            return null;
        }

        public void SetCookie(string name, string value)
        {
            Cookies[name] = value;
        }
    }

}
