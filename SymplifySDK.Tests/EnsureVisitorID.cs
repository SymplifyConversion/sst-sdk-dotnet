using System.Collections.Generic;

using SymplifySDK.Cookies;
using Xunit;

namespace SymplifySDK.Tests
{
    public class EnsureVisitorID
    {

        const string COOKIE_NAME = SymplifyCookie.CookieName;

        [Fact]
        public void TestSetCookie()
        {
            SymplifyCookie sympCookie = new("TestSite");
            string returnedID = Visitor.EnsureVisitorID(sympCookie, () => "goober");

            Assert.Equal("goober", returnedID);
            Assert.Equal("goober", sympCookie.GetVisitorID());
        }

        [Fact]
        public void TestReuseCookie()
        {
            SymplifyCookie sympCookie = new("TestSite");
            sympCookie.SetVisitorID("goober");

            string returnedID = Visitor.EnsureVisitorID(sympCookie, () => "foobar");

            Assert.Equal("goober", returnedID);
        }

        [Fact]
        public void TestGenerateUUID()
        {
            string returnedIDA = Visitor.EnsureVisitorID(new("TestSite"));
            string returnedIDB = Visitor.EnsureVisitorID(new("TestSite"));

            string uuidPattern = "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";
            Assert.Matches(uuidPattern, returnedIDA);
            Assert.Matches(uuidPattern, returnedIDB);
            Assert.NotEqual(returnedIDB, returnedIDA);
        }
    }

    /// <summary>
    /// RawCookieJar is an in memory cookie jar not tied to any request. It just uses a dictionary for storage, and does no encoding or decoding (hence "raw").
    /// </summary>
    public class RawCookieJar : ICookieJar
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

        public void SetCookie(string name, string value, uint expireInDays)
        {
            Cookies[name] = value;
        }
    }

}
