    
using System;
using System.Threading;

namespace SimpleLudo
{
    class Program
    {
        // Define the goal position. A short 30-step track keeps the game fast.
        const int GOAL = 30;
        static int[] playerPositions = { 0, 0 }; // Player 1 (Red) and Player 2 (Green)
        static string[] playerNames = { "Red", "Green" };
        static Random dice = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("=== WELCOME TO MINI LUDO ===");
            Console.WriteLine($"First player to reach space {GOAL} wins!");
            Console.WriteLine("Press any key to start...\n");
            Console.ReadKey();

            int currentPlayer = 0;
            bool gameWon = false;

            while (!gameWon)
            {
                Console.Clear();
                DrawBoard();

                Console.WriteLine($"\nIt's Player {playerNames[currentPlayer]}'s turn!");
                Console.WriteLine("Press Enter to roll the dice...");
                Console.ReadLine();

                int roll = dice.Next(1, 7);
                Console.WriteLine($"You rolled a {roll}!");

                // Logic to move the player
                if (playerPositions[currentPlayer] + roll <= GOAL)
                {
                    playerPositions[currentPlayer] += roll;
                    Console.WriteLine($"{playerNames[currentPlayer]} moves to space {playerPositions[currentPlayer]}.");

                    // Simple capturing mechanic: if you land on the opponent, send them back to 0!
                    int opponent = (currentPlayer == 0) ? 1 : 0;
                    if (playerPositions[currentPlayer] == playerPositions[opponent] && playerPositions[currentPlayer] != 0)
                    {
                        playerPositions[opponent] = 0;
                        Console.WriteLine($"💥 BOOM! {playerNames[currentPlayer]} captured {playerNames[opponent]} and sent them back to the start!");
                    }
                }
                else
                {
                    Console.WriteLine($"Rolled too high! {playerNames[currentPlayer]} needs an exact roll to hit {GOAL}.");
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

                Thread.Sleep(2000); // Pause for 2 seconds so players can read the result

                // Switch turns
                currentPlayer = (currentPlayer == 0) ? 1 : 0;
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Draws a visual text-based track representing the game board
        static void DrawBoard()
        {
            Console.WriteLine("Board Track:");
            for (int i = 0; i <= GOAL; i++)
            {
                if (playerPositions[0] == i && playerPositions[1] == i && i != 0)
                {
                    Console.Write("[R&G]"); // Both players on the same spot
                }
                else if (playerPositions[0] == i && i != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[ R ]");
                    Console.ResetColor();
                }
                else if (playerPositions[1] == i && i != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[ G ]");
                    Console.ResetColor();
                }
                else if (i == 0)
                {
                    Console.Write("[Start]");
                }
                else if (i == GOAL)
                {
                    Console.Write("[GOAL]");
                }
                else
                {
                    Console.Write($"[ {i.ToString("D2")} ]");
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