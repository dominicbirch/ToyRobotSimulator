namespace ToyRobotSimulator
{
    /// <summary>
    /// A point in 2D space with an orientation expressed as an enumeration.
    /// </summary>
    public struct Placement : IPlacement
    {
        public IPoint Location { get; set; }
        public Direction Orientation { get; set; }

        public Placement(IPoint location, Direction orientation)
        {
            Location = location;
            Orientation = orientation;
        }

        public override string ToString()
            => $"({Location?.X ?? 0}, {Location?.Y ?? 0}), {Orientation}";
        public static implicit operator string(Placement placement) 
            => placement.ToString();
    }
}
