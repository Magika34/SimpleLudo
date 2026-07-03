using System;
using System.Linq;
using System.Threading;

namespace SimpleLudo
{
    class Program
    {
        const int GOAL = 41; // Center slot index
        static int[] playerPositions = { 0, 0, 0, 0 };
        static string[] playerNames = { "Red", "Green", "Yellow", "Blue" };
        static ConsoleColor[] playerColors = { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Blue };

        // Proper start squares mapped onto the revised cross track path
        static int[] startSquares = { 1, 11, 21, 31 };
        static Random dice = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine("=== WELCOME TO MINI LUDO ===");
            Console.WriteLine("Press any key to start...\n");
            Console.ReadKey();

            var game = new Game();
            game.Run();
        }

        static void DrawBoard()
        {
            Console.WriteLine("Home:");
            for (int p = 0; p < playerNames.Length; p++)
            {
                Console.ForegroundColor = playerColors[p];
                string status = playerPositions[p] == 0 ? "(Home)" : $"(Pos {playerPositions[p]})";
                Console.WriteLine($" {playerNames[p]} {status}");
            }
            Console.ResetColor();

            // FIX: Explicitly map the 40 perimeter steps tracking clockwise around the classic cross
            var coords = new System.Collections.Generic.List<Tuple<int, int>>
            {
                // Left arm (going right)
                Tuple.Create(1,5), Tuple.Create(2,5), Tuple.Create(3,5), Tuple.Create(4,5),
                // Top arm (going up then down)
                Tuple.Create(5,4), Tuple.Create(5,3), Tuple.Create(5,2), Tuple.Create(5,1), Tuple.Create(5,0),
                Tuple.Create(6,0), Tuple.Create(6,1), Tuple.Create(6,2), Tuple.Create(6,3), Tuple.Create(6,4),
                // Right arm (going right then left)
                Tuple.Create(7,5), Tuple.Create(8,5), Tuple.Create(9,5), Tuple.Create(10,5),
                Tuple.Create(10,6), Tuple.Create(9,6), Tuple.Create(8,6), Tuple.Create(7,6),
                // Bottom arm (going down then up)
                Tuple.Create(6,7), Tuple.Create(6,8), Tuple.Create(6,9), Tuple.Create(6,10),
                Tuple.Create(5,10), Tuple.Create(5,9), Tuple.Create(5,8), Tuple.Create(5,7),
                // Left arm wrapping back to start
                Tuple.Create(4,6), Tuple.Create(3,6), Tuple.Create(2,6), Tuple.Create(1,6),
                Tuple.Create(0,6), Tuple.Create(0,5)
            };

            var trackMap = new System.Collections.Generic.Dictionary<int, Tuple<int, int>>();
            for (int i = 0; i < coords.Count; i++) trackMap[i + 1] = coords[i];
            trackMap[GOAL] = Tuple.Create(5, 5); // Center block

            var gridIndex = new int[11, 11];
            for (int yy = 0; yy < 11; yy++) for (int xx = 0; xx < 11; xx++) gridIndex[xx, yy] = -1;
            foreach (var kv in trackMap) gridIndex[kv.Value.Item1, kv.Value.Item2] = kv.Key;

            bool IsTopLeftCorner(int x, int y) => x <= 4 && y <= 4;
            bool IsTopRightCorner(int x, int y) => x >= 6 && y <= 4;
            bool IsBottomLeftCorner(int x, int y) => x <= 4 && y >= 6;
            bool IsBottomRightCorner(int x, int y) => x >= 6 && y >= 6;

            Console.WriteLine("\nBoard:");
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    int idx = gridIndex[x, y];
                    var playersHere = Enumerable.Range(0, playerPositions.Length).Where(p => playerPositions[p] != 0 && playerPositions[p] == idx).ToArray();

                    if (IsTopLeftCorner(x, y))
                    {
                        Console.Write("["); Console.BackgroundColor = ConsoleColor.Red;
                        bool isCircle = (x % 5 == 1 || x % 5 == 3) && (y % 5 == 1 || y % 5 == 3);
                        if (isCircle) { Console.ForegroundColor = ConsoleColor.White; Console.Write(" o "); }
                        else Console.Write("   ");
                        Console.ResetColor(); Console.Write("]"); continue;
                    }
                    if (IsTopRightCorner(x, y))
                    {
                        Console.Write("["); Console.BackgroundColor = ConsoleColor.Green;
                        bool isCircle = ((x - 6) % 5 == 1 || (x - 6) % 5 == 3) && (y % 5 == 1 || y % 5 == 3);
                        if (isCircle) { Console.ForegroundColor = ConsoleColor.White; Console.Write(" o "); }
                        else Console.Write("   ");
                        Console.ResetColor(); Console.Write("]"); continue;
                    }
                    if (IsBottomLeftCorner(x, y))
                    {
                        Console.Write("["); Console.BackgroundColor = ConsoleColor.Blue;
                        bool isCircle = (x % 5 == 1 || x % 5 == 3) && ((y - 6) % 5 == 1 || (y - 6) % 5 == 3);
                        if (isCircle) { Console.ForegroundColor = ConsoleColor.White; Console.Write(" o "); }
                        else Console.Write("   ");
                        Console.ResetColor(); Console.Write("]"); continue;
                    }
                    if (IsBottomRightCorner(x, y))
                    {
                        Console.Write("["); Console.BackgroundColor = ConsoleColor.Yellow;
                        bool isCircle = ((x - 6) % 5 == 1 || (x - 6) % 5 == 3) && ((y - 6) % 5 == 1 || (y - 6) % 5 == 3);
                        if (isCircle) { Console.ForegroundColor = ConsoleColor.Black; Console.Write(" o "); }
                        else Console.Write("   ");
                        Console.ResetColor(); Console.Write("]"); continue;
                    }

                    if (x == 5 && y == 5)
                    {
                        Console.Write("["); Console.BackgroundColor = ConsoleColor.White; Console.ForegroundColor = ConsoleColor.Black; Console.Write(" + "); Console.ResetColor(); Console.Write("]"); continue;
                    }

                    // Colored straight entry paths leading to the goal
                    if (y == 5 && x >= 1 && x <= 4) { Console.Write("["); Console.BackgroundColor = ConsoleColor.Red; Console.Write("   "); Console.ResetColor(); Console.Write("]"); continue; }
                    if (y == 5 && x >= 6 && x <= 9) { Console.Write("["); Console.BackgroundColor = ConsoleColor.Yellow; Console.Write("   "); Console.ResetColor(); Console.Write("]"); continue; }
                    if (x == 5 && y >= 1 && y <= 4) { Console.Write("["); Console.BackgroundColor = ConsoleColor.Green; Console.Write("   "); Console.ResetColor(); Console.Write("]"); continue; }
                    if (x == 5 && y >= 6 && y <= 9) { Console.Write("["); Console.BackgroundColor = ConsoleColor.Blue; Console.Write("   "); Console.ResetColor(); Console.Write("]"); continue; }

                    if (idx != -1)
                    {
                        if (playersHere.Length > 0)
                        {
                            if (playersHere.Length == 1)
                            {
                                int p = playersHere[0];
                                Console.Write("["); Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = playerColors[p]; Console.Write($" {playerNames[p][0]} "); Console.ResetColor(); Console.Write("]");
                            }
                            else
                            {
                                Console.Write("["); Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = ConsoleColor.White;
                                string s = string.Join("&", playersHere.Select(i => playerNames[i][0]));
                                s = s.PadLeft(2).PadRight(3);
                                Console.Write(s); Console.ResetColor(); Console.Write("]");
                            }
                        }
                        else if (startSquares.Contains(idx))
                        {
                            int owner = Array.IndexOf(startSquares, idx);
                            Console.Write("["); Console.BackgroundColor = playerColors[owner]; Console.ForegroundColor = (owner == 2) ? ConsoleColor.Black : ConsoleColor.White; Console.Write($" S "); Console.ResetColor(); Console.Write("]");
                        }
                        else
                        {
                            int nextIdx = idx % 36 + 1; // Corrected wrap limit matching the new cross tracking path count
                            var cur = trackMap[idx];
                            var nxt = trackMap[nextIdx];
                            int dx = nxt.Item1 - cur.Item1;
                            int dy = nxt.Item2 - cur.Item2;
                            char arrow = '·';
                            if (dx == 1) arrow = '→';
                            else if (dx == -1) arrow = '←';
                            else if (dy == 1) arrow = '↓';
                            else if (dy == -1) arrow = '↑';

                            Console.Write($"[ {arrow} ]");
                        }
                        continue;
                    }
                    Console.Write("[   ]");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}