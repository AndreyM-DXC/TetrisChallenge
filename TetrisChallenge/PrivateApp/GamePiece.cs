using System;
using System.Collections.Generic;

namespace TetrisChallenge
{
    // O-0     I-0     I-1
    // [][]    []      [][][][]
    // [][]    []
    //         []
    //         []

    // T-0     T-1     T-2     T-3
    // [][][]  []        []      []
    //   []    [][]    [][][]  [][]
    //         []                []

    // S-0     S-1     Z-0     Z-1
    //   [][]  []      [][]      []
    // [][]    [][]      [][]  [][]
    //           []            []

    // L-0     L-1     L-2     L-3
    // []          []  [][]    [][][]
    // []      [][][]    []    []
    // [][]              []

    // J-0     J-1     J-2     J-3
    //   []    [][][]  [][]    []
    //   []        []  []      [][][]
    // [][]            []

    public class GamePiece
    {
        /// <summary> Collection of all possible Pieces </summary>
        public static readonly Dictionary<string, GamePiece> All = Init();

        /// <summary> Type of Piece: O, I, T, S, Z, L or J</summary>
        public readonly char type;
        /// <summary> Widht of Piece </summary>
        public readonly int width;
        /// <summary> Index of rotation from 0 to rotations-value exclusive </summary>
        public readonly int rotation;
        /// <summary> Count of distinct rotations </summary>
        public readonly int rotations;

        private readonly int[] layout;

        private GamePiece(char type, int rotation, int rotations, int width, int[] layout)
        {
            this.type = type;
            this.rotation = rotation;
            this.rotations = rotations;
            this.width = width;
            this.layout = layout;
        }

        public GamePiece Rotate()
        {
            return All[$"{type}-{(rotation + 1) % rotations}"];
        }

        public GamePiece Rotate(int count)
        {
            var result = this;
            while (count-- > 0)
            {
                result = result.Rotate();
            }
            return result;
        }

        public override string ToString()
        {
            return $"{type}-{rotation}";
        }

        public int[] GetLayout(int offset)
        {
            offset = Math.Max(0, Math.Min(offset, GameState.Width - width));

            var result = new int[layout.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = layout[i] + offset;
            }
            return result;
        }

        private static Dictionary<string, GamePiece> Init()
        {
            const int W = GameState.Width;
            return new Dictionary<string, GamePiece>()
            {
                { "O-0", new GamePiece('O', 0, 1, 2, new[] { W, W + 1, 0, 1 }) },
                { "I-0", new GamePiece('I', 0, 2, 1, new[] { W + W + W, W + W, W, 0 }) },
                { "I-1", new GamePiece('I', 1, 2, 4, new[] { 0, 1, 2, 3 }) },
                { "T-0", new GamePiece('T', 0, 4, 3, new[] { W + 1, 0, 1, 2 }) },
                { "T-1", new GamePiece('T', 1, 4, 2, new[] { W + W, W, W + 1, 0 }) },
                { "T-2", new GamePiece('T', 2, 4, 3, new[] { W, W + 1, W + 2, 1 }) },
                { "T-3", new GamePiece('T', 3, 4, 2, new[] { W + W + 1, W, W + 1, 1 }) },
                { "S-0", new GamePiece('S', 0, 2, 3, new[] { W, W + 1, 1, 2 }) },
                { "S-1", new GamePiece('S', 1, 2, 2, new[] { W + W + 1, W, W + 1, 0 }) },
                { "Z-0", new GamePiece('Z', 0, 2, 3, new[] { W + 1, W + 2, 0, 1 }) },
                { "Z-1", new GamePiece('Z', 1, 2, 2, new[] { W + W, W, W + 1, 1 }) },
                { "L-0", new GamePiece('L', 0, 4, 2, new[] { W + W, W + W + 1, W, 0 }) },
                { "L-1", new GamePiece('L', 1, 4, 3, new[] { W, W + 1, W + 2, 2 }) },
                { "L-2", new GamePiece('L', 2, 4, 2, new[] { W + W + 1, W + 1, 0, 1 }) },
                { "L-3", new GamePiece('L', 3, 4, 3, new[] { W, 0, 1, 2 }) },
                { "J-0", new GamePiece('J', 0, 4, 2, new[] { W + W, W + W + 1, W + 1, 1 }) },
                { "J-1", new GamePiece('J', 1, 4, 3, new[] { W + 2, 0, 1, 2 }) },
                { "J-2", new GamePiece('J', 2, 4, 2, new[] { W + W, W, 0, 1 }) },
                { "J-3", new GamePiece('J', 3, 4, 3, new[] { W, W + 1, W + 2, 0 }) },
            };
        }
    }
}
