using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleLudo
{
    public class Game
    {
        public LudoBoard Board { get; }
        public List<Player> Players { get; }
        Random dice = new Random();
        int currentPlayer = 0;

        public Game()
        {
            Board = new LudoBoard();
            Players = new List<Player>
            {
                new Player("Red", ConsoleColor.Red, Board.StartIndices[0]),
                new Player("Green", ConsoleColor.Green, Board.StartIndices[1]),
                new Player("Yellow", ConsoleColor.Yellow, Board.StartIndices[2]),
                new Player("Blue", ConsoleColor.Blue, Board.StartIndices[3])
            };
        }

        public void Run()
        {
            bool gameWon = false;
            while (!gameWon)
            {
                Console.Clear();
                Board.Draw(Players);

                var player = Players[currentPlayer];
                Console.WriteLine($"\nIt's {player.Name}'s turn. Press Enter to roll the dice...");
                Console.ReadLine();
                int roll = dice.Next(1, 7);
                Console.WriteLine($"Rolled: {roll}");

                bool moved = TryMove(player, roll);

                // Check for win
                if (player.AllFinished())
                {
                    Console.WriteLine($"\n{player.Name} wins!");
                    gameWon = true;
                    break;
                }

                if (roll == 6)
                {
                    Console.WriteLine($"{player.Name} rolled a 6 and gets another turn.");
                    Thread.Sleep(1000);
                    continue; // same player
                }

                Thread.Sleep(800);
                currentPlayer = (currentPlayer + 1) % Players.Count;
            }

            Console.WriteLine("\nGame over. Press any key to exit...");
            Console.ReadKey();
        }

        bool TryMove(Player player, int roll)
        {
            // priority: move pawn on track if possible (exact to goal), otherwise enter from home on 6
            // find a pawn on track that can move
            Pawn candidate = null;
            int candidateTarget = -1;

            foreach (var p in player.Pawns.Where(p => p.IsOnTrack()))
            {
                int target = p.Index + roll;
                if (target > Board.GoalIndex) continue; // needs exact
                candidate = p; candidateTarget = target; break;
            }

            if (candidate == null && roll == 6)
            {
                // enter a pawn from home if available
                var homePawn = player.Pawns.FirstOrDefault(p => p.IsAtHome());
                if (homePawn != null)
                {
                    homePawn.Enter(player.StartIndex);
                    Console.WriteLine($"{player.Name} enters a pawn at {player.StartIndex}.");
                    // capture any opponent here
                    CaptureAt(player.StartIndex, player);
                    return true;
                }
            }

            if (candidate != null)
            {
                candidate.MoveTo(candidateTarget, Board.GoalIndex);
                Console.WriteLine($"{player.Name} moves pawn to {candidateTarget}.");
                if (candidate.IsOnTrack()) CaptureAt(candidate.Index, player);
                return true;
            }

            Console.WriteLine("No valid move.");
            return false;
        }

        void CaptureAt(int index, Player mover)
        {
            if (index <= 0 || index == Board.GoalIndex) return;
            foreach (var pl in Players.Where(p => p != mover))
            {
                foreach (var pa in pl.Pawns)
                {
                    if (pa.IsOnTrack() && pa.Index == index)
                    {
                        pa.SendHome();
                        Console.WriteLine($"BOOM! {mover.Name} captured {pl.Name}'s pawn.");
                    }
                }
            }
        }
    }
}
