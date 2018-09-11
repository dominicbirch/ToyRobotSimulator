namespace ToyRobotSimulator
{
    public interface ICommand<T>
    {
        void Execute(T instance);
    }


}
