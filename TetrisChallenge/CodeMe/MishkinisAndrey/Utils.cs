using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisChallenge.CodeMe
{
    public static class Utils
    {
        public static readonly GamePiece[] NewPieces = GamePiece.All.Where(p => p.Key.EndsWith("-0")).Select(p => p.Value).ToArray();

        public static IEnumerable<Command> GetCommands(this StateSnapshot snapshot)
        {
            for (var rotation = 0; rotation < snapshot.piece.rotations; rotation++)
            {
                var piece = snapshot.piece.Rotate(rotation);

                for (var offset = 0; offset <= GameState.Width - piece.width; offset++)
                {
                    yield return new Command(offset, rotation);
                }
            }
        }

        public static StateSnapshot Apply(this StateSnapshot snapshot, Command command)
        {
            var layout = snapshot.piece.Rotate(command.rotation).GetLayout(command.offset);
            var board = new bool[GameState.Width * GameState.Height];
            for (int y = 0, i = 0; y < GameState.Height; y++)
            {
                for (var x = 0; x < GameState.Width; x++, i++)
                {
                    board[i] = snapshot.board[x, y];
                }
            }
            if (!MoveToNextLine(board, layout))
            {
                return null;
            }
            while (MoveToNextLine(board, layout))
            {
            }
            foreach (var index in layout)
            {
                board[index - GameState.Width] = true;
            }
            var score = snapshot.score + 1;
            var nextScore = 1;
            for (var i = board.Length - GameState.Width; i >= 0; i -= GameState.Width)
            {
                if (IsFilled(board, i))
                {
                    score += nextScore++;
                    Array.Copy(board, 0, board, GameState.Width, i);
                    Array.Clear(board, 0, GameState.Width);
                    i += GameState.Width;
                }
            }
            var result = new StateSnapshot { score = score };
            for (int y = 0, i = 0; y < GameState.Height; y++)
            {
                for (var x = 0; x < GameState.Width; x++, i++)
                {
                    result.board[x, y] = board[i];
                }
            }
            return result;
        }

        private static bool MoveToNextLine(bool[] board, int[] layout)
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
                layout[i] += GameState.Width;
            }
            return true;
        }

        private static bool IsFilled(bool[] board, int offset)
        {
            for (var i = 0; i < GameState.Width; i++)
            {
                if (!board[i + offset])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
