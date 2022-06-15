using Xunit;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;

using SymplifySDK.Cookies;

namespace SymplifySDK.Tests
{
    public class CompatibilityTestCase
    {
        public string Skip { get; set; }
        public string TestName { get; set; }
        public string SDKConfig { get; set; }
        public string WebsiteID { get; set; }
        public string TestProjectName { get; set; }
        public string ExpectVariationMatch { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public JObject ExpectSgCookiePropertiesMatch { get; set; }

        override public string ToString()
        {
            return TestName;
        }
    }

    public class CompatibilityTest
    {

        readonly ITestOutputHelper output;

        public CompatibilityTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        HttpClient fakeHttpClient(string responseBody = "{}", string contentType = "application/json")
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("*").Respond(contentType, responseBody);

            return mockHttp.ToHttpClient();
        }

        public static IEnumerable<object[]> CompatibilityTestData()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "test_cases.json");
            var json = File.ReadAllText(filePath);

            var testCases = JArray.Parse(json).Select(c => new CompatibilityTestCase
            {
                Skip = (string)c["skip"],
                TestName = (string)c["test_name"],
                SDKConfig = (string)c["sdk_config"],
                WebsiteID = (string)c["website_id"],
                TestProjectName = (string)c["test_project_name"],
                ExpectVariationMatch = (string)c["expect_variation_match"],
                Cookies = c["cookies"]?.ToObject<Dictionary<string, string>>(),
                ExpectSgCookiePropertiesMatch = c["expect_sg_cookie_properties_match"]?.ToObject<JObject>(),
            }).ToList();

            foreach (var test in testCases)
            {
                yield return new[] { test };
            }
        }

        static string ReadConfig(string filename)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", filename);
            var json = File.ReadAllText(filePath);
            return json;
        }

        static void AssertMatchOrBothNull(string expected, string actual)
        {
            if (expected != null)
            {
                Assert.Matches(expected, actual);
            }
            else
            {
                Assert.Equal(expected, actual);
            }
        }

        [Theory]
        [MemberData(nameof(CompatibilityTestData))]
        async public void SDKIsCompatible(CompatibilityTestCase test)
        {
            // HACK but we need xunit v3 before we can skip tests dynamically.
            if (test.Skip != null)
            {
                Assert.Equal(test.Skip, test.Skip);
                return;
            }

            // prepare the client data
            var sdkConfig = ReadConfig(test.SDKConfig);
            ClientConfig cfg = new ClientConfig(test.WebsiteID, "https://cdn.example.com");
            SymplifyClient client = new SymplifyClient(cfg, fakeHttpClient(sdkConfig), new DefaultLogger());
            await client.LoadConfig();

            // prepare the per request data
            CookieJar cookieJar = new();
            foreach (var cookie in test?.Cookies ?? new())
            {
                cookieJar.SetCookie(cookie.Key, cookie.Value);
            }

            // simulate the request
            var variation = client.FindVariation(test.TestProjectName, cookieJar);

            // verify the allocated variation
            AssertMatchOrBothNull(test.ExpectVariationMatch, variation);

            // verify cookie afterwards
            foreach (var expect in test?.ExpectSgCookiePropertiesMatch?.Properties())
            {
                // get the root object first
                var node = JObject.Parse(WebUtility.UrlDecode(cookieJar.GetCookie(SymplifyCookie.CookieName) ?? "{}")).Root;
                foreach (var part in expect.Name.Split('/'))
                {
                    // traverse to an expected leaf
                    if (node is not JObject)
                    {
                        break;
                    }
                    node = node[part];
                }

                // verify the expected leaf value
                if (expect.Value.Type == JTokenType.String)
                {
                    AssertMatchOrBothNull((string)expect.Value, (string)node);
                }
                else if (expect.Value.Type == JTokenType.Null)
                {
                    Assert.Null(node);
                }
                else
                {
                    Assert.Equal(expect.Value, node);
                }
            }
        }
    }
}
