namespace ToyRobotSimulator
{
    /// <summary>
    /// A point in 2D space.
    /// </summary>
    public struct Point : IPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point(IPoint other)
        {
            X = other.X;
            Y = other.Y;
        }
        
        public void Offset(IPoint other)
        {
            X += other.X;
            Y += other.Y;
        }

        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }

        public override string ToString()
            => $"({X}, {Y})";
        public static implicit operator string(Point point) => point.ToString();
    }
}
