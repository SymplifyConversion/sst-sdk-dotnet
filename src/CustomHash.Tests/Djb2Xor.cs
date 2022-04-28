using System;
using Xunit;

namespace CustomHash.Tests
{
    public class Djb2Xor
    {
        [Fact]
        public void Djb2XorString()
        {
            Assert.Equal(2_515_910_790, CustomHash.Djb2Xor("Fabian"));
        }

        [Fact]
        public void Djb2XorEmpty()
        {
            Assert.Equal((ulong)5_381, CustomHash.Djb2Xor(""));
        }

        [Theory]
        [InlineData("9e66a7fa-984a-4681-9319-80c2be2ffe8a", 913_699_141)]
        [InlineData("72784e9c-f5ae-4aed-8ae7-baa9c6e31d3c", 1_619_464_113)]
        [InlineData("cc615f71-1ab8-4322-b7d7-e10294a8d483", 3_367_636_261)]
        public void Djb2XorUuid(string str, ulong expected)
        {
            Assert.Equal(
                expected,
                CustomHash.Djb2Xor(str)
            );
        }
    }
}
