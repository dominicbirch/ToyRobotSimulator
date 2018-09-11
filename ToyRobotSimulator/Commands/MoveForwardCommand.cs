namespace ToyRobotSimulator.Commands
{
    /// <summary>
    /// This command instructs a <see cref="IRobot"/> to move forward.
    /// </summary>
    public class MoveForwardCommand : ICommand<IRobot>
    {
        public void Execute(IRobot instance) => instance?.MoveForward();
    }
}
