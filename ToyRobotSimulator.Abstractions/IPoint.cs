using System;
using System.Collections.Generic;

namespace ToyRobotSimulator
{
    public interface IPoint
    {
        int X { get; set; }
        int Y { get; set; }

        void Offset(IPoint other);
        void Offset(int x, int y);
    }

    public interface ISequencer<T>
    {
        void Clear();
        void AddStep(ICommand<T> step);
        IEnumerable<ICommand<T>> GetCommands(); 
    }
}
