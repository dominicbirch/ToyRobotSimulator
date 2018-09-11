namespace ToyRobotSimulator.Commands
{ 
    /// <summary>
    /// This command can be used to place instances of <see cref="IRobot"/>.
    /// </summary>
    public class PlaceCommand : ICommand<IRobot>
    {
        readonly IPlacement _placement;

        public PlaceCommand(IPlacement placement)
        {
            _placement = placement;
        }


        public void Execute(IRobot instance) => instance?.Place(_placement);
    }
}
