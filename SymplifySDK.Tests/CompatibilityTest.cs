using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Web;

using SymplifySDK.Allocation;
using SymplifySDK.Allocation.Config;

namespace SymplifySDK.Tests
{
    public class CompatibilityTestCase {
        public string skip { get; set; }
        public string test_name { get; set; }
        public string sdk_config { get; set; }
        public string website_id { get; set; }
        public string test_project_name { get; set; }
        public string expect_variation_match { get; set; }
        public Dictionary<string, string> cookies { get; set; }
        public Dictionary<string, string> expect_sg_cookie_properties_match { get; set; }

        override public string ToString() {
            return test_name;
        }
    }

    public class CompatibilityTest
    {

        public static IEnumerable<object[]> CompatibilityTestData()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "test_cases.json");
            var json = File.ReadAllText(filePath);
            var testCases = JsonSerializer.Deserialize<List<CompatibilityTestCase>>(json);
            foreach (var test in testCases)
            {
                yield return new[] { test };
            }
        }

        public static SymplifyConfig ReadConfig(string filename)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", filename);
            var json = File.ReadAllText(filePath);
            return new SymplifyConfig(json);
        }

        [Theory]
        [MemberData(nameof(CompatibilityTestData))]
        public void SDKIsCompatible(CompatibilityTestCase test)
        {
            // HACK but we need xunit v3 before we can skip tests dynamically.
            if (test.skip != null) {
                Assert.Equal(test.skip, test.skip);
                return;
            }

            var sdkConfig = ReadConfig(test.sdk_config);
            var projConfig = sdkConfig.FindProjectWithName(test.test_project_name);
            var sg_cookiesJSON = HttpUtility.UrlDecode(test?.cookies?["sg_cookies"] ?? "{}");
            var sg_cookies = JsonDocument.Parse(sg_cookiesJSON).RootElement;

            JsonElement websiteData;
            JsonElement visid;
            string visitorID = "";

            if (sg_cookies.TryGetProperty(test.website_id, out websiteData)) {
                if (websiteData.TryGetProperty("visid", out visid)) {
                    visitorID = visid.GetString();
                }
            }

            var variation = Allocation.Allocation.FindVariationForVisitor(projConfig, visitorID);

            Assert.Equal(test.expect_variation_match, variation?.Name);
        }
    }
}
