using System;

namespace TetrisChallenge
{
    public class RandomPlayer : IPlayer
    {
        private readonly Random random = new Random();

        public void Init() { }

        public Command Step(StateSnapshot snapshot)
        {
            var rotation = random.Next(snapshot.piece.rotations);
            var piece = snapshot.piece.Rotate(rotation);
            var offset = random.Next(GameState.Width - piece.width + 1);

            ConsoleRenderer.Render(offset, rotation, snapshot);

            return new Command(offset, rotation);
        }
    }
}
