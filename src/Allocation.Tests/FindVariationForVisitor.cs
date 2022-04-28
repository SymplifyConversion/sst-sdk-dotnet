using System;
using System.Collections.Generic;
using Allocation.Config;
using Xunit;

namespace Allocation.Test
{
    public class FindVariationForVisitor
    {
        private readonly ProjectConfig projectConfig;

        public FindVariationForVisitor()
        {
            List<VariationConfig> variations = new()
            {
                new VariationConfig(42, "original", 2),
                new VariationConfig(1337, "massive", 1)
            };

            projectConfig = new ProjectConfig(4711, "discount", variations);
        }

        [Theory]
        [InlineData("foobar", 42)]
        [InlineData("Fabian", 1337)]
        [InlineData("", 42)]
        [InlineData("Alexander", 42)]
        [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 1337)]
        public void TestAllocateIsWeighted(string str, int expected)
        {
            var variation = Allocation.FindVariationForVisitor(projectConfig, str);

            Assert.Equal(expected, variation.ID);
        }
    }
}
