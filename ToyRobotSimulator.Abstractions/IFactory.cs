namespace ToyRobotSimulator
{
    public interface IFactory<out T>
    {
        T Create();
    }
}
