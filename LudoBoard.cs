using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLudo
{
    public enum CellType { Empty, Perimeter, Start, HomeCorner, Lane, Goal }

    public class Cell
    {
        public int Index { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public CellType Type { get; set; }
        public bool IsSafe { get; set; }
    }

    public class LudoBoard
    {
        public Dictionary<int, Cell> Cells { get; } = new Dictionary<int, Cell>();
        public int GoalIndex { get; } = 41;
        public int[] StartIndices { get; } = { 1, 11, 21, 31 };

        public LudoBoard()
        {
            Build();
        }

        void Build()
        {
            // Build an 11x11 mapping similar to the console visuals
            var coords = new List<Tuple<int, int>>();
            for (int y = 5; y >= 0; y--) coords.Add(Tuple.Create(0, y));
            for (int x = 1; x <= 10; x++) coords.Add(Tuple.Create(x, 0));
            for (int y = 1; y <= 10; y++) coords.Add(Tuple.Create(10, y));
            for (int x = 9; x >= 0; x--) coords.Add(Tuple.Create(x, 10));
            for (int y = 9; y >= 6; y--) coords.Add(Tuple.Create(0, y));

            for (int i = 0; i < coords.Count; i++)
            {
                var c = coords[i];
                Cells[i + 1] = new Cell { Index = i + 1, X = c.Item1, Y = c.Item2, Type = CellType.Perimeter };
            }

            // center
            Cells[GoalIndex] = new Cell { Index = GoalIndex, X = 5, Y = 5, Type = CellType.Goal };

            // mark starts
            foreach (var s in StartIndices)
            {
                if (Cells.ContainsKey(s)) Cells[s].Type = CellType.Start;
            }

            // mark corners as home areas (not in Cells list because they're interior); keep IsSafe false by default
        }

        public int GetNextIndex(int idx)
        {
            if (idx <= 0) return idx;
            return idx % 40 + 1;
        }

        public Tuple<int, int> GetCoords(int index)
        {
            if (Cells.TryGetValue(index, out var c)) return Tuple.Create(c.X, c.Y);
            if (index == GoalIndex) return Tuple.Create(5, 5);
            return null;
        }

        public void Draw(List<Player> players)
        {
            // reuse console board drawing but using board data
            Console.WriteLine();

            // Show Home statuses
            Console.WriteLine("Players:");
            foreach (var p in players)
            {
                Console.ForegroundColor = p.Color;
                Console.WriteLine($" {p.Name} - Pawns: Home={p.Pawns.Count(pa => pa.IsAtHome())} OnTrack={p.Pawns.Count(pa => pa.IsOnTrack())} Finished={p.Pawns.Count(pa => pa.IsFinished())}");
            }
            Console.ResetColor();

            // build grid index
            var gridIndex = new int[11, 11];
            for (int yy = 0; yy < 11; yy++) for (int xx = 0; xx < 11; xx++) gridIndex[xx, yy] = -1;
            foreach (var kv in Cells) gridIndex[kv.Value.X, kv.Value.Y] = kv.Key;

            Console.WriteLine();
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    int idx = gridIndex[x, y];

                    // players here
                    var playersHere = new List<Player>();
                    var pawnsHere = new List<string>();
                    foreach (var pl in players)
                    {
                        foreach (var pa in pl.Pawns)
                        {
                            if (pa.Index != 0 && pa.Index == idx) pawnsHere.Add(pl.Name[0].ToString());
                            if (pa.Index != 0 && pa.Index == GoalIndex && idx == GoalIndex) pawnsHere.Add(pl.Name[0].ToString());
                        }
                    }

                    // center goal
                    if (x == 5 && y == 5)
                    {
                        Console.Write("[ G ]");
                        continue;
                    }

                    if (idx == -1)
                    {
                        // interior or corner
                        Console.Write("[   ]");
                    }
                    else
                    {
                        // show pawns if present
                        if (pawnsHere.Count > 0)
                        {
                            string s = pawnsHere.Count == 1 ? $" {pawnsHere[0]} " : "*";
                            Console.Write($"[{s}]");
                        }
                        else if (StartIndices.Contains(idx))
                        {
                            int owner = Array.IndexOf(StartIndices, idx);
                            Console.ForegroundColor = players[owner].Color;
                            Console.Write("[ S ]");
                            Console.ResetColor();
                        }
                        else
                        {
                            // arrow to indicate path
                            int next = GetNextIndex(idx);
                            var cur = Cells[idx];
                            var nxt = Cells[next];
                            int dx = nxt.X - cur.X;
                            int dy = nxt.Y - cur.Y;
                            char arrow = '·';
                            if (dx == 1) arrow = '>'; else if (dx == -1) arrow = '<'; else if (dy == 1) arrow = 'v'; else if (dy == -1) arrow = '^';
                            Console.Write($"[{arrow}]");
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
