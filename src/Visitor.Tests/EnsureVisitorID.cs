using System.Net;
using Xunit;

namespace Visitor.Tests
{
    public class EnsureVisitorID
    {
        [Fact]
        public void testSetCookie()
        {
            CookieCollection cookieCollection = new CookieCollection();
            string returnedID = Visitor.EnsureVisitorID(cookieCollection, "", () => "goober");
            Assert.Equal("goober", returnedID);
            Assert.Equal("goober", cookieCollection["sg_sst_vid"].Value);
        }

        [Fact]
        public void testSetCookieWithOtherCookiesInside()
        {
            CookieCollection cookieCollection = new CookieCollection
            {
                new Cookie("testName", "testValue")
            };
            string returnedID = Visitor.EnsureVisitorID(cookieCollection, "", () => "goober");
            Assert.Equal("goober", returnedID);
            Assert.Equal("goober", cookieCollection["sg_sst_vid"].Value);
        }

        [Fact]
        public void testReuseCookie()
        {
            CookieCollection cookieCollection = new CookieCollection();
            string returnedIDA = Visitor.EnsureVisitorID(cookieCollection, "", () => "goober");
            string returnedIDB = Visitor.EnsureVisitorID(cookieCollection, "", () => "foober");
            Assert.Equal("goober", returnedIDA);
            Assert.Equal("goober", returnedIDB);
            Assert.Equal("goober", cookieCollection["sg_sst_vid"].Value);
        }

        [Fact]
        public void testGenerateUUID()
        {
            string returnedIDA = Visitor.EnsureVisitorID(new CookieCollection(), "");
            string returnedIDB = Visitor.EnsureVisitorID(new CookieCollection(), "");

            string uuidPattern = "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";
            Assert.Matches(uuidPattern, returnedIDA);
            Assert.Matches(uuidPattern, returnedIDB);
            Assert.NotEqual(returnedIDB, returnedIDA);
        }
    }
}
