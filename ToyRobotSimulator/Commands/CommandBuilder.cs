using System;

namespace ToyRobotSimulator.Commands
{
    /// <summary>
    /// Used to create commands which execute each of the supplied commands once in sequence.
    /// </summary>
    /// <typeparam name="T">The type which the built commands execute against.</typeparam>
    public class CommandBuilder<T> : ICommandBuilder<T>
    {
        Func<T, T> _sequence = i => i;
        Func<T, T> Compose(Func<T, T> f1, Action<T> f2)
            => robot =>
            {
                f2(f1(robot));

                return robot;
            };

        public ICommandBuilder<T> Add(ICommand<T> command)
        {
            _sequence = Compose(_sequence, robot => command.Execute(robot));

            return this;
        }

        public ICommand<T> Build() => new BuilderCommand(_sequence);



        class BuilderCommand : ICommand<T>
        {
            readonly Func<T, T> _sequence;
            public BuilderCommand(Func<T, T> sequence) => _sequence = sequence;

            public void Execute(T instance) => _sequence?.Invoke(instance);
        }
    }
}
