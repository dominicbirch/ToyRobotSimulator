using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;

namespace ToyRobotSimulator.Tests
{
    using Commands;
    using System.Collections.Generic;

    public class RobotTests
    {
        [Theory]
        [InlineData(3, 3, Direction.West)]
        [InlineData(-3, 3, Direction.North)]
        [InlineData(-5, -5, Direction.East)]
        [InlineData(0, 0, Direction.South)]
        [InlineData(1, -1, Direction.West)]
        public void RobotPlacedCorrectly(int startX, int startY, Direction startFacing)
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            var placed = new Placement(new Point(startX, startY), startFacing);

            subject.Place(placed);

            Assert.Equal(placed, subject.Report());
        }

        [Fact(DisplayName = "Robot correctly handles being placed twice")]
        public void RobotHandlesMultiplePlace()
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            var placed = new Placement(new Point(-1337, -1337), Direction.North);

            subject.Place(placed);

            Assert.Equal(placed, subject.Report());

            var placed2 = new Placement(new Point(2, 2), Direction.East);
            subject.Place(placed2);

            Assert.Equal(placed2, subject.Report());
        }

        [Fact(DisplayName = "Placed robot moves forward one unit relative to the direction it is facing")]
        public void PlacedRobotMovesForwardCorrectly()
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            var placed = new Placement(new Point(0, 0), Direction.North);

            subject.Place(placed);
            subject.MoveForward();
            placed.Location.Offset(0, 1);
            Assert.Equal(placed.Location, subject.Report().Location);

            placed.Orientation = Direction.South;
            subject.Place(placed);
            subject.MoveForward();
            placed.Location.Offset(0, -1);
            Assert.Equal(placed.Location, subject.Report().Location);

            placed.Orientation = Direction.West;
            subject.Place(placed);
            subject.MoveForward();
            placed.Location.Offset(-1, 0);
            Assert.Equal(placed.Location, subject.Report().Location);

            placed.Orientation = Direction.East;
            subject.Place(placed);
            subject.MoveForward();
            placed.Location.Offset(1, 0);
            Assert.Equal(placed.Location, subject.Report().Location);
        }

        [Fact(DisplayName = "Placed robot responds correctly to TurnLeft()")]
        public void RobotRespondsCorrectlyToTurnLeft()
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            subject.Place(new Placement(new Point(0, 0), Direction.East));

            subject.TurnLeft();
            Assert.Equal(Direction.North, subject.Report().Orientation);

            subject.TurnLeft();
            Assert.Equal(Direction.West, subject.Report().Orientation);

            subject.TurnLeft();
            Assert.Equal(Direction.South, subject.Report().Orientation);

            subject.TurnLeft();
            Assert.Equal(Direction.East, subject.Report().Orientation);
        }

        [Fact(DisplayName = "Placed robot responds correctly to TurnRight()")]
        public void RobotRespondsCorrectlyToTurnRight()
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            subject.Place(new Placement(new Point(0, 0), Direction.West));

            subject.TurnRight();
            Assert.Equal(Direction.North, subject.Report().Orientation);

            subject.TurnRight();
            Assert.Equal(Direction.East, subject.Report().Orientation);

            subject.TurnRight();
            Assert.Equal(Direction.South, subject.Report().Orientation);

            subject.TurnRight();
            Assert.Equal(Direction.West, subject.Report().Orientation);
        }

        [Theory]
        [InlineData(1, 0, 0, Direction.North)]
        [InlineData(2, 5, 0, Direction.South)]
        [InlineData(3, -5, -5, Direction.West)]
        [InlineData(4, -1, -1, Direction.East)]
        [InlineData(5, 0, 0, Direction.South)]
        public void WalkAroundRight(int width, int startX, int startY, Direction startFacing)
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            var finalPosition = default(IPlacement);
            var cmd = new CommandBuilder<IRobot>()
                .Place(new Placement(new Point(startX, startY), startFacing))
                .WalkPerimeteRight(width)
                .Report(p => finalPosition = p)
                .Build();

            cmd.Execute(subject);

            Assert.Equal(startFacing, finalPosition.Orientation);
            Assert.Equal(new Point(startX, startY), finalPosition.Location);
        }

        [Theory]
        [InlineData(1, 0, 0, Direction.North)]
        [InlineData(2, 5, 0, Direction.South)]
        [InlineData(3, -5, -5, Direction.West)]
        [InlineData(4, -1, -1, Direction.East)]
        [InlineData(5, 0, 0, Direction.South)]
        public void WalkAroundLeft(int width, int startX, int startY, Direction startFacing)
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            var finalPosition = default(IPlacement);
            var cmd = new CommandBuilder<IRobot>()
                .Place(new Placement(new Point(startX, startY), startFacing))
                .WalkPerimeterLeft(width)
                .Report(p => finalPosition = p)
                .Build();

            cmd.Execute(subject);

            Assert.Equal(startFacing, finalPosition.Orientation);
            Assert.Equal(new Point(startX, startY), finalPosition.Location);
        }

        [Fact(DisplayName = "Robot can execute example")]
        public void CanExecuteExample()
        {
            var subject = new Robot(Substitute.For<ILogger<Robot>>());
            var reportQueue = new Queue<IPlacement>();
            var cmd = new CommandBuilder<IRobot>()
                .RunExample(r => reportQueue.Enqueue(r))
                .Build();

            cmd.Execute(subject);

            Assert.Equal(new Placement(new Point(0, 1), Direction.North), reportQueue.Dequeue());
            Assert.Equal(new Placement(new Point(0, 0), Direction.West), reportQueue.Dequeue());
            Assert.Equal(new Placement(new Point(3, 3), Direction.North), reportQueue.Dequeue());
        }
    }
}
