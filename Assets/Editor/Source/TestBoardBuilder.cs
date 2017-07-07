using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestBoardBuilder : TestConstants {
        BoardBuilder builder = new BoardBuilder();

        [Test]
        public void FromInvalidTypeGridStringLength() {
            Assert.Throws(
                    typeof(InvalidBoardOperationException),
                    new TestDelegate(() => builder.FromTypeGrid("8982 3789 7823 980")));

            Assert.Throws(
                    typeof(InvalidBoardOperationException),
                    new TestDelegate(() => builder.FromTypeGrid("8982 3789 7823 98090")));
        }

        [Test]
        public void FromTypeGrid() {
            var typeGrid = new int[,]{
                {1, 2, 0, 2},
                {2, 1, 0, 2},
                {1, 2, 0, 2},
                {0, 0, 2, 1}
            };
            Board board = builder.FromTypeGrid(typeGrid);
            Assert.NotNull(board);
            Assert.AreEqual(t0, board.Get(c00).type);
            Assert.AreEqual(t0, board.Get(c10).type);
            Assert.AreEqual(t2, board.Get(c20).type);
            Assert.AreEqual(t1, board.Get(c30).type);

            Assert.AreEqual(t1, board.Get(c01).type);
            Assert.AreEqual(t2, board.Get(c11).type);
            Assert.AreEqual(t0, board.Get(c21).type);
            Assert.AreEqual(t2, board.Get(c31).type);

            Assert.AreEqual(t2, board.Get(c02).type);
            Assert.AreEqual(t1, board.Get(c12).type);
            Assert.AreEqual(t0, board.Get(c22).type);
            Assert.AreEqual(t2, board.Get(c32).type);

            Assert.AreEqual(t1, board.Get(c03).type);
            Assert.AreEqual(t2, board.Get(c13).type);
            Assert.AreEqual(t0, board.Get(c23).type);
            Assert.AreEqual(t2, board.Get(c33).type);
        }

        [Test]
        public void StringToTypeGrid() {
            var stringGrid = "1202 2102 1202 0021";
            var grid = BoardBuilder.StringToTypeGrid(stringGrid);
            Assert.AreEqual(1, grid[0,0]);
            Assert.AreEqual(2, grid[0,1]);
            Assert.AreEqual(0, grid[0,2]);
            Assert.AreEqual(2, grid[0,3]);

            Assert.AreEqual(0, grid[3,0]);
            Assert.AreEqual(0, grid[3,1]);
            Assert.AreEqual(2, grid[3,2]);
            Assert.AreEqual(1, grid[3,3]);
        }

        [Test]
        public void FromTypeGridString() {
            var stringGrid = "1202 2102 1202 0021";
            Board board = builder.FromTypeGrid(stringGrid);
            Assert.NotNull(board);
            Assert.AreEqual(t0, board.Get(c00).type);
            Assert.AreEqual(t0, board.Get(c10).type);
            Assert.AreEqual(t2, board.Get(c20).type);
            Assert.AreEqual(t1, board.Get(c30).type);

            Assert.AreEqual(t1, board.Get(c01).type);
            Assert.AreEqual(t2, board.Get(c11).type);
            Assert.AreEqual(t0, board.Get(c21).type);
            Assert.AreEqual(t2, board.Get(c31).type);

            Assert.AreEqual(t2, board.Get(c02).type);
            Assert.AreEqual(t1, board.Get(c12).type);
            Assert.AreEqual(t0, board.Get(c22).type);
            Assert.AreEqual(t2, board.Get(c32).type);

            Assert.AreEqual(t1, board.Get(c03).type);
            Assert.AreEqual(t2, board.Get(c13).type);
            Assert.AreEqual(t0, board.Get(c23).type);
            Assert.AreEqual(t2, board.Get(c33).type);
        }

        [Test]
        public void FromTypeGridCompactsBoard() {
            var grid = new int[,]{
                {0, 0, 1, 2},
                {0, 0, 2, 1},
                {2, 2, 1, 2},
                {1, 2, 2, 2}};
            var board = builder.FromTypeGrid(grid);
            var expectedPiece = new Piece(PieceType.Get(0), 2);
            var piece_c03 = board.Get(c03);
            Assert.IsTrue(expectedPiece.Matches(piece_c03));
        }

        [Test]
        public void BoardScaling() {
            var board = builder.FromTypeGrid("01 23");
            var cloned = builder.CloneScaled(board, 2);
            Assert.IsTrue(new Piece(PieceType.Get(0), 2).Matches(cloned.Get(c02)));
            Assert.IsTrue(new Piece(PieceType.Get(1), 2).Matches(cloned.Get(c22)));
            Assert.IsTrue(new Piece(PieceType.Get(2), 2).Matches(cloned.Get(c00)));
            Assert.IsTrue(new Piece(PieceType.Get(3), 2).Matches(cloned.Get(c20)));
        }
    }
}