using System;

namespace ToyRobotSimulator.Commands
{
    /// <summary>
    /// Fluent builder methods for easy-populating of <see cref="ICommandBuilder{T}"/> instances.
    /// </summary>
    public static class CommandBuilderExtensions
    {
        public static ICommandBuilder<IRobot> Place(this ICommandBuilder<IRobot> builder, IPlacement placement)
            => builder.Add(new PlaceCommand(placement));

        public static ICommandBuilder<IRobot> Report(this ICommandBuilder<IRobot> builder, Action<IPlacement> callback)
            => builder.Add(new ReportCommand(callback));

        public static ICommandBuilder<IRobot> MoveForward(this ICommandBuilder<IRobot> builder)
            => builder.Add(new MoveForwardCommand());

        public static ICommandBuilder<IRobot> TurnLeft(this ICommandBuilder<IRobot> builder) 
            => builder.Add(new TurnLeftCommand());

        public static ICommandBuilder<IRobot> TurnRight(this ICommandBuilder<IRobot> builder)
            => builder.Add(new TurnRightCommand());

        /// <summary>
        /// Adds <paramref name="command"/> to the builder <paramref name="iterations"/> times.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="iterations"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static ICommandBuilder<IRobot> Repeat(this ICommandBuilder<IRobot> builder, int iterations, ICommand<IRobot> command) 
        {
            for (var iteration = 0; iteration < iterations; iteration++)
                builder.Add(command);

            return builder;
        }

        public static ICommandBuilder<IRobot> Repeat(this ICommandBuilder<IRobot> builder, int iterations, Func<ICommandBuilder<IRobot>, ICommandBuilder<IRobot>> processToRepeat)
        {
            for (var iteration = 0; iteration < iterations; iteration++)
                processToRepeat(builder);

            return builder;
        }
    }
}
