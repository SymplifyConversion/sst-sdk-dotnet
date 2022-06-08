using System.Text.Json;
using SymplifySDK.Allocation.Config;
using Xunit;

namespace SymplifySDK.Tests
{
    public class FindVariationForVisitorTests
    {
        readonly string ALLOCATION_TEST_PROJECT_JSON = @"{
            ""updated"": 1648466732,
            ""projects"": [
                {
                    ""id"": 4711,
                    ""name"":""discount"",
                    ""variations"": [
                        {
                            ""id"": 42,
                            ""name"": ""original"",
                            ""weight"": 67
                        },
                        {
                            ""id"": 1337,
                            ""name"": ""massive"",
                            ""weight"":33
                        }
                    ]
                },
                {
                    ""id"": 1337,
                    ""name"":""manyVariations"",
                    ""variations"": [
                        {
                            ""id"": 0,
                            ""name"": ""original"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 1,
                            ""name"": ""variation1"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 2,
                            ""name"": ""variation2"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 3,
                            ""name"": ""variation3"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 4,
                            ""name"": ""variation4"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 5,
                            ""name"": ""variation5"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 6,
                            ""name"": ""variation6"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 7,
                            ""name"": ""variation7"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 8,
                            ""name"": ""variation8"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 9,
                            ""name"": ""variation9"",
                            ""weight"": 10
                        },
                        {
                            ""id"": 10,
                            ""name"": ""variation10"",
                            ""weight"": 10
                        }
                    ]
                }
            ]
        }";

        private readonly ProjectConfig projectConfig;
        private readonly ProjectConfig projectConfigWithManyVariations;

        public FindVariationForVisitorTests()
        {
            SymplifyConfig config = JsonSerializer.Deserialize<SymplifyConfig>(ALLOCATION_TEST_PROJECT_JSON);
            projectConfig = config.Projects[0];
            projectConfigWithManyVariations = config.Projects[1];
        }

        [Theory]
        [InlineData("foobar", 42)]
        [InlineData("Fabian", 1337)]
        [InlineData("", null)]
        [InlineData("Alexander", 42)]
        [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 1337)]
        public void TestAllocateIsWeighted(string str, int? expected)
        {
            var variation = Allocation.Allocation.FindVariationForVisitor(projectConfig, str);

            Assert.Equal(expected, variation?.ID);
        }

        [Theory]
        [InlineData("foobar", 1)]
        [InlineData("Fabian", 7)]
        [InlineData("", null)]
        [InlineData("Alexander", 5)]
        [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 10)]
        public void TestAllocateWithManyVariations(string str, int? expected)
        {
            var variation = Allocation.Allocation.FindVariationForVisitor(projectConfigWithManyVariations, str);

            Assert.Equal(expected, variation?.ID);
        }
    }
}
