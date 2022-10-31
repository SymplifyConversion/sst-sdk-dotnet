using System;
using Symplify.Conversion.SDK.Allocation.Config;
using Xunit;

namespace Symplify.Conversion.SDK.Tests
{
    public class ConfigTest
    {
        private const string CONFIG_JSON_DISCOUNT = @"{
        ""updated"": 1648466732,
        ""projects"": [
            {
                ""id"": 4711,
                ""name"": ""discount"",
                ""variations"": [
                    {
                        ""id"": 42,
                        ""name"": ""original"",
                        ""weight"": 10
                    },
                    {
                        ""id"": 1337,
                        ""name"": ""huge"",
                        ""weight"": 2
                    },
                    {
                        ""id"": 9999,
                        ""name"": ""small"",
                        ""weight"": 1
                    }
                ]
            }
        ]}";

        private const string CONFIG_JSON_WITH_FLOAT_WEIGHTS = @"{
        ""updated"": 1648466732,
        ""projects"": [
            {
                ""id"": 4711,
                ""name"": ""discount"",
                ""variations"": [
                    {
                        ""id"": 42,
                        ""name"": ""original"",
                        ""weight"": 10.99
                    },
                    {
                        ""id"": 1337,
                        ""name"": ""huge"",
                        ""weight"": 2.9
                    },
                    {
                        ""id"": 9999,
                        ""name"": ""small"",
                        ""weight"": 1.9
                    }
                ]
            }
        ]}";


        private const string CONFIG_JSON_WITH_BOM = @"{
        ""updated"": 1648466732,
        ""projects"": [
            {
                ""id"": 4711,
                ""name"": ""discount"",
                ""variations"": [
                    {
                        ""id"": 42,
                        ""name"": ""original"",
                        ""weight"": 10
                    },
                    {
                        ""id"": 1337,
                        ""name"": ""huge"",
                        ""weight"": 2
                    },
                    {
                        ""id"": 9999,
                        ""name"": ""small"",
                        ""weight"": 1
                    }
                ]
            }
        ]}";

        private const string CONFIG_JSON_MISSING_ROOT_PROPERTY = @"{
        ""projects"": [
            {
            ""id"": 4711,
            ""name"": ""discount"",
            ""variations"": [
                {
                    ""id"": 42,
                    ""name"": ""original"",
                    ""weight"": 10
                },
                {
                    ""id"": 1337,
                    ""name"": ""huge"",
                    ""weight"": 2
                },
                {
                    ""id"": 9999,
                    ""name"": ""small"",
                    ""weight"": 1
                }
            ]
            }
        ]}";

        private const string CONFIG_JSON_MISSING_PROJECT_PROPERTY = @"{
        ""updated"": 1648466732,
        ""projects"": [
            {
            ""id"": 4711,
            ""variations"": [
                {
            ""id"": 42,
                    ""name"": ""original"",
                    ""weight"": 10
                },
                {
            ""id"": 1337,
                    ""name"": ""huge"",
                    ""weight"": 2
                },
                {
            ""id"": 9999,
                    ""name"": ""small"",
                    ""weight"": 1
                }
            ]
            }
        ]}";

        private const string CONFIG_JSON_MISSING_VARIATION_PROPERTY = @"{
        ""updated"": 1648466732,
        ""projects"": [
            {
                ""id"": 4711,
                ""name"": ""discount"",
                ""variations"": [
                    {
                        ""id"": 42,
                        ""name"": ""original"",
                        ""weight"": 10
                    },
                    {
                        ""id"": 1337,
                        ""name"": ""huge"",
                        ""weight"": 2
                    },
                    {
                        ""id"": 9999,
                        ""name"": ""small""
                    }
                ]
            }
        ]}";

        private const string CONFIG_JSON_WITH_PRIVACY_MODE_2 = @"{
        ""updated"": 1648466732,
        ""privacy_mode"": 2,
        ""projects"": [
            {
                ""id"": 4711,
                ""name"": ""discount"",
                ""variations"": [
                    { ""id"": 42, ""name"": ""original"", ""weight"": 10 },
                    { ""id"": 1337, ""name"": ""huge"", ""weight"": 2 }
                ]
            }
        ]}";


        [Fact]
        public void TestCanBeCreatedFromValidJSON()
        {
            SymplifyConfig config = new SymplifyConfig(CONFIG_JSON_DISCOUNT);

            Assert.Equal(1648466732, config.Updated);
            ProjectConfig project = config.Projects[0];
            Assert.Equal(4711, project.ID);
            Assert.Equal("discount", project.Name);

            VariationConfig variationOriginal = project.Variations[0];
            Assert.Equal(42, variationOriginal.ID);
            Assert.Equal("original", variationOriginal.Name);
            Assert.Equal((uint)10, variationOriginal.Weight);

            VariationConfig variationHuge = project.Variations[1];
            Assert.Equal(1337, variationHuge.ID);
            Assert.Equal("huge", variationHuge.Name);
            Assert.Equal((uint)2, variationHuge.Weight);

            VariationConfig variationSmall = project.Variations[2];
            Assert.Equal(9999, variationSmall.ID);
            Assert.Equal("small", variationSmall.Name);
            Assert.Equal((uint)1, variationSmall.Weight);
        }

        [Fact]
        public void TestCannotBeCreatedWithFloatWeights()
        {
            SymplifyConfig config = new SymplifyConfig(CONFIG_JSON_WITH_FLOAT_WEIGHTS);
            Assert.Equal(1648466732, config.Updated);
            Assert.Single(config.Projects);
            Assert.Equal(3, config.Projects[0].Variations.Count);
        }

        [Fact]
        public void TestCanBeCreatedFromValidJSONWithBOM()
        {
            SymplifyConfig config = new SymplifyConfig("\xEF\xBB\xBF" + CONFIG_JSON_WITH_BOM);

            Assert.Equal(1648466732, config.Updated);
            Assert.Single(config.Projects);
            Assert.Equal(3, config.Projects[0].Variations.Count);
        }

        [Fact]
        public void TestCannotBeCreatedFromInvalidJSON()
        {
            Assert.Throws<ArgumentException>(() => new SymplifyConfig("invalid"));
        }

        [Fact]
        public void TestCannotBeCreatedWithMissingRootFields()
        {
            Assert.Throws<ArgumentException>(() => new SymplifyConfig(CONFIG_JSON_MISSING_ROOT_PROPERTY));
        }

        [Fact]
        public void TestCannotBeCreatedWithMissingProjectFields()
        {
            SymplifyConfig config = new SymplifyConfig(CONFIG_JSON_MISSING_PROJECT_PROPERTY);
            Assert.Null(config.Projects[0].Name);
        }

        [Fact]
        public void TestCannotBeCreatedWithMissingVariationFields()
        {
            SymplifyConfig config = new SymplifyConfig(CONFIG_JSON_MISSING_PROJECT_PROPERTY);
            Assert.Equal((uint)1, config.Projects[0].Variations[2].Weight);
        }

        [Theory]
        [InlineData(CONFIG_JSON_DISCOUNT, 0)]
        [InlineData(CONFIG_JSON_WITH_PRIVACY_MODE_2, 2)]
        public void TestCanReadPrivacyMode(string json, int privacyMode)
        {
            Assert.Equal(privacyMode, (new SymplifyConfig(json)).Privacy_mode);
        }
    }
}
