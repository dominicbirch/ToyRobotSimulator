namespace ToyRobotSimulator
{
    public interface IRobot
    {
        void Place(IPlacement position);
        void MoveForward();
        void TurnLeft();
        void TurnRight();

        IPlacement Report();
    }
}
