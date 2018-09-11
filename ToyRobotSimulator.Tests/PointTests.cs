using NSubstitute;
using Xunit;

namespace ToyRobotSimulator.Tests
{
    public class PointTests
    {
        [Fact(DisplayName = "Can create point from integers")]
        public void CanCreateFromIntegers()
        {
            var subject = new Point(1337, 7777);
            Assert.Equal(1337, subject.X);
            Assert.Equal(7777, subject.Y);
        }

        [Fact(DisplayName = "Can create point from another point")]
        public void ConstructFromOther()
        {
            var subject = new Point(new Point(666, 777));
            Assert.Equal(666, subject.X);
            Assert.Equal(777, subject.Y);
        }

        [Fact(DisplayName = "Can implicitly ToString()")]
        public void CanImplicitString()
        {
            var subject = new Point(6, 9);
            string result = subject;
            Assert.Equal("(6, 9)", result);
        }

        [Fact(DisplayName = "Can offset by point")]
        public void CanOffsetByPoint()
        {
            var subject = new Point(0, 0);
            var offset = new Point(-5, 5);
            subject.Offset(offset);
            Assert.Equal(offset, subject);
        }

        [Fact(DisplayName = "Can offset by integers")]
        public void CanOffsetByIntegers()
        {
            var subject = new Point(-5, -5);
            subject.Offset(5, 5);
            Assert.Equal(new Point(0, 0), subject);
        }
    }

    public class PlacementTests
    {
        [Fact(DisplayName = "Can create placement with values")]
        public void CreateWithValues()
        {
            var position = Substitute.For<IPoint>();
            var orientation = Direction.East;

            var subject = new Placement(position, orientation);

            Assert.Same(position, subject.Location);
            Assert.Equal(orientation, subject.Orientation);
        }

        [Fact(DisplayName = "Can implicitly ToString()")]
        public void CanImplicitString()
        {
            var position = Substitute.For<IPoint>();
            position.X.Returns(1);
            position.Y.Returns(2);
            var orientation = Direction.West;
            var subject = new Placement(position, orientation);
            string result = subject;

            Assert.Equal("(1, 2), West", result);
        }
    }
}
