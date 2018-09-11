using System;
using ToyRobotSimulator.Commands;

namespace ToyRobotSimulator.Tests
{
    /// <summary>
    /// Robot command sequences.
    /// <para>These are written with builder and fluent builder methods.</para>
    /// </summary>
    internal static class Sequences
    {
        public static ICommandBuilder<IRobot> WalkPerimeteRight(this ICommandBuilder<IRobot> builder, int width)
            => builder
                .Repeat(4, b => b
                    .Repeat(width, bb => bb
                        .MoveForward())
                    .TurnRight());

        public static ICommandBuilder<IRobot> WalkPerimeterLeft(this ICommandBuilder<IRobot> builder, int width)
            => builder
                .Repeat(4, new CommandBuilder<IRobot>()
                    .Repeat(width, new MoveForwardCommand())
                    .TurnLeft()
                    .Build());

        public static ICommandBuilder<IRobot> RunExample(this ICommandBuilder<IRobot> builder, Action<IPlacement> reportCallback)
            => builder
                .Place(new Placement(new Point(0, 0), Direction.North))
                .MoveForward()
                .Report(reportCallback)
                .Place(new Placement(new Point(0, 0), Direction.North))
                .TurnLeft()
                .Report(reportCallback)
                .Place(new Placement(new Point(1, 2), Direction.East))
                .MoveForward()
                .MoveForward()
                .TurnLeft()
                .MoveForward()
                .Report(reportCallback);
    }
}
