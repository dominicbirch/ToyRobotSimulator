using Microsoft.Extensions.Logging;

namespace ToyRobotSimulator.Decorators
{
    /// <summary>
    /// This decorator robot filters commands based on the constraints of an <see cref="IEnvironment"/>.
    /// </summary>
    public class ConstrainedRobot : IRobot
    {
        private readonly ILogger<ConstrainedRobot> _logger;

        private readonly IRobot _innerRobot;
        private readonly IEnvironment _environment;

        private bool _hasBeenPlaced;

        public ConstrainedRobot(IRobot innerRobot, IEnvironment environment, ILogger<ConstrainedRobot> logger = null)
        {
            _innerRobot = innerRobot;
            _environment = environment;

            _logger = logger;
        }


        public void MoveForward()
        {
            if (!_hasBeenPlaced)
            {
                _logger?.LogTrace("Not yet placed.");

                return;
            }

            var report = _innerRobot.Report();
            var newPosition = new Point(report.Location);

            switch (report.Orientation)
            {
                case Direction.North:
                    newPosition.Offset(0, 1);
                    break;

                case Direction.East:
                    newPosition.Offset(1, 0);
                    break;

                case Direction.South:
                    newPosition.Offset(0, -1);
                    break;

                case Direction.West:
                    newPosition.Offset(-1, 0);
                    break;
            }

            if (_environment.CheckInBounds(newPosition)) _innerRobot.MoveForward();
            else _logger?.LogTrace($"{newPosition} is out of bounds; command will be ignored");
        }

        public void Place(IPlacement position)
        {
            if (position == null)
                return;
            if (!_environment.CheckInBounds(position?.Location))
            {
                _logger?.LogTrace($"{position.Location} is out of bounds; command will be ignored");

                return;
            }
            else if (position?.Orientation == Direction.None)
            {
                _logger?.LogTrace("Placement must have an orientation; command will be ignored");

                return;
            }

            _innerRobot.Place(position);
            _hasBeenPlaced = true;
        }

        public IPlacement Report()
        {
            if (!_hasBeenPlaced)
            {
                _logger?.LogTrace("Not yet placed.");

                return null;
            }

            return _innerRobot.Report();
        }

        public void TurnLeft()
        {
            if (_hasBeenPlaced) _innerRobot.TurnLeft();
            else _logger?.LogTrace("Not yet placed.");
        }

        public void TurnRight()
        {
            if (_hasBeenPlaced) _innerRobot.TurnRight();
            else _logger?.LogTrace("Not yet placed.");
        }
    }
}
