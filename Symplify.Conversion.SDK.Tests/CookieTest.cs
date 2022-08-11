using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Xunit;

using Symplify.Conversion.SDK.Cookies;

namespace Symplify.Conversion.SDK.Tests
{
    public class CookieTest
    {
        const string COOKIE_JSON = @"{
            ""4711"":{
                ""1122"":[9114],
                ""1123"":[9115],
                ""1122_ch"":1,
                ""1123_ch"":1,
                ""lv"":1654780473129,
                ""rf"":"""",
                ""pv"":1,
                ""pv_p"":{
                    ""1122"":1,
                    ""1123"":1
                },
                ""tv"":1,
                ""tv_p"":{
                    ""1122"":1,
                    ""1123"":1
                },
                ""aud_p"":[1122,1123],
                ""visid"":""d5f81f13-0e15-453e-978f-000000000000"",
                ""commid"":""7475ff19-9088-4e08-923f-000000000000""
            },
            ""_g"":1
        }";

        const string RAW_COOKIE = "{%224711%22:{%221122%22:[9114]%2C%221123%22:[9115]%2C%221122_ch%22:1%2C%221123_ch%22:1%2C%22lv%22:1654780473129%2C%22rf%22:%22%22%2C%22pv%22:1%2C%22pv_p%22:{%221122%22:1%2C%221123%22:1}%2C%22tv%22:1%2C%22tv_p%22:{%221122%22:1%2C%221123%22:1}%2C%22aud_p%22:[1122%2C1123]%2C%22visid%22:%22d5f81f13-0e15-453e-978f-000000000000%22%2C%22commid%22:%227475ff19-9088-4e08-923f-000000000000%22}%2C%22_g%22:1}";

        [Theory]
        [InlineData(COOKIE_JSON, "d5f81f13-0e15-453e-978f-000000000000")]
        [InlineData(RAW_COOKIE, "d5f81f13-0e15-453e-978f-000000000000")]
        public void TestCreateFromJSON(string json, string visid)
        {
            SymplifyCookie cookie = SymplifyCookie.FromJSON("4711", WebUtility.UrlDecode(json));
            Assert.Equal(1, cookie.GetVersion());
            Assert.Equal(visid, cookie.GetVisitorID());
        }

        [Theory]
        [InlineData(COOKIE_JSON)]
        [InlineData(RAW_COOKIE)]
        public void TestDontDestroyPrevData(string json)
        {
            SymplifyCookie cookie = SymplifyCookie.FromJSON("4711", WebUtility.UrlDecode(json));
            var newJSON = WebUtility.UrlDecode(cookie.ToJSON());
            Assert.Equal(1654780473129, (long)JObject.Parse(newJSON)["4711"]["lv"]);
        }

        [Fact]
        public void TestCreateFromScratch()
        {
            SymplifyCookie cookie = new SymplifyCookie("anything");
            Assert.Equal(1, cookie.GetVersion());
        }

        [Theory]
        [InlineData(COOKIE_JSON)]
        [InlineData(RAW_COOKIE)]
        public void TestJSONCodec(string cookieVal)
        {
            var originalJSON = WebUtility.UrlDecode(cookieVal);
            var roundtripJSON = WebUtility.UrlDecode(SymplifyCookie.FromJSON("4711", originalJSON).ToJSON());
            Assert.True(
                JToken.DeepEquals(JObject.Parse(originalJSON), JObject.Parse(roundtripJSON)),
                $"{originalJSON} != {roundtripJSON}"
            );
        }

        [Theory]
        [InlineData(COOKIE_JSON, 1122, 9114)]
        [InlineData(COOKIE_JSON, 1123, 9115)]
        [InlineData(RAW_COOKIE, 1122, 9114)]
        [InlineData(RAW_COOKIE, 1123, 9115)]
        public void TestGetAllocatedVariation(string json, int projectID, int variationID)
        {
            SymplifyCookie cookie = SymplifyCookie.FromJSON("4711", WebUtility.UrlDecode(json));
            Assert.Equal(variationID, cookie.GetAllocatedVariationID(projectID));
        }

        [Fact]
        public void TestSetAllocatedVariation()
        {
            int projectID = 1337;
            int variationID = 42;
            SymplifyCookie cookie = new SymplifyCookie("4711");
            cookie.SetAllocatedVariationID(projectID, variationID);
            Assert.Equal(variationID, cookie.GetAllocatedVariationID(projectID));
        }

        [Fact]
        public void TestAllocatedProjectSet()
        {
            int projectID = 1337;
            SymplifyCookie cookie = new SymplifyCookie("some site id");

            cookie.SetAllocatedVariationID(projectID, 42);
            Assert.Equal(42, cookie.GetAllocatedVariationID(projectID));
            cookie.SetAllocatedVariationID(projectID, 9999);
            Assert.Equal(9999, cookie.GetAllocatedVariationID(projectID));

            Assert.Equal(new List<long> { 1337 }, cookie.GetAllocatedProjectIDs());
        }
    }
}
