using EnlEliteBot.Web;
using Xunit;

namespace XUnitTestProject1
{
    public class DistanceTests
    {

        [Fact]
        public void Test0s()
        {
            var distance = LocationHelper.CalcDistance(CoOrds.From(0, 0, 0), CoOrds.From(0, 0, 0));
            Assert.Equal(0, distance);
        }

        [Fact]
        public void TestXDimension()
        {
            var distance = LocationHelper.CalcDistance(CoOrds.From(0, 0, 0), CoOrds.From(4, 0, 0));
            Assert.Equal(4, distance);
        }

        [Fact]
        public void TestYDimension()
        {
            var distance = LocationHelper.CalcDistance(CoOrds.From(0, 0, 0), CoOrds.From(0, 4, 0));
            Assert.Equal(4, distance);
        }

        [Fact]
        public void TestZDimension()
        {
            var distance = LocationHelper.CalcDistance(CoOrds.From(0, 0, 0), CoOrds.From(0, 0, 4));
            Assert.Equal(4, distance);
        }

        [Fact]
        public void TestNegativeXDimension()
        {
            var distance = LocationHelper.CalcDistance(CoOrds.From(0, 0, 0), CoOrds.From(-4, 0, 0));
            Assert.Equal(4, distance);
        }

        [Fact]
        public void Test3Dimension()
        {
            var distance = LocationHelper.CalcDistance(CoOrds.From(0, 0, 0), CoOrds.From(1, 1, 1));
            Assert.Equal(1.73, distance);
        }
    }

    class CoOrds : ICoordinates
    {
        public static CoOrds From(float x, float y, float z)
        {
            return new CoOrds()
            {
                x = x,
                y = y,
                z = z
            };
        }

        public float y { get; set; }

        public float z { get; set; }

        public float x { get; set; }
    }
}
