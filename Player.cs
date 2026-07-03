using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLudo
{
    public class Player
    {
        public string Name { get; }
        public ConsoleColor Color { get; }
        public int StartIndex { get; }
        public List<Pawn> Pawns { get; } = new List<Pawn>();

        public Player(string name, ConsoleColor color, int startIndex)
        {
            Name = name;
            Color = color;
            StartIndex = startIndex;
            for (int i = 0; i < 4; i++) Pawns.Add(new Pawn(this));
        }

        public bool AllFinished() => Pawns.All(p => p.IsFinished());
    }
}
