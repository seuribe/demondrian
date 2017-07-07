using System;
using System.Collections.Generic;
using System.Linq;
using com.perroelectrico.demondrian.generator;
using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestPlayLogic : TestConstants {

        BoardBuilder builder = new BoardBuilder();
        BoardGenerator boardGen = new BoardGenerator();

        [Test]
        public void FindsObstaclesAbove() {
            var board = builder.FromTypeGrid("0012 0011 1211 0120");
            var logic = new PlayLogic(board);
            var expected = new Piece[] { board.Get(c02) };
            var piece = board.Get(c11);
            var obstacles = logic.GetObstaclesAbove(piece);
            CollectionAssert.AreEqual(expected, obstacles);
        }

        [Test]
        public void NoObstaclesAbove() {
            var board = builder.FromTypeGrid("0102 1011 1221 0120");
            var logic = new PlayLogic(board);
            foreach (var piece in board.Pieces.Keys)
                CollectionAssert.IsEmpty(logic.GetObstaclesAbove(piece));
        }

        [Test]
        public void AvailableMovesFound() {
            var board = builder.FromTypeGrid("0110 0212 2012 2222");
            var logic = new PlayLogic(board);
            Assert.IsTrue(logic.HasAvailableMoves());
        }

        [Test]
        public void NoAvailableMoves() {
            var board = builder.FromTypeGrid("01 01");
            var logic = new PlayLogic(board);
            Assert.IsFalse(logic.HasAvailableMoves());
        }

        [Test]
        public void PiecesInAvailableMovesAreAlone() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 0},
                {1, 0, 0, 2},
                {1, 1, 1, 2},
                {0, 1, 1, 2}});
            var logic = new PlayLogic(board);
            var moves = logic.AvailableMoves();
            var orientation = board.Orientation;
            foreach (var move in moves) {
                board.RotateTo(move.orientation);
                var matchingPieces = board.GetMatchingNeighbours(board.Get(move.coord));
                Assert.IsEmpty(matchingPieces);
                board.RotateTo(orientation);
            }
        }

        [Test]
        public void MovesSearchDoesNotChangeBoard() {
            var board = boardGen.GenerateRandom(8, 4);
            var logic = new PlayLogic(board);

            var clone = board.Clone();
            logic.AvailableMoves();
            Assert.IsTrue(clone.Matches(board), "board matches after searching for available moves");
        }

        [Test]
        public void AlonePieces() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 0},
                {1, 1, 0, 2},
                {1, 2, 1, 1},
                {0, 1, 1, 0}});
            var logic = new PlayLogic(board);

            foreach (var coord in new Coord[]{c00, c11, c22})
                Assert.IsTrue(logic.IsAlone(coord));

            foreach (var coord in new Coord[]{c01, c02, c12})
                Assert.IsFalse(logic.IsAlone(coord));
        }

        [Test]
        public void FindsDroppablesAbove() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 0},
                {1, 1, 0, 2},
                {1, 2, 1, 1},
                {0, 1, 1, 1}});
            var logic = new PlayLogic(board);
            var tests = new Dictionary<Coord, Coord[]> {
                {c00, new Coord[]{c01, c02, c03}},
                {c02, new Coord[]{c03}},
                {c11, new Coord[]{c12, c13}},
                {c20, new Coord[]{c22, c23, c32, c33}}
            };
            foreach (var coord in tests.Keys) {
                var droppables = logic.GetDroppablesAbove(board.Get(coord));
                var expected = board.GetAll(tests[coord]);
                CollectionAssert.AreEquivalent(expected, droppables);
            }
        }

        [Test]
        public void FindsDroppablesAboveDouble() {
            var board = builder.FromTypeGrid("0101 1101 1100 1110");
            var logic = new PlayLogic(board);
            var expected = Coord.SquareRange(c02, 2).Select( c => board.Get(c));
            var pieceCoords = Coord.SquareRange(c00, 2);
            foreach (var coord in pieceCoords) {
                var droppables = logic.GetDroppablesAbove(board.Get(coord));
                CollectionAssert.AreEquivalent(expected, droppables);
            }
        }

        [Test]
        public void NotDroppableIfBigger() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 2},
                {1, 1, 2, 2},
                {1, 2, 0, 1},
                {0, 0, 1, 1}});
            var logic = new PlayLogic(board);
            var bigPiece = board.Get(c22);
            foreach (var piece in board.GetAll(c20, c30, c21, c31)) {
                var droppables = logic.GetDroppablesAbove(piece);
                CollectionAssert.DoesNotContain(droppables, bigPiece);
            }
        }

        [Test]
        public void NotDroppableIfNotAligned() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 2},
                {1, 1, 2, 2},
                {1, 0, 0, 1},
                {0, 0, 0, 1}});
            var logic = new PlayLogic(board);
            var bigPiece = board.Get(c22);
            foreach (var piece in board.GetAll(c10, c20, c11, c21)) {
                var droppables = logic.GetDroppablesAbove(piece);
                CollectionAssert.DoesNotContain(droppables, bigPiece);
            }
        }

        [Test]
        public void SpecialCaseAvailableMoves() {
            var board = builder.FromTypeGrid("0111 0011 1100 1100");
            board.RotateRight();
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 2));
            var logic = new PlayLogic(board);
            foreach (var move in logic.AvailableMoves()) {
                game.Execute(move);
                game.Undo();
            }
            Assert.Pass("All moves could be done");
        }

        [Test]
        public void AvailableMovesFoundOnRotateBoard() {
            var board = builder.FromTypeGrid("0100 0000 0010 0000");
            board.RotateLeft();
            AvailableMovesAreValid(board,0, 2);
            Assert.Pass("All moves could be done");
        }

        [Test]
        public void AvailableMovesAreSameWhenRotating() {
            var board = builder.FromTypeGrid("0111 0011 1100 1100");
            var movesTop = new PlayLogic(board).AvailableMoves();
            board.RotateTo(Orientation.Right);
            var movesRight = new PlayLogic(board).AvailableMoves();
            board.RotateTo(Orientation.Left);
            var movesLeft = new PlayLogic(board).AvailableMoves();
            board.RotateTo(Orientation.Bottom);
            var movesBottom = new PlayLogic(board).AvailableMoves();
            CollectionAssert.AreEquivalent(movesTop, movesRight);
            CollectionAssert.AreEquivalent(movesTop, movesLeft);
            CollectionAssert.AreEquivalent(movesTop, movesBottom);
        }

        [Test]
        public void AvailableMovesAreSameWhenRotating2() {
            var board = builder.FromTypeGrid("0111 0011 1100 1100");
            var logic = new PlayLogic(board);
            var movesTop = logic.AvailableMoves();
            board.RotateTo(Orientation.Right);
            var movesRight = logic.AvailableMoves();
            board.RotateTo(Orientation.Left);
            var movesLeft = logic.AvailableMoves();
            board.RotateTo(Orientation.Bottom);
            var movesBottom = logic.AvailableMoves();
            CollectionAssert.AreEquivalent(movesTop, movesRight);
            CollectionAssert.AreEquivalent(movesTop, movesLeft);
            CollectionAssert.AreEquivalent(movesTop, movesBottom);
        }

        [Test]
        public void FailingCaseForSolver_1() {
            var board = builder.FromTypeGrid("1111 1111 0000 0001");
            board.RotateTo(Orientation.Bottom);
            AvailableMovesAreValid(board, 0, 2);
            Assert.Pass("All moves could be done");
        }
        [Test]
        public void FailingCaseForSolver_2() {
            var board = builder.FromTypeGrid("1000 0000 1111 1111");
            AvailableMovesAreValid(board, 0, 2);
            Assert.Pass("All moves could be done");
        }

        void AvailableMovesAreValid(Board board, int fromType, int numTypes) {
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(fromType, numTypes));
            var logic = new PlayLogic(board);
            foreach (var move in logic.AvailableMoves()) {
                System.Console.WriteLine("move: " + move);
                game.Execute(move);
                game.Undo();
            }
        }
    }
}