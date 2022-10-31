using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Symplify.Conversion.SDK.Allocation.Config;
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

        private dynamic getData(string path)
        {
            WebClient client = new WebClient();
            string reply = client.DownloadString(path);


            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            JsonTextReader reader = new JsonTextReader(new StringReader(reply));
            return serializer.Deserialize<dynamic>(reader);

        }

        [Fact]
        public void TestAudienceAttributes()
        {
            dynamic jsonObject = getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_attributes_spec.json");

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
        public void TestAudience()
        {
            dynamic jsonObject = getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_spec.json");

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
        public void TestAudienceValidation()
        {
            dynamic jsonObject = getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_validation_spec.json");

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
        public void TestAudienceTracing()
        {
            dynamic jsonObject = getData("https://raw.githubusercontent.com/SymplifyConversion/sst-documentation/main/test/audience_tracing_spec.json");

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