using Xunit;

namespace Symplify.Conversion.SDK.Tests
{
    public class HashInWindow
    {
        [Theory]
        [InlineData("", 1.2528616937931771E-05, 10)]
        [InlineData("Alexander", 0.50469162187182615, 10)] // not random
        [InlineData("Fabian", 5.8578112874780341, 10)] // not random
        [InlineData("Alexander", 50.469162187182611, 1_000)] // can scale up
        [InlineData("Fabian", 585.78112874780345, 1_000)] // can scale up
        [InlineData("Alexander", 0.050469162187182615, 1)] // can scale down
        [InlineData("Fabian", 1.1715622574956068, 2)] // can scale down
        public void HashInWindowIsNotRandom(string str, double expected, uint windowSize)
        {
            Assert.Equal(expected, CustomHash.HashInWindow(str, windowSize));
        }

        [Theory]
        [InlineData("9e66a7fa-984a-4681-9319-80c2be2ffe8a", 0.63821147746364859)]
        [InlineData("72784e9c-f5ae-4aed-8ae7-baa9c6e31d3c", 1.1311826156757732)]
        [InlineData("cc615f71-1ab8-4322-b7d7-e10294a8d483", 2.3522667552699028)]
        public void hashInWindowIsDistributed(string str, double expected)
        {
            Assert.Equal(expected, CustomHash.HashInWindow(str, 3));
        }


        [Theory]
        [InlineData("b7850777-f581-4f66-ad3e-4e54963661df", 100, 56.057958271367937)]
        public void hashInWindowIsCompatible(string str, uint window, double expected)
        {
            Assert.Equal(expected, CustomHash.HashInWindow(str, window));
        }
    }
}
