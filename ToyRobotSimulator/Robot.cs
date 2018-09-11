using Microsoft.Extensions.Logging;

namespace ToyRobotSimulator
{
    /// <summary>
    /// Dumb robot, can be placed, move around with 3 commands and report.
    /// <para>No concerns other than movement.</para>
    /// </summary>
    public class Robot : IRobot
    {
        private ILogger<Robot> _logger;

        private IPoint _currentPosition;
        private Direction _currentDirection;

        public Robot(ILogger<Robot> logger = null)
        {
            _logger = logger;
        }


        public IPlacement Report() 
            => new Placement(_currentPosition, _currentDirection);

        public void Place(IPlacement position)
        {
            _logger?.LogTrace($"Placed at: {position}");

            _currentPosition = position.Location;
            _currentDirection = position.Orientation;
        }

        public void MoveForward()
        {
            _logger?.LogTrace($"Moving: {_currentDirection}");

            switch (_currentDirection)
            {
                case Direction.North:
                    ++_currentPosition.Y;
                    break;

                case Direction.East:
                    ++_currentPosition.X;
                    break;

                case Direction.South:
                    --_currentPosition.Y;
                    break;

                case Direction.West:
                    --_currentPosition.X;
                    break;
            }
        }

        public void TurnLeft()
        {
            if (_currentDirection == Direction.North)
            {
                _currentDirection = Direction.West;
                _logger?.LogTrace($"Turned left to face {_currentDirection}");

                return;
            }

            --_currentDirection;
            _logger?.LogTrace($"Turned left to face {_currentDirection}");
        }

        public void TurnRight()
        {
            if (_currentDirection == Direction.West)
            {
                _currentDirection = Direction.North;
                _logger?.LogTrace($"Turned right to face {_currentDirection}");

                return;
            }

            ++_currentDirection;
            _logger?.LogTrace($"Turned right to face {_currentDirection}");
        }
    }
}
