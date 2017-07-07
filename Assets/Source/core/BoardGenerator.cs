using System;
using com.perroelectrico.demondrian.core;

namespace com.perroelectrico.demondrian.generator {

    /// <summary>
    /// Generates random boards
    /// </summary>
    public class BoardGenerator {

        readonly Random rnd;
        public BoardGenerator(int seed) {
            rnd = new Random(seed);
        }

        public BoardGenerator() {
            rnd = new Random();
        }

        public Board GenerateRandom(int size, int numTypes, bool compact = true) {
            return GenerateRandom(size,
            new PieceType[]{
                PieceType.Get(0),
                PieceType.Get(1),
                PieceType.Get(2)
            }, compact);
        }

        private Piece GenerateRandomPiece(PieceType[] types, int size = 1) {
            return new Piece(types[rnd.Next(types.Length)], size);
        }

        public Board GenerateRandom(int size, PieceType[] types, bool compact = true) {
            Board board = new Board(size);

            foreach (var coord in board.AllCoords)
                board.Set(coord, GenerateRandomPiece(types, 1));

            if (compact)
                new BoardCompactor(board).Compact();

            return board;
        }
    }
}