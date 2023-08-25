using System;
using System.Threading;

namespace TetrisChallenge
{
    public static class ConsoleRenderer
    {
        public delegate void Delegate(int offset, int rotation, StateSnapshot snapshot, int delay = 1000);

        public static Delegate Render { get; set; } = ConsoleRender;

        public static void ConsoleRender(int offset, int rotation, StateSnapshot snapshot, int delay = 1000)
        {
            var piece = snapshot.piece.Rotate(rotation);

            Console.Clear();

            var layout = piece.GetLayout(offset);
            foreach (var value in layout)
            {
                var x = value % GameState.Width;
                var y = value / GameState.Width;
                Console.SetCursorPosition(x * 2 + 1, y + 1);
                Console.Write("[]");
            }

            Console.SetCursorPosition(0, 5);

            for (var i = 0; i < GameState.Height; i++)
            {
                Console.Write('#');
                for (var j = 0; j < GameState.Width; j++)
                {
                    Console.Write(snapshot.board[j, i] ? "[]" : " .");
                }
                Console.WriteLine('#');
            }
            Console.WriteLine(new string('#', GameState.Width * 2 + 2));
            Console.WriteLine($"Piece: {piece} Score: {snapshot.score}");
            Console.WriteLine($"Offset: {offset} Rotation: {rotation}");

            if (delay > 0)
            {
                Thread.Sleep(delay);
            }
        }

        public static void NullRender(int offset, int rotation, StateSnapshot snapshot, int delay = 1000)
        {
        }
    }
}
