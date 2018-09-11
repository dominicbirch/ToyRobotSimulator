using System;

namespace ToyRobotSimulator
{
    /// <summary>
    /// A rect in space with no other constraints.
    /// </summary>
    public class BasicTableSurface : IEnvironment
    {
        readonly IPoint _southWest;
        readonly IPoint _northEast;

        public BasicTableSurface(IPoint point1, IPoint point2)
        {
            _southWest = new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            _northEast = new Point(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
        }

        public bool CheckInBounds(IPoint point)
            => point.X >= _southWest.X && point.X <= _northEast.X
            && point.Y >= _southWest.Y && point.Y <= _northEast.Y;
    }
}
