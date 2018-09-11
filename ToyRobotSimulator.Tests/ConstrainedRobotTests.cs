using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using ToyRobotSimulator.Commands;
using ToyRobotSimulator.Decorators;
using Xunit;

namespace ToyRobotSimulator.Tests
{
    public class ConstrainedRobotTests
    {
        [Fact(DisplayName = "Unplaced constrained robot will not move forward")]
        public void CannotMoveForwardUntilPlaced()
        {
            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.MoveForward();

            inner.DidNotReceiveWithAnyArgs().MoveForward();
        }
        [Fact(DisplayName = "Placed constrained robot will move forward")]
        public void CanMoveForwardOncePlaced()
        {
            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(new Placement(new Point(1, 1), Direction.East));
            subject.MoveForward();

            inner.ReceivedWithAnyArgs().MoveForward();
        }

        [Fact(DisplayName = "Unplaced constrained robot will not turn left")]
        public void CannotTurnLeftUntilPlaced()
        {
            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.TurnLeft();

            inner.DidNotReceiveWithAnyArgs().TurnLeft();
        }
        [Fact(DisplayName = "Constrained robot will turn left once placed")]
        public void CanTurnLeftOncePlaced()
        {
            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(new Placement(new Point(0, 0), Direction.North));
            subject.TurnLeft();

            inner.ReceivedWithAnyArgs().TurnLeft();
        }

        [Fact(DisplayName = "Unplaced constrained robot will not turn right")]
        public void CannotTurnRightUntilPlaced()
        {
            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.TurnRight();

            inner.DidNotReceiveWithAnyArgs().TurnRight();
        }
        [Fact(DisplayName = "Constrained robot will turn right once placed")]
        public void CanTurnRightOncePlaced()
        {
            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(new Placement(new Point(0, 0), Direction.North));
            subject.TurnRight();

            inner.ReceivedWithAnyArgs().TurnRight();
        }

        [Fact(DisplayName = "Constrained robot will not report until placed")]
        public void CannotReportUntilPlaced()
        {
            Point sw = new Point(0, 0), ne = new Point(5, 5);

            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(sw, ne);
            var subject = new ConstrainedRobot(inner, environment, logger);
            var resultsQueue = new Queue<IPlacement>();

            resultsQueue.Enqueue(subject.Report());
            inner.DidNotReceiveWithAnyArgs().Report();

            subject.Place(new Placement(new Point(sw), Direction.South));
            resultsQueue.Enqueue(subject.Report());
            inner.ReceivedWithAnyArgs().Report();

            Assert.Null(resultsQueue.Dequeue());
            Assert.NotNull(resultsQueue.Dequeue());
        }

        [Fact(DisplayName = "Cannot place constrained robot off grid")]
        public void CannotPlaceOffGrid()
        {
            Point sw = new Point(0, 0), ne = new Point(5, 5);

            var inner = Substitute.For<IRobot>();
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(sw, ne);
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(new Placement(new Point(6, 6), Direction.West));

            inner.DidNotReceiveWithAnyArgs().Place(Arg.Any<IPlacement>());
        }

        [Theory(DisplayName = "Cannot move off grid")]
        [InlineData(Direction.North)]
        [InlineData(Direction.East)]
        [InlineData(Direction.South)]
        [InlineData(Direction.West)]
        public void CannotMoveOffGrid(Direction direction)
        {
            Point sw = new Point(0, 0), ne = new Point(0, 0);

            var inner = Substitute.ForPartsOf<Robot>(default(ILogger<Robot>));
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(sw, ne);
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(new Placement(new Point(0, 0), direction));
            subject.MoveForward();

            inner.DidNotReceiveWithAnyArgs().MoveForward();
        }

        [Theory(DisplayName = "Constrained robot can move within bounds")]
        [InlineData(Direction.North)]
        [InlineData(Direction.East)]
        [InlineData(Direction.South)]
        [InlineData(Direction.West)]
        public void CanMoveWIthinBounds(Direction direction)
        {
            Point sw = new Point(int.MinValue, int.MinValue), ne = new Point(int.MaxValue, int.MaxValue);

            var inner = Substitute.ForPartsOf<Robot>(default(ILogger<Robot>));
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(sw, ne);
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(new Placement(new Point(0, 0), direction));
            subject.MoveForward();

            inner.ReceivedWithAnyArgs().MoveForward();
        }

        [Fact(DisplayName = "Constrained robot can execute example")]
        public void CanExecuteExample()
        {
            var inner = Substitute.ForPartsOf<Robot>(default(ILogger<Robot>));
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);
            var reportQueue = new Queue<IPlacement>();
            var cmd = new CommandBuilder<IRobot>()
                .RunExample(r => reportQueue.Enqueue(r))
                .Build();

            cmd.Execute(subject);

            Assert.Equal(new Placement(new Point(0, 1), Direction.North), reportQueue.Dequeue());
            Assert.Equal(new Placement(new Point(0, 0), Direction.West), reportQueue.Dequeue());
            Assert.Equal(new Placement(new Point(3, 3), Direction.North), reportQueue.Dequeue());
        }

        [Fact(DisplayName ="")]
        public void DoesNotAcceptOrientationNone()
        {
            var inner = Substitute.ForPartsOf<Robot>(default(ILogger<Robot>));
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(new Placement(new Point(1, 1), Direction.None));

            inner.DidNotReceiveWithAnyArgs().Place(Arg.Any<Placement>());
        }

        [Fact(DisplayName = "Handles Place(null)")]
        public void HandlesPlaceNull()
        {
            var inner = Substitute.ForPartsOf<Robot>(default(ILogger<Robot>));
            var logger = Substitute.For<ILogger<ConstrainedRobot>>();
            var environment = Substitute.ForPartsOf<BasicTableSurface>(new Point(0, 0), new Point(5, 5));
            var subject = new ConstrainedRobot(inner, environment, logger);

            subject.Place(null);

            inner.DidNotReceiveWithAnyArgs().Place(Arg.Any<Placement>());
        }
    }
}
