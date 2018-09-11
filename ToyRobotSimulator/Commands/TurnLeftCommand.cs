namespace ToyRobotSimulator.Commands
{
    /// <summary>
    /// This command can be used to instruct instances of <see cref="IRobot"/> to turn left.
    /// </summary>
    public class TurnLeftCommand : ICommand<IRobot>
    {
        public void Execute(IRobot instance) => instance?.TurnLeft();
    }
}
