using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisChallenge.CodeMe
{
    public class YetAnotherPlayer : IPlayer
    {
        private readonly List<string> log = new List<string>();
        public string Log => string.Join("\n", log.Select((x, i) => $"Step: {i} {x}").Reverse());

        public void Init() { }

        public Command Step(StateSnapshot snapshot)
        {
            //log.Add(snapshot.ToString());
            var command = StepImpl(snapshot);
            ConsoleRenderer.Render(command.offset, command.rotation, snapshot, 100);
            return command;
        }

        private Command StepImpl(StateSnapshot prevSnapshot)
        {
            var prevPits = GetPits(prevSnapshot);
            var prevHoles = GetHoles(prevSnapshot);

            var actions = prevSnapshot.GetCommands().Select(command =>
            {
                var snapshot = prevSnapshot.Apply(command);
                if (snapshot == null) return null;

                var pits = GetPits(snapshot);
                var holes = GetHoles(snapshot);
                var noise = NoiseLevel(snapshot);

                var holeLevel = holes.Where(x => !prevHoles.Contains(x)).Select(h => h.y).OrderBy(x => x).FirstOrDefault();

                return new
                {
                    command,
                    noise,
                    holes = holes.Count - prevHoles.Count,
                    holeLevel,
                    score = snapshot.score - prevSnapshot.score,
                    pits,
                    snapshot,
                };
            }).Where(x => x != null).ToList();

            // No more moves, surrender
            if (actions.Count == 0)
            {
                return new Command(0, 0);
            }

            // Obvious move
            if (prevSnapshot.piece.type == 'I' && prevPits.Count > 0)
            {
                var pit = prevPits.OrderByDescending(p => p.y).FirstOrDefault();
                var act = actions.FirstOrDefault(x => x.command.offset == pit.x && x.command.rotation == 0);
                if (act != null)
                {
                    return act.command;
                }
            }

            // Best moves
            var act1 = actions.Where(x => x.score > 1 && x.holes <= 0).ToList();
            if (act1.Count > 0)
            {
                act1.Sort((a, b) => D(a.holes, b.holes) ?? D(a.score, b.score) ?? 0);
                return act1[0].command;
            }

            // No new Holes
            var act2 = actions.Where(x => x.holes == 0).ToList();
            if (act2.Count > 0)
            {
                act2.Sort((a, b) => D(a.pits.Count, b.pits.Count) ?? D(a.noise, b.noise) ?? 0);
                return act2[0].command;
            }

            // Too much pits
            if (prevPits.Count > 1)
            {
                var act3 = actions.Where(x => x.pits.Count == prevPits.Count - 1).ToList();
                if (act3.Count > 0)
                {
                    act3.Sort((a, b) => D(a.holes, b.holes) ?? D(b.holeLevel, a.holeLevel) ?? D(a.noise, b.noise) ?? 0);
                    return act3[0].command;
                }
            }

            actions.Sort((a, b) => D(a.holes, b.holes) ?? D(b.holeLevel, a.holeLevel) ?? D(a.noise, b.noise) ?? 0);
            return actions[0].command;
        }

        private static int? D(int a, int b) => a == b ? default(int?) : a - b;

        private List<(int x, int y)> GetHoles(StateSnapshot snapshot)
        {
            var result = new List<(int x, int y)>();
            for (var x = 0; x < GameState.Width; x++)
            {
                var y = 0;
                while (y < GameState.Height)
                {
                    if (snapshot.board[x, y++])
                    {
                        break;
                    }
                }
                while (y < GameState.Height)
                {
                    if (!snapshot.board[x, y++])
                    {
                        result.Add((x, y));
                    }
                }
            }
            return result;
        }

        private int NoiseLevel(StateSnapshot snapshot)
        {
            var noise = 0;
            var prev = GameState.Height;
            for (var y = 0; y < GameState.Height; y++)
            {
                if (snapshot.board[0, y])
                {
                    prev = y;
                    break;
                }
            }
            for (var x = 1; x < GameState.Width; x++)
            {
                var h = GameState.Height;
                for (var y = 0; y < GameState.Height; y++)
                {
                    if (snapshot.board[x, y])
                    {
                        h = y;
                        break;
                    }
                }
                noise += Math.Abs(prev - h);
                prev = h;
            }
            return noise;
        }

        private List<(int x, int y, int height)> GetPits(StateSnapshot snapshot)
        {
            var result = new List<(int x, int y, int height)>();
            for (var x = 0; x < GameState.Width; x++)
            {
                var found = true;
                var y = 0;
                while (y < GameState.Height)
                {
                    var left = x == 0 || snapshot.board[x - 1, y];
                    var right = x == GameState.Width - 1 || snapshot.board[x + 1, y];
                    if (left && right)
                    {
                        break;
                    }
                    if (snapshot.board[x, y])
                    {
                        found = false;
                        break;
                    }
                    y++;
                }
                if (found)
                {
                    var height = 0;
                    while (y < GameState.Height)
                    {
                        if (!snapshot.board[x, y++])
                        {
                            height++;
                        }
                        else break;
                    }
                    if (height >= 3)
                    {
                        result.Add((x, y, height));
                    }
                }
            }
            return result;
        }
    }
}