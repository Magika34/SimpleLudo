    
using System;
using System.Linq;
using System.Threading;

namespace SimpleLudo
{
    class Program
    {
        // Simplified Ludo-style track
        const int GOAL = 40; // finish line index
        // 0 means at Home, 1..GOAL are track positions, GOAL is the final safe goal
        static int[] playerPositions = { 0, 0, 0, 0 };
        static string[] playerNames = { "Red", "Green", "Yellow", "Blue" };
        static ConsoleColor[] playerColors = { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Blue };
        // Each player's entry (start) square on the track
        static int[] startSquares = { 1, 11, 21, 31 };
        static Random dice = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("=== WELCOME TO MINI LUDO ===");
            Console.WriteLine($"First player to reach space {GOAL} wins!");
            Console.WriteLine("Rules: roll a 6 to enter from Home, roll a 6 to get an extra turn. Capture sends a token back to Home.");
            Console.WriteLine("Press any key to start...\n");
            Console.ReadKey();

            int currentPlayer = 0;
            bool gameWon = false;

            while (!gameWon)
            {
                Console.Clear();
                DrawBoard();

                Console.WriteLine($"\nIt's {playerNames[currentPlayer]}'s turn! (Press Enter to roll the dice)");
                Console.ReadLine();

                int roll = dice.Next(1, 7);
                Console.WriteLine($"You rolled a {roll}!");

                bool moved = false;

                // If at Home (0), need a 6 to enter
                if (playerPositions[currentPlayer] == 0)
                {
                    if (roll == 6)
                    {
                        playerPositions[currentPlayer] = startSquares[currentPlayer];
                        Console.WriteLine($"{playerNames[currentPlayer]} enters the board at {playerPositions[currentPlayer]}.");
                        moved = true;
                    }
                    else
                    {
                        Console.WriteLine($"{playerNames[currentPlayer]} is at Home and needs a 6 to enter.");
                    }
                }
                else
                {
                    // Move along the linear track and require exact roll to reach GOAL
                    int target = playerPositions[currentPlayer] + roll;
                    if (target <= GOAL)
                    {
                        playerPositions[currentPlayer] = target;
                        Console.WriteLine($"{playerNames[currentPlayer]} moves to space {playerPositions[currentPlayer]}.");
                        moved = true;
                    }
                    else
                    {
                        Console.WriteLine($"Rolled too high! {playerNames[currentPlayer]} needs an exact roll to hit {GOAL}.");
                    }
                }

                // Capture mechanic: if you land on an opponent on the track (not Home and not GOAL), send them to Home
                if (moved && playerPositions[currentPlayer] != 0 && playerPositions[currentPlayer] != GOAL)
                {
                    for (int i = 0; i < playerPositions.Length; i++)
                    {
                        if (i == currentPlayer) continue;
                        if (playerPositions[i] == playerPositions[currentPlayer])
                        {
                            playerPositions[i] = 0;
                            Console.WriteLine($"💥 BOOM! {playerNames[currentPlayer]} captured {playerNames[i]} and sent them back to Home!");
                        }
                    }
                }

                // Check for win condition
                if (playerPositions[currentPlayer] == GOAL)
                {
                    Console.Clear();
                    DrawBoard();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n🎉🎉🎉 PLAYER {playerNames[currentPlayer].ToUpper()} WINS THE GAME! 🎉🎉🎉");
                    Console.ResetColor();
                    gameWon = true;
                    break;
                }

                // If roll was 6, current player gets another turn
                if (roll == 6)
                {
                    Console.WriteLine($"{playerNames[currentPlayer]} rolled a 6 and gets another turn!");
                    Thread.Sleep(1500);
                    continue; // do not switch player
                }

                Thread.Sleep(1200); // Pause so players can read the result

                // Switch turns
                currentPlayer = (currentPlayer + 1) % playerPositions.Length;
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Draws a visual text-based track representing the game board
        static void DrawBoard()
        {
            // Show which players are at Home
            Console.WriteLine("Home:");
            for (int p = 0; p < playerNames.Length; p++)
            {
                Console.ForegroundColor = playerColors[p];
                string status = playerPositions[p] == 0 ? "(Home)" : $"(Pos {playerPositions[p]})";
                Console.WriteLine($" {playerNames[p]} {status}");
            }
            Console.ResetColor();

            Console.WriteLine("\nBoard Track:");
            for (int i = 0; i <= GOAL; i++)
            {
                // collect players at this index
                var playersHere = Enumerable.Range(0, playerPositions.Length).Where(idx => playerPositions[idx] == i).ToArray();

                if (playersHere.Length > 0 && i != 0)
                {
                    if (playersHere.Length == 1)
                    {
                        int p = playersHere[0];
                        Console.ForegroundColor = playerColors[p];
                        Console.Write($"[{playerNames[p][0]}]");
                        Console.ResetColor();
                    }
                    else
                    {
                        // multiple players on same spot
                        Console.Write("[");
                        for (int k = 0; k < playersHere.Length; k++)
                        {
                            Console.Write(playerNames[playersHere[k]][0]);
                            if (k < playersHere.Length - 1) Console.Write("&");
                        }
                        Console.Write("]");
                    }
                }
                else if (i == 0)
                {
                    Console.Write("[Home]");
                }
                else if (startSquares.Contains(i))
                {
                    // mark start squares with the first letter of the owner
                    int owner = Array.IndexOf(startSquares, i);
                    Console.ForegroundColor = playerColors[owner];
                    Console.Write($"[S{playerNames[owner][0]}]");
                    Console.ResetColor();
                }
                else if (i == GOAL)
                {
                    Console.Write("[GOAL]");
                }
                else
                {
                    Console.Write($"[{i.ToString("D2")}]");
                }

                // Break lines every 10 spaces to make it look like a grid board
                if ((i + 1) % 10 == 0)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine("\n");
        }
    }
}
