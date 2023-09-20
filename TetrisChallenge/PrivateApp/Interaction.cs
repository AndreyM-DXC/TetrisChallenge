using System;
using System.Text;

namespace TetrisChallenge
{
    public interface IPlayer
    {
        /// <summary> Initialization before each next game </summary>
        void Init();
        Command Step(StateSnapshot snapshot);
    }

    public class StateSnapshot
    {
        /// <summary> Current board state, indexing starts from the top left corner </summary>
        public readonly bool[,] board = new bool[GameState.Width, GameState.Height];
        /// <summary> Current Piece to place</summary>
        public GamePiece piece;
        /// <summary> Current Score </summary>
        public int score;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Piece: {piece}");
            for (var i = 0; i < GameState.Height; i++)
            {
                sb.Append("#");
                for (var j = 0; j < GameState.Width; j++)
                {
                    sb.Append(board[j, i] ? "[]" : " .");
                }
                sb.AppendLine("# " + i);
            }
            sb.AppendLine(new string('#', GameState.Width * 2 + 2));
            return sb.ToString();
        }

        public void Parse(string text)
        {
            var i = 0;
            foreach (var ch in text)
            {
                if (ch ==  '.' || ch == ']')
                {
                    var x = i % GameState.Width;
                    var y = i / GameState.Width;
                    board[x, y] = ch == ']';
                    i++;
                }
            }
        }
    }

    public class Command
    {
        /// <summary> Horizontal offset </summary>
        public readonly int offset;
        /// <summary> Index of rotation </summary>
        public readonly int rotation;

        public Command(int offset, int rotation)
        {
            this.offset = offset;
            this.rotation = rotation;
        }

        public override string ToString()
        {
            return $"Off: {offset} Rot: {rotation}";
        }
    }
}
