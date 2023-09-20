using System;
using System.Collections.Generic;
using System.Linq;
using TetrisChallenge.CodeMe;

namespace TetrisChallenge
{
    public static class Program
    {
        private static readonly int[] LongTournament = Enumerable.Range(0, 1000).ToArray();
        private static readonly int[] FakeTournament = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private static readonly int[] RealTournament = new[] { 
            144918497, -981526650, 291710637, -782581481, 348936210, 968096173, -306087753, 303093805, 158705266, 120636160
        };

        public static void Main()
        {
            Console.BufferHeight = Console.WindowHeight;

            // Andrey Mishkinis
            var player = new YetAnotherPlayer();

            // Illia Levandovskyi
            //var player = new MinStateBotPlayer();

            // Egemen Ciftci
            //var player = new MyPlayer();

            // Try to play by your own!
            // var player = new ConsolePlayer();

            RunSingleGame(player);
            //RunTournament(player, RealTournament);
            Console.ReadKey();
        }

        private static void RunSingleGame(IPlayer player)
        {
            var score = Run(0, 100000, player);
            Console.WriteLine($"Final Score: {score}");
        }

        private static int RunTournament(IPlayer player, int[] rounds)
        {
            if (player as ConsolePlayer == null)
            {
                ConsoleRenderer.Render = ConsoleRenderer.NullRender;
            }

            var stats = new List<(int round, int score)>();
            var score = 0;
            foreach (var round in rounds)
            {
                var res = Run(round, 100000, player);
                Console.WriteLine($"Score: {res} Round: {round}");
                score += res;

                stats.Add((round, res));
            }

            var min = stats.OrderBy(x => x.score).FirstOrDefault();
            var max = stats.OrderBy(x => x.score).LastOrDefault();

            Console.WriteLine($"Final Score: {score}");
            Console.WriteLine($"Min: {min}");
            Console.WriteLine($"Max: {max}");
            return score;
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
