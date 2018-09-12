using Xunit;

namespace ToyRobotSimulator.Tests
{
    using Commands;

    public class CommandParserTests
    {
        [Theory]
        [InlineData("REPORT")]
        [InlineData("Report")]
        [InlineData("MOVE")]
        [InlineData("move ")]
        [InlineData(" lEfT")]
        [InlineData("RiGhT  ")]
        [InlineData("PLACE 1 1 north")]
        [InlineData("Place 5 6 East")]
        [InlineData("PLACE -1, -1, SOUTH ")]
        [InlineData(" PLACE (-1, -1) west ")]
        public void CanParseValidCommand(string input)
        {
            var subject = new RobotCommandParser();
            var result = subject.TryParse(input, out var command);

            Assert.True(result);
            Assert.NotNull(command);
        }

        [Fact]
        public void HandlesInvalidOrientation()
        {
            var subject = new RobotCommandParser();
            var result = subject.TryParse("PLACE 10 10 DOWN", out var command);

            Assert.False(result);
            Assert.Null(command);
        }
    }
}
