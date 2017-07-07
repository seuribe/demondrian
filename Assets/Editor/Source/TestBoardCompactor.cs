using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestBoardCompactor : TestConstants {

        BoardBuilder builder = new BoardBuilder();

        [Test]
        public void CannotCompactDifferentTypes() {
            var board = builder.FromTypeGrid("0112");
            var compactor = new BoardCompactor(board);
            Assert.IsFalse(compactor.CanCompact(c00));
        }

        [Test]
        public void CannotCompactDifferentSizes() {
            var board = new Board(4);
//            {0, 0, 0, 2},
//            {0, 0, 0, 1},
//            {1 ,0, 0, 2},
//            {0, 0, 1, 0}
            board.Set(c00, new Piece(PieceType.Get(0), 1));
            board.Set(c10, new Piece(PieceType.Get(0), 1));
            board.Set(c20, new Piece(PieceType.Get(1), 1));
            board.Set(c30, new Piece(PieceType.Get(0), 1));

            board.Set(c01, new Piece(PieceType.Get(1), 1));
            board.Set(c11, new Piece(PieceType.Get(0), 1));
            board.Set(c21, new Piece(PieceType.Get(3), 1));
            board.Set(c31, new Piece(PieceType.Get(3), 1));

            board.Set(c02, new Piece(PieceType.Get(0), 2));

            board.Set(c22, new Piece(PieceType.Get(0), 1));
            board.Set(c32, new Piece(PieceType.Get(1), 1));

            board.Set(c23, new Piece(PieceType.Get(0), 1));
            board.Set(c33, new Piece(PieceType.Get(2), 1));

            var compactor = new BoardCompactor(board);
            Assert.IsFalse(compactor.CanCompact(c11));
            Assert.IsFalse(compactor.CanCompact(c12));
        }

        [Test]
        public void FindsNextCompactable() {
            var board = builder.FromTypeGrid("0001");
            var compactor = new BoardCompactor(board);
            board.Remove(c10);
            board.Set(c10, piece_t0_s1);
            Assert.AreEqual(c00, compactor.NextCompactable());
        }

        [Test]
        public void CannotCompactNonAligned() {
            var board = builder.FromTypeGrid(new int[,] {
                {1, 0, 1, 0, 1, 0, 1, 0},
                {1, 0, 1, 0, 1, 0, 1, 0},
                {2, 2, 2, 2, 1, 0, 1, 0},
                {2, 2, 2, 2, 1, 0, 1, 0},
                {1, 2, 2, 2, 2, 0, 1, 0},
                {1, 2, 2, 2, 2, 0, 1, 0},
                {1, 0, 1, 0, 1, 0, 1, 0},
                {1, 0, 1, 0, 1, 0, 1, 0}
            });
            var compactor = new BoardCompactor(board);
            // verify that the expected pieces are in place
            Assert.AreEqual(2, board.Get(c12).size);
            Assert.AreEqual(2, board.Get(c32).size);
            Assert.AreEqual(2, board.Get(c04).size);
            Assert.AreEqual(2, board.Get(c24).size);
            Assert.IsFalse(compactor.CanCompact());
        }

        [Test]
        public void CanCompactAligned2x2Recursive() {
            var board = builder.FromTypeGrid(new int[,] {
                {1, 0, 1, 0, 1, 0, 1, 0},
                {1, 0, 1, 0, 1, 0, 1, 0},
                {2, 2, 2, 2, 1, 0, 1, 0},
                {2, 2, 2, 2, 1, 0, 1, 0},
                {2, 2, 1, 2, 1, 0, 1, 0},
                {2, 2, 1, 2, 1, 0, 1, 0},
                {1, 0, 1, 0, 1, 0, 1, 0},
                {1, 0, 1, 0, 1, 0, 1, 0}
            });
            board.Remove(c22);
            board.Remove(c23);
            board.Set(c22, new Piece(PieceType.Get(2), 1));
            board.Set(c23, new Piece(PieceType.Get(2), 1));
            var compactor = new BoardCompactor(board);
            Assert.IsTrue(compactor.CanCompact(), "board:\n" + board);
            compactor.Compact();
            Assert.AreEqual(4, board.Get(c02).size);
        }
        [Test]
        public void CanCompact() {
            var board = builder.FromTypeGrid("0001");
            var compactor = new BoardCompactor(board);
            board.Remove(c10);
            board.Set(c10, piece_t0_s1);
            Assert.IsTrue(compactor.CanCompact(c00));
        }

        [Test]
        public void CannotCompactCoord() {
            var board = builder.FromTypeGrid("0001");
            var compactor = new BoardCompactor(board);
            Assert.IsFalse(compactor.CanCompact(c00));
            Assert.IsFalse(compactor.CanCompact(c01));
            Assert.IsFalse(compactor.CanCompact(c10));
            Assert.IsFalse(compactor.CanCompact(c11));
        }

        [Test]
        public void ArePotentialCompactablePieces() {
            var board = builder.FromTypeGrid("0001");
            var expected = new Piece[] {
                board.Get(c00), board.Get(c10), board.Get(c01), board.Get(c11)
            };
            var potentiallyCompactable = board.PiecesSquare(c00);
            CollectionAssert.AreEquivalent(expected, potentiallyCompactable);
        }

        [Test]
        public void SimpleCompaction2x2() {
            var board = builder.FromTypeGrid("00 01");
            var compactor = new BoardCompactor(board);
            Assert.IsFalse(compactor.CanCompact());
            board.Remove(c10);
            board.Set(c10, new Piece(PieceType.Get(0), 1));
            Assert.IsTrue(compactor.CanCompact());
            compactor.Compact();
            Assert.IsTrue(piece_t0_s2.Matches(board.Get(c00)), "board:\n" + board);
        }
    }
}