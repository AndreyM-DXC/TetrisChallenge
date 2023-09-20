using System;

namespace TetrisChallenge
{
    /// <summary>
    /// this class is needed to avoid GameState class changes, while the functionality GameState implements
    /// is needed in another places as well...
    /// ideally the functionality should be extracted as some sort of primitives and consumed in the two places
    /// instead of the code duplication...
    /// </summary>
    public class VirtualGameState
    {
        public bool[] Board { get; } = new bool[GameState.Width * GameState.Height];

        public VirtualGameState(bool[,] board)
        {
            for (int y = 0, i = 0; y < GameState.Height; y++)
                for (var x = 0; x < GameState.Width; x++, i++)
                    Board[i] = board[x, y];
        }

        /// <summary>
        /// returns a premium for filled in lines
        ///     for now could be skipped but could be useful
        /// -1 means given layout is not applicable for a given state
        /// 0 means no lines are filled in with given layout and state
        /// n>0 means n lines have been filled in with given layout and state
        /// </summary>
        public int PutPiece(int[] layout)
        {
            if (!MoveToNextLine(layout))
            {
                return -1;//not applicable
            }
            while (MoveToNextLine(layout))
            {
            }
            foreach (var index in layout)
            {
                Board[index - GameState.Width] = true;
            }

            var filledInLines = 0;
            for (var i = Board.Length - GameState.Width; i >= 0; i -= GameState.Width)
            {
                if (IsFilled(i))
                {
                    filledInLines++;
                    Array.Copy(Board, 0, Board, GameState.Width, i);
                    Array.Clear(Board, 0, GameState.Width);
                    i += GameState.Width;
                }
            }
            return filledInLines;
        }

        private bool IsFilled(int offset)
        {
            for (var i = 0; i < GameState.Width; i++)
                if (!Board[i + offset])
                    return false;

            return true;
        }

        private bool MoveToNextLine(int[] layout)
        {
            foreach (var index in layout)
                if (index >= Board.Length || Board[index])
                    return false;

            for (var i = 0; i < layout.Length; i++)
                layout[i] += GameState.Width;

            return true;
        }

    }
}