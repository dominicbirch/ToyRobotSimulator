using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToyRobotSimulator.Commands
{
    public class RobotCommandParser : ICommandParser<IRobot>
    {
        public delegate void RobotReportCallback(IPlacement report);

        readonly ILogger<RobotCommandParser> _logger;
        readonly RobotReportCallback _reportCallback;

        readonly Regex _placeCommand = new Regex("^\\s*PLACE\\s+\\(?\\s*(?'X'\\-?\\d+)\\s*(\\,|\\s+)\\s*(?'Y'\\-?\\d+)\\s*\\)?(\\,|\\s+)\\s*(?'F'\\w+)\\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(500));

        public RobotCommandParser(ILogger<RobotCommandParser> logger = null, RobotReportCallback reportCallback = null)
        {
            _logger = logger;
            _reportCallback = reportCallback;
        }

        public bool TryParse(string input, out ICommand<IRobot> result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(input)) return false;

            try
            {
                if (input.Trim().Equals("MOVE", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = new MoveForwardCommand();

                    return true;
                }

                if (input.Trim().Equals("LEFT", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = new TurnLeftCommand();

                    return true;
                }

                if (input.Trim().Equals("RIGHT", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = new TurnRightCommand();

                    return true;
                }

                if (input.Trim().Equals("REPORT", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = new ReportCommand(r => _reportCallback(r));

                    return true;
                }

                if (_placeCommand.IsMatch(input))
                {
                    var match = _placeCommand.Match(input);
                    var location = new Point(int.Parse(match.Groups["X"].Value), int.Parse(match.Groups["Y"].Value));
                    var orientation = Enum.GetNames(typeof(Direction))
                        .SingleOrDefault(n => n.Equals(match.Groups["F"].Value, StringComparison.InvariantCultureIgnoreCase));

                    if (orientation == null)
                        throw new ArgumentException($"Unrecognized orientation \'{match.Groups["F"].Value}\'");

                    result = new PlaceCommand(new Placement(location, Enum.Parse<Direction>(orientation)));

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger?.LogWarning(e, $"Failed to parse command:\'{input}\'");
            }

            return false;
        }
    }
}
