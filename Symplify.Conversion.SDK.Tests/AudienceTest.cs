using System;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Symplify.Conversion.SDK.Audience;
using Xunit;

namespace Symplify.Conversion.SDK.Tests
{
    public class AudianceAttributes
    {
        [JsonPropertyName("suite_name")]
        public string SuiteName { get; set; }

        [JsonPropertyName("audience_json")]
        public Array AudienceJson { get; set; }

        [JsonPropertyName("test_cases")]
        public Array TestCases { get; set; }
    }

    public class AudienceTest
    {
        public static async Task<string> DownloadString(string url)
        {
            var client = new HttpClient();
            var data = await client.GetStringAsync(url);

            return data;
        }

        private async Task<dynamic> getData(string path)
        {
            string json = await DownloadString(path);

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            return serializer.Deserialize<dynamic>(reader);

        }

        [Fact]
        public async void TestAudienceAttributes()
        {
            dynamic jsonObject = await getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_attributes_spec.json");

            foreach (dynamic j in jsonObject)
            {
                SymplifyAudience audience = new SymplifyAudience(j["audience_json"]);
                foreach (dynamic testCase in j["test_cases"])
                {
                    string desc = testCase.ContainsKey("expect_error") ?
                                        String.Format("Attributes {0} should give err {1}", testCase["attributes"].ToString(), testCase["expect_error"].ToString()) :
                                        String.Format("Attributes {0} should give {1}", testCase["attributes"].ToString(), testCase["expect_result"].ToString());
                    //Console.WriteLine(desc);
                    var expectation = testCase["expect_result"] ?? testCase["expect_error"];
                    var actualResult = audience.Eval(testCase["attributes"]);
                    Assert.StrictEqual(expectation.Value, actualResult);
                }
            }
        }

        [Fact]
        public async void TestAudience()
        {
            dynamic jsonObject = await getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_spec.json");

            foreach (dynamic j in jsonObject)
            {
                foreach (dynamic testCase in j["test_cases"])
                {
                    string desc = testCase.ContainsKey("expect_error") ?
                                        String.Format("Audience {0} should give err {1}", testCase["audience_json"].ToString(), testCase["expect_error"].ToString()) :
                                        String.Format("Audience {0} should give {1}", testCase["audience_json"].ToString(), testCase["expect_result"].ToString());
                    //Console.WriteLine(desc);

                    SymplifyAudience audience = new SymplifyAudience(testCase["audience_json"]);
                    var expectation = testCase["expect_result"] ?? testCase["expect_error"];
                    var actualResult = audience.Eval(new JObject());
                    Assert.StrictEqual(expectation.Value, actualResult);
                }
            }
        }

        [Fact]
        public async void TestAudienceValidation()
        {
            dynamic jsonObject = await getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_validation_spec.json");

            foreach (dynamic j in jsonObject)
            {
                Console.WriteLine(j["suite_name"]);
                foreach (dynamic testCase in j["test_cases"])
                {
                    string desc = testCase.ContainsKey("expect_error") ?
                                        String.Format("Audience {0} should give err {1}", testCase["audience_string"].ToString(), testCase["expect_error"].ToString()) :
                                        String.Format("Audience {0} should give {1}", testCase["audience_string"].ToString(), testCase["expect_result"].ToString());
                    //Console.WriteLine(desc);

                    try
                    {
                        new SymplifyAudience(testCase["audience_string"]);
                    }
                    catch (Exception e)
                    {
                        Assert.Contains(testCase["expect_error"].ToString(), e.Message);
                    }

                }
            }
        }


        [Fact]
        public async void TestAudienceTracing()
        {
            dynamic jsonObject = await getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_tracing_spec.json");

            foreach (var j in jsonObject)
            {
                string desc = String.Format("Attributed {0}, should give expected trace {1}", j["attributes"].ToString(), j["expect_trace"].ToString());
                //Console.WriteLine(desc);
                SymplifyAudience audience = new SymplifyAudience(j["rules"]);
                var actualResult = audience.Trace(j["attributes"]);
                string actualResultJson = JsonConvert.SerializeObject(actualResult, Formatting.Indented);
                Assert.Equal(j["expect_trace"].ToString(), actualResultJson);
            }
        }
    }
}