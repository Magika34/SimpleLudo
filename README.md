# SimpleLudo

SimpleLudo is a small console-based implementation of the classic board game Ludo written in C# targeting .NET Framework 4.7.2.

## Features
- Four players: Red, Green, Yellow, Blue
- Console UI with simple ASCII board rendering
- Dice rolling, pawn entry on rolling 6, exact finish requirement
- Capture opponent pawns (send them home)

## Requirements
- Windows
- Visual Studio (2019/2022/2026) or MSBuild that supports .NET Framework 4.7.2

## Build and run
1. Open `SimpleLudo.slnx` in Visual Studio and build the solution.
2. Run the project (Start Without Debugging) or run the produced executable from `bin\Debug` or `bin\Release`.

Alternative (msbuild):

msbuild SimpleLudo.slnx /p:Configuration=Debug

## Gameplay
- On your turn press Enter to roll the dice.
- Rolling a 6 lets a pawn leave home and gives an extra turn.
- Pawns move around the perimeter; you must roll the exact number to reach the goal.
- Landing on an opponent's pawn sends it back to their home.
- First player to get all pawns to the goal wins.

## Project structure (high level)
- Game.cs — game loop, input, move logic, animation
- LudoBoard.cs — board representation and drawing
- Player.cs — player state and pawns
- Pawn.cs — pawn state and movement

## Repository
https://github.com/Magika34/SimpleLudo
