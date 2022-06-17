using Xunit;

namespace Symplify.Conversion.SDK.Tests
{
    public class HashInWindow
    {
        [Theory]
        [InlineData("", 1, 10)]
        [InlineData("Alexander", 1, 10)] // not random
        [InlineData("Fabian", 6, 10)] // not random
        [InlineData("Alexander", 51, 1_000)] // can scale up
        [InlineData("Fabian", 586, 1_000)] // can scale up
        [InlineData("Alexander", 1, 1)] // can scale down
        [InlineData("Fabian", 2, 2)] // can scale down
        public void HashInWindowIsNotRandom(string str, int expected, uint windowSize)
        {
            Assert.Equal(expected, CustomHash.HashInWindow(str, windowSize));
        }

        [Theory]
        [InlineData("9e66a7fa-984a-4681-9319-80c2be2ffe8a", 1)]
        [InlineData("72784e9c-f5ae-4aed-8ae7-baa9c6e31d3c", 2)]
        [InlineData("cc615f71-1ab8-4322-b7d7-e10294a8d483", 3)]
        public void hashInWindowIsDistributed(string str, int expected)
        {
            Assert.Equal(expected, CustomHash.HashInWindow(str, 3));
        }
    }
}
