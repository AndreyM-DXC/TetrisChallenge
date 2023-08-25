
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
    }
}
