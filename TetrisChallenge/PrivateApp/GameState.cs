using System;
using System.Linq;

namespace TetrisChallenge
{
    public class GameState
    {
        private readonly bool[] board = new bool[Width * Height];
        private readonly GamePiece[] pieces;
        private readonly Random random;

        public const int Width = 10;
        public const int Height = 20;

        public GamePiece Piece { get; private set; }
        public int Score { get; private set; }

        public GameState(int seed)
        {
            pieces = GamePiece.All.Where(p => p.Key.EndsWith("-0")).Select(p => p.Value).ToArray();
            random = new Random(seed);
            Piece = pieces[random.Next(pieces.Length)];
        }

        public void CopyTo(StateSnapshot snapshot)
        {
            snapshot.score = Score;
            snapshot.piece = Piece;
            
            for (int y = 0, i = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++, i++)
                {
                    snapshot.board[x, y] = board[i];
                }
            }
        }

        public void Apply(Command command)
        {
            var layout = Piece.Rotate(command.rotation).GetLayout(command.offset);

            if (!MoveToNextLine(layout))
            {
                throw new Exception("Game Over!");
            }
            while (MoveToNextLine(layout))
            {
            }
            foreach (var index in layout)
            {
                board[index - Width] = true;
            }
            var score = 1;
            for (var i = board.Length - Width; i >= 0; i -= Width)
            {
                if (IsFilled(i))
                {
                    Score += score++;
                    Array.Copy(board, 0, board, Width, i);
                    Array.Clear(board, 0, Width);
                    i += Width;
                }
            }
            Score++;
            Piece = pieces[random.Next(pieces.Length)];
        }

        private bool IsFilled(int offset)
        {
            for (var i = 0; i < Width; i++)
            {
                if (!board[i + offset])
                {
                    return false;
                }
            }
            return true;
        }

        private bool MoveToNextLine(int[] layout)
        {
            foreach (var index in layout)
            {
                if (index >= board.Length || board[index])
                {
                    return false;
                }
            }
            for (var i = 0; i < layout.Length; i++)
            {
                layout[i] += Width;
            }
            return true;
        }
    }
}
