using System;

namespace SimpleLudo
{
    public enum PawnState { Home, OnTrack, Finished }

    public class Pawn
    {
        public Player Owner { get; }
        public int Index { get; set; } // 0 = Home, 1..40 perimeter, 41 = Goal
        public PawnState State { get; set; }

        public Pawn(Player owner)
        {
            Owner = owner;
            Index = 0;
            State = PawnState.Home;
        }

        public bool IsAtHome() => State == PawnState.Home;
        public bool IsOnTrack() => State == PawnState.OnTrack;
        public bool IsFinished() => State == PawnState.Finished;

        public void Enter(int startIndex)
        {
            Index = startIndex;
            State = PawnState.OnTrack;
        }

        public void MoveTo(int newIndex, int goalIndex)
        {
            Index = newIndex;
            if (Index == goalIndex) State = PawnState.Finished;
            else State = PawnState.OnTrack;
        }

        public void SendHome()
        {
            Index = 0;
            State = PawnState.Home;
        }
    }
}
