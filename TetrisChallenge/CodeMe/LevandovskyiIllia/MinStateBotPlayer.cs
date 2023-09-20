using System;

namespace TetrisChallenge
{
    /// <summary>
    /// naive solution
    /// imagine a piece is put onto the board what would be the metric for a given rotation and offset?
    ///
    /// thus everything is about how to calculate the metric
    /// 1. min weight of the system (pieces should be as low as possible)
    /// 2. add penalty for covered holes
    /// 3. add penalty for non-adjacent piece locations
    ///
    /// penalties are taken into the consideration for 4 rows in depth at most
    /// not to prevent new pieces being stacked upon each other
    ///
    /// holes penalty is multiplied by a magic number 2.1 (for a single play) or 3.1 (for a tournament)
    /// adjacent penalty is divided by a magic number - the width of the board
    /// (to show it's much less important than the holes penalty)
    ///
    /// current solution is not ideal, is quite volatile (best result so far is 615 score; average ~394 per run)
    /// possible to have 459 per run in a tournament
    /// </summary>
    public class MinStateBotPlayer : IPlayer
    {
        //private const double HolePenaltyFactor = 2.1;//for single play
        private const double HolePenaltyFactor = 3.1;//for tournament

        //private const int HolePenaltyDepth = 4;//for single play; //as [][][][] vert is 4 rows
        private const int HolePenaltyDepth = 30;//for tournament

        public void Init() { }

        public Command Step(StateSnapshot snapshot)
        {
            double targetMetric = double.MaxValue;
            int targetRotation = 0;
            int targetOffset = 0;

            for (int rotation = 0; rotation < snapshot.piece.rotations; rotation++)
            {
                var piece = snapshot.piece.Rotate(rotation);
                for (int offset = 0; offset < GameState.Width - piece.width + 1; offset++)
                {
                    var layout = piece.GetLayout(offset);
                    var metric = CalculateMetric(snapshot.board, layout);
                    if (metric < targetMetric)
                    {
                        targetMetric = metric;
                        targetOffset = offset;
                        targetRotation = rotation;
                    }
                }
            }

            ConsoleRenderer.Render(targetOffset, targetRotation, snapshot);
            return new Command(targetOffset, targetRotation);
        }

        private double CalculateMetric(bool[,] board, int[] layout)
        {
            var metric = 0.0;

            var state = new VirtualGameState(board);
            var filledInLines = state.PutPiece(layout);
            if (filledInLines < 0) return double.MaxValue;

            metric += GetWeightMetric(state);

            var currentHeight = GetCurrentHeight(state);

            metric += GetHoleMetric(state, currentHeight);
            metric += GetAdjacentMetric(state, currentHeight);

            //as [][][][] vertically can occupy 4 rows
            //also indicates it's extremely important to remove row now....
            return metric - filledInLines * Math.Pow(GameState.Width, 4);
        }

        private int GetCurrentHeight(VirtualGameState state)
        {
            for (int row = 0; row < GameState.Height; row++)
                for (int column = 0; column < GameState.Width; column++)
                    if (state.Board[column + row * GameState.Width])
                        return row;

            return GameState.Height;
        }

        private static double GetWeightMetric(VirtualGameState state)
        {
            var totalWeight = 0.0;
            for (var i = 0; i < state.Board.Length; i++)
                if (state.Board[i])
                {
                    //fractional part loss is intentional
                    totalWeight +=
                        ((GameState.Height * GameState.Width - i) / GameState.Width) * GameState.Width;
                }

            return totalWeight;
        }

        private double GetHoleMetric(VirtualGameState state, int currentHeight)
        {
            var totalHoleMetric = 0.0;
            var rowLimit = Math.Min(GameState.Height, currentHeight + HolePenaltyDepth);

            for (int column = 0; column < GameState.Width; column++)
            {
                var coveredOnRow = -1;
                for (int row = currentHeight; row < rowLimit; row++)
                {
                    var i = column + row * GameState.Width;
                    if (coveredOnRow < 0 && state.Board[i])
                    {
                        coveredOnRow = row;
                        continue;
                    }

                    if (coveredOnRow >= 0 && !state.Board[i])
                    {
                        //different approaches are taken into the consideration, this one happens to be the best
                        totalHoleMetric += row;
                    }
                }
            }

            return totalHoleMetric * HolePenaltyFactor;
        }

        private double GetAdjacentMetric(VirtualGameState state, int currentHeight)
        {
            var totalAdjacentMetric = 0.0;
            var rowLimit = Math.Min(GameState.Height, currentHeight + 4);//as [][][][] vert is 4 rows

            for (int row = currentHeight; row < rowLimit; row++)
            {
                var intervalsCount = 1;
                var cell = state.Board[0 + row * GameState.Width];
                for (int column = 1; column < GameState.Width; column++)
                {
                    var i = column + row * GameState.Width;
                    if (cell != state.Board[i])
                    {
                        intervalsCount++;
                        cell = state.Board[i];
                    }
                }
                totalAdjacentMetric += intervalsCount;
            }

            //divide to express is much less important than the hole penalty
            return totalAdjacentMetric / GameState.Width;
        }
    }
}