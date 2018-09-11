namespace ToyRobotSimulator
{
    public interface IPlacement
    {
        IPoint Location { get; set; }
        Direction Orientation { get; set; }
    }
}
