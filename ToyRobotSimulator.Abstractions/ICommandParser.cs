namespace ToyRobotSimulator
{
    public interface ICommandParser<T>
    {
        bool TryParse(string input, out ICommand<T> result);
    }
}
