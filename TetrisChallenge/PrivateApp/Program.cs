using System;

namespace TetrisChallenge
{
    public static class Program
    {
        public static void Main()
        {
            // Here your player is initialized
            var player = new RandomPlayer();

            // Try to play by your own!
            // var player = new ConsolePlayer();

            RunSingleGame(player);
            // RunTournament(player);
        }

        private static void RunSingleGame(IPlayer player)
        {
            var score = Run(0, 100000, player);
            Console.WriteLine($"Final Score: {score}");
            Console.ReadKey();
        }

        private static void RunTournament(IPlayer player)
        {
            // Here will be a set of random seeds, right now this is a test placeholder
            var rounds = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            if (player as ConsolePlayer == null)
            {
                ConsoleRenderer.Render = ConsoleRenderer.NullRender;
            }
            var score = 0;
            foreach (var round in rounds)
            {
                score += Run(round, 100000, player);
            }
            Console.WriteLine($"Final Score: {score}");
            Console.ReadKey();
        }

        private static int Run(int seed, int steps, IPlayer player)
        {
            var state = new GameState(seed);
            var snapshot = new StateSnapshot();

            try
            {
                player.Init();
                for (var i = 0; i < steps; i++)
                {
                    state.CopyTo(snapshot);
                    state.Apply(player.Step(snapshot));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"End of Game: {ex.Message}");
            }

            return state.Score;
        }
    }
}
