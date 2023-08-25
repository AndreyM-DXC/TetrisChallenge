using System;

namespace TetrisChallenge
{
    public class ConsolePlayer : IPlayer
    {
        public void Init() { }

        public Command Step(StateSnapshot snapshot)
        {
            var offset = 0;
            var rotation = 0;
            var piece = snapshot.piece;

            while (true)
            {
                ConsoleRenderer.Render(offset, rotation, snapshot, 0);

                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;

                    case ConsoleKey.LeftArrow:
                        offset = Math.Max(offset - 1, 0);
                        break;

                    case ConsoleKey.RightArrow:
                        offset = Math.Min(offset + 1, GameState.Width - piece.width);
                        break;

                    case ConsoleKey.UpArrow:
                        piece = piece.Rotate();
                        rotation = (rotation + 1) % piece.rotations;
                        offset = Math.Min(offset, GameState.Width - piece.width);
                        break;

                    case ConsoleKey.DownArrow:
                        return new Command(offset, rotation);
                }
            }
        }
    }
}
