# Toy Robot Simulator

Answer to interview question written as a .NET Core 2 library and console application.

This demonstrates some simple examples of:
- Command Pattern
- Builder/Fluent Builder pattern
- Decorator Pattern

There are some integrated tests to demonstrate the behaviours requested using xUnit and NSubstitute as well as a stdin/stdout interpreter.

---
## Example Input and Output:
```
PLACE 0,0,NORTH
MOVE
REPORT
Output: 0,1,NORTH
PLACE 0,0,NORTH
LEFT
REPORT
Output: 0,0,WEST
PLACE 1,2,EAST
MOVE
MOVE
LEFT
MOVE
REPORT
Output: 3,3,NORTH
```

## CLI
It's possible to provide arguments on startup which are either commands to be executed in order or paths to files containing the commands.