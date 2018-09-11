using System;

namespace ToyRobotSimulator.Commands
{
    /// <summary>
    /// This command can be used to get a report from instances of <see cref="IRobot"/> and execute a callback on the result.
    /// </summary>
    public class ReportCommand : ICommand<IRobot>
    {
        readonly Action<IPlacement> _callback;

        public ReportCommand(Action<IPlacement> callback = null)
        {
            _callback = callback;
        }


        public void Execute(IRobot instance)
        {
            var report = instance?.Report();
            _callback?.Invoke(report);
        }
    }
}
