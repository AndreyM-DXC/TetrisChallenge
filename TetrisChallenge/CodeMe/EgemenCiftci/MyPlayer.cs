using System;
using TetrisChallenge;

public class MyPlayer : IPlayer
{
    public int aggregateHeightWeight = 89;
    public int clearedLinesWeight = 92;
    public int holesWeight = 89;
    public int bumpinessWeight = 40;

    public void Init() { }

    public Command Step(StateSnapshot snapshot)
    {
        GamePiece currentPiece = snapshot.piece;
        int bestObjValue = int.MinValue;
        Command bestMove = new Command(0, 0);

        int width = snapshot.board.GetLength(0);
        int height = snapshot.board.GetLength(1);

        for (int rotationCount = 0; rotationCount < currentPiece.rotations; rotationCount++)
        {
            GamePiece rotatedPiece = currentPiece.Rotate(rotationCount);

            for (int offset = 0; offset < width - rotatedPiece.width + 1; offset++)
            {
                int[] piecelayout = rotatedPiece.GetLayout(offset);
                bool[,] newBoard = PlacePiece(snapshot.board, piecelayout, width, height);

                int objValue = GetObjectiveValue(newBoard, width, height);

                if (objValue > bestObjValue)
                {
                    bestObjValue = objValue;
                    bestMove = new Command(offset, rotationCount);
                }
            }
        }

        ConsoleRenderer.Render(bestMove.offset, bestMove.rotation, snapshot, 100);

        return bestMove;
    }

    private int GetObjectiveValue(bool[,] board, int width, int height)
    {
        int[] heights = GetHeights(board, width, height);

        return (GetClearedLines(board, width, height) * clearedLinesWeight) -
               (GetHoles(board, width, height) * holesWeight) -
               (GetAggregateHeight(heights) * aggregateHeightWeight) -
               (GetBumpiness(heights) * bumpinessWeight);
    }

    private static bool[,] PlacePiece(bool[,] board, int[] pieceLayout, int width, int height)
    {
        for (int h = 0; h < height; h++)
        {
            foreach (int index in pieceLayout)
            {
                int x = index % width;
                int y = (index / width) + h;

                if (y >= height || board[x, y])
                {
                    h--;
                    return Apply(board, pieceLayout, width, h);
                }
            }
        }

        return Apply(board, pieceLayout, width, 0);
    }

    private static bool[,] Apply(bool[,] board, int[] pieceLayout, int width, int h)
    {
        bool[,] newBoard = (bool[,])board.Clone();

        foreach (int index in pieceLayout)
        {
            int x = index % width;
            int y = (index / width) + h;

            newBoard[x, y] = true;
        }

        return newBoard;
    }

    /// <summary>
    /// Holes are empty spaces beneath filled cells. 
    /// More holes can restrict movement and create problems. 
    /// We can assign a negative weight to the number of holes.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private static int GetHoles(bool[,] board, int width, int height)
    {
        int count = 0;

        for (int x = 0; x < width; x++)
        {
            bool isUnder = false;

            for (int y = 0; y < height; y++)
            {
                if (board[x, y])
                {
                    isUnder = true;
                }
                else
                {
                    if (isUnder)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Higher columns can lead to more difficult situations later. 
    /// Weights for heights could decrease as columns get higher.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private static int GetAggregateHeight(int[] heights)
    {
        int sum = 0;

        for (int i = 0; i < heights.Length; i++)
        {
            sum += heights[i];
        }

        return sum;
    }

    private static int[] GetHeights(bool[,] board, int width, int height)
    {
        int[] heights = new int[width];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y])
                {
                    heights[x] = height - y;
                    break;
                }
            }
        }

        return heights;
    }

    /// <summary>
    /// This measures the difference in column heights. 
    /// A smoother playing field is generally preferable. 
    /// We can assign a negative weight to the sum of differences between adjacent column heights.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private static int GetBumpiness(int[] heights)
    {
        int sumOfDeltas = 0;

        for (int h = 0; h < heights.Length - 1; h++)
        {
            sumOfDeltas += Math.Abs(heights[h] - heights[h + 1]);
        }

        return sumOfDeltas;
    }

    /// <summary>
    /// Clearing lines is essential. 
    /// We can assign a positive weight to the number of lines cleared in a move.
    /// </summary>
    /// <param name="board"></param>
    /// <returns>0, 1, 2, 3, 4</returns>
    private static int GetClearedLines(bool[,] board, int width, int height)
    {
        int clearedLines = 0;

        for (int y = 0; y < height; y++)
        {
            clearedLines++;

            for (int x = 0; x < width; x++)
            {
                if (!board[x, y])
                {
                    clearedLines--;
                    break;
                }
            }
        }
        switch (clearedLines)
        {
            case 0: return 0;
            case 1: return 1;
            case 2: return 3;
            case 3: return 6;
            case 4: return 10;
            default: return clearedLines;
        }
    }
}
