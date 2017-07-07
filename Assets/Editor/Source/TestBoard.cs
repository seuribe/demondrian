using System.Collections.Generic;
using System.Linq;
using com.perroelectrico.demondrian.generator;
using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestBoard : TestConstants {

        BoardBuilder builder = new BoardBuilder();
        BoardGenerator boardGen = new BoardGenerator();

        [Test]
        public void AreEmptyDetectsNonEmpty() {
            var board = boardGen.GenerateRandom(4, 3);
            Assert.IsFalse(board.AllEmpty(new Coord[]{c00, c11}));
            board.Remove(c00);
            Assert.IsFalse(board.AllEmpty(new Coord[]{c00, c11}));
        }

        [Test]
        public void SetFailsOnNonEmpty() {
            var board = boardGen.GenerateRandom(4, 3);
            Assert.Throws(typeof(InvalidBoardOperationException),
                new TestDelegate(() => board.Set(c00, piece_t0_s1)));
        }

        [Test]
        public void SetOnEmptyCoords() {
            var board = new Board(2);
            foreach (var coord in board.AllCoords)
                board.Set(coord, new Piece(PieceType.Get(0), 1));

            Assert.Pass("No exceptions thrown");
        }

        [Test]
        public void SetFillsAllCoords() {
            var board = new Board(4);
            board.Set(c00, piece_t1_s4);
            Assert.IsTrue(board.IsFull());

            board.Clear();
            board.Set(c11, piece_t0_s1);
            foreach (var coord in board.AllCoords)
                Assert.AreEqual(coord.Matches(c11), !board.IsEmpty(coord));
        }

        [Test]
        public void RotateLeft() {
            // 12   c01 c11
            // 34   c00 c10
            var board = builder.FromTypeGrid("1234");
            board.RotateLeft();
            // 24   c01 c11
            // 13   c00 c10
            Assert.AreEqual(PieceType.Get(2), board.Get(c01).type);
            Assert.AreEqual(PieceType.Get(4), board.Get(c11).type);
            Assert.AreEqual(PieceType.Get(1), board.Get(c00).type);
            Assert.AreEqual(PieceType.Get(3), board.Get(c10).type);
        }

        [Test]
        public void RotateRight() {
            // 12   c01 c11
            // 34   c00 c10
            var board = builder.FromTypeGrid("1234");
            board.RotateRight();
            // 31   c01 c11
            // 42   c00 c10
            Assert.AreEqual(PieceType.Get(3), board.Get(c01).type);
            Assert.AreEqual(PieceType.Get(1), board.Get(c11).type);
            Assert.AreEqual(PieceType.Get(4), board.Get(c00).type);
            Assert.AreEqual(PieceType.Get(2), board.Get(c10).type);
        }

        [Test]
        public void RotateLeftBigPieces() {
            // 11 22   ..  ..  ..  ..
            // 11 22   c02 ..  c22 ..
            // 33 44   ..  ..  ..  ..
            // 33 44   c00 ..  c20 ..
            var initialState = new Dictionary<Piece, Coord>() {
                {piece_t1_s2, c02}, {piece_t2_s2, c22},
                {piece_t3_s2, c00}, {piece_t4_s2, c20}};
            var rotatedState = new Dictionary<Piece, Coord>() {
                {piece_t2_s2, c02}, {piece_t4_s2, c22},
                {piece_t1_s2, c00}, {piece_t3_s2, c20}};
            var board = new Board(4);
            board.ReplacePieces(initialState);
            board.RotateLeft();
            foreach (var piece in rotatedState.Keys)
                Assert.AreEqual(rotatedState[piece], board.Where(piece));
        }

        [Test]
        public void TransportCoord() {
            var board = new Board(4);
            Assert.AreEqual(c00, board.TransportCoord(c00, Orientation.Top));
            Assert.AreEqual(c30, board.TransportCoord(c00, Orientation.Right));
            Assert.AreEqual(c03, board.TransportCoord(c00, Orientation.Left));
            Assert.AreEqual(c33, board.TransportCoord(c00, Orientation.Bottom));

            board.RotateTo(Orientation.Bottom);
            Assert.AreEqual(c00, board.TransportCoord(c33, Orientation.Top));
            Assert.AreEqual(c33, board.TransportCoord(c03, Orientation.Right));
            Assert.AreEqual(c33, board.TransportCoord(c30, Orientation.Left));
            Assert.AreEqual(c00, board.TransportCoord(c00, Orientation.Bottom));
        }

        [Test]
        public void SetPieces2x2() {
            var board = new Board(4);
            board.Set(c00, piece_t1_s2);
            board.Set(c02, piece_t2_s2);
            board.Set(c20, piece_t3_s2);
            board.Set(c22, piece_t4_s2);
            foreach (var coord in new Coord[]{c00, c01, c10, c11})
                Assert.AreEqual(piece_t1_s2, board.Get(coord));
            foreach (var coord in new Coord[]{c02, c03, c12, c13})
                Assert.AreEqual(piece_t2_s2, board.Get(coord));
            foreach (var coord in new Coord[]{c20, c21, c30, c31})
                Assert.AreEqual(piece_t3_s2, board.Get(coord));
            foreach (var coord in new Coord[]{c22, c23, c32, c33})
                Assert.AreEqual(piece_t4_s2, board.Get(coord));
        }

        [Test]
        public void SetOutOfRange() {
            var board = new Board(2);
            Assert.Throws(typeof(InvalidBoardOperationException),
                new TestDelegate( () => board.Set(c22, piece_t0_s1)));
            Assert.Throws(typeof(InvalidBoardOperationException),
                    new TestDelegate( () => board.Set(c11, piece_t0_s2)));
        }

        [Test]
        public void RotateCoordLeft() {
            var board = new Board(8);
            Assert.AreEqual(new Coord(7, 0), board.RotateCoordLeft(c00));
            Assert.AreEqual(new Coord(6, 1), board.RotateCoordLeft(c11));
            Assert.AreEqual(new Coord(1, 6), board.RotateCoordLeft(c66));
        }

        [Test]
        public void RotateCoordRight() {
            var board = new Board(8);
            Assert.AreEqual(new Coord(0, 7), board.RotateCoordRight(c00));
            Assert.AreEqual(new Coord(1, 6), board.RotateCoordRight(c11));
            Assert.AreEqual(new Coord(6, 1), board.RotateCoordRight(c66));
        }

        [Test]
        public void NeighbourCoordsAreInRange() {
            var board = builder.FromTypeGrid(new int[,] {
                {1, 2, 0, 1},
                {2, 1, 1, 0},
                {1, 2, 0, 1},
                {0, 1, 2, 0}}
            );
            foreach (var piece in board.Pieces.Keys) {
                var nCoords = board.GetNeighbouringCoords(piece);
                Assert.IsTrue(nCoords.All( c => !board.OutOfRange(c)));
            }
        }

        [Test]
        public void NeighbourCoordsAreComplete() {
            var board = builder.FromTypeGrid(new int[,] {
                {1, 2, 0, 1},
                {2, 1, 1, 0},
                {1, 2, 0, 1},
                {0, 1, 2, 0}}
            );
            var tests = new Dictionary<Coord, Coord[]>() {
                {c11, new Coord[]{ c10, c01, c12, c21}},
                {c31, new Coord[]{ c30, c21, c32}}
            };
            foreach (var coord in tests.Keys) {
                var expected = tests[coord];
                var piece = board.Get(coord);
                var neighbours = board.GetNeighbouringCoords(piece);
                CollectionAssert.AreEquivalent(expected, neighbours);
            }
        }

        [Test]
        public void MatchingNeighboursMatch() {
            var board = builder.FromTypeGrid(new int[,] {
                {1, 2, 0, 1},
                {2, 1, 1, 0},
                {1, 2, 0, 1},
                {0, 1, 2, 0}}
            );
            foreach (var piece in board.Pieces.Keys) {
                var nPieces = board.GetMatchingNeighbours(piece);
                Assert.IsTrue(nPieces.All( p => p.Matches(piece)));
            }
        }

        [Test]
        public void MatchingNeighboursAreExpected() {
            var board = builder.FromTypeGrid(new int[,] {
                {1, 2, 0, 1},
                {2, 1, 1, 0},
                {1, 2, 0, 0},
                {0, 1, 2, 0}}
            );
            var tests = new Dictionary<Piece, Piece[]>() {
                { board.Get(c11), new Piece[]{}},
                { board.Get(c21), new Piece[]{ board.Get(c31) }},
                { board.Get(c12), new Piece[]{ board.Get(c22) }},
                { board.Get(c31), new Piece[]{
                    board.Get(c21), board.Get(c30), board.Get(c32)}}};
            foreach (var p_exp in tests) {
                var piece = p_exp.Key;
                var expected = p_exp.Value;
                var matching = board.GetMatchingNeighbours(piece);
                CollectionAssert.AreEquivalent(expected, matching);
            }
        }

        [Test]
        public void ClonedBoardMatchesOriginalRnd() {
            for (int i = 0 ; i < 100 ; i++) {
                var board = boardGen.GenerateRandom(8, 3);
                var clone = board.Clone();
                Assert.IsTrue(clone.Matches(board));
            }
        }

        [Test]
        public void CopiedBoardMatchesRnd() {
            int size = 8;
            var board = boardGen.GenerateRandom(size, 4);
            var board2= new Board(size);

            foreach (var piece in board.Pieces.Keys)
                board2.Set(board.Where(piece), piece);

            Assert.IsTrue(board.Matches(board2));
        }

        [Test]
        public void MatchesIsSymmetricRnd() {
            for (int i = 0 ; i < 100 ; i++) {
                var board = boardGen.GenerateRandom(4, 2);
                var board2 = boardGen.GenerateRandom(4, 2);
                Assert.AreEqual(board.Matches(board2),
                                board2.Matches(board));
            }
        }

        [Test]
        public void MatchesIsReflexiveRnd() {
            for (int i = 0; i < 100; i++) {
                var board = boardGen.GenerateRandom(4, 2);
                Assert.IsTrue(board.Matches(board));
            }
        }

        [Test]
        public void CannotFillNonEmptyBoard() {
            Board b = new Board(4);
            b.Set(c01, piece_t0_s1);

            Assert.Throws(typeof(InvalidBoardOperationException),
                    new TestDelegate(() => b.Fill(t2)));
        }

        [Test]
        public void BoardFilledIsFull() {
            Board b = new Board(8);
            b.Fill(t1);
            foreach (var c in b.AllCoords)
                Assert.IsTrue(b.Get(c).type.Matches(t1));

            b.Clear();
            b.Fill(t2);
            foreach (var c in b.AllCoords)
                Assert.IsTrue(b.Get(c).type.Matches(t2));
        }


        private void AssertMatches(Board a, Board b) {
            int size = a.Size;
            for (int y = 0 ; y < size ; y++) {
                for (int x = 0 ; x < size ; x++) {
                    var c = new Coord(x, y);
                    Assert.IsTrue(a.Get(c).Matches(b.Get(c)), "pieces at " + x + "," + y + " match: " + a.Get(c) + " vs. " + b.Get(c));
                    Assert.IsTrue(b.Get(c).Matches(a.Get(c)), "pieces at " + x + "," + y + " match: " + b.Get(c) + " vs. " + a.Get(c));
                }
            }
        }

        [Test]
        public void ReplacePiecesOnePiece() {
            string compactBoard = "1202 2102 1202 0021";
            Board board = builder.FromTypeGrid(compactBoard);
            Assert.IsFalse(piece_t1_s4.Matches(board.Get(c00)));

            var onePieceDict = new Dictionary<Piece, Coord>();
            onePieceDict[piece_t1_s4] = c00;
            board.ReplacePieces(onePieceDict);
            Assert.IsTrue(piece_t1_s4.Matches(board.Get(c00)));
        }

        [Test]
        public void ReplacePiecesBig() {
            var initialState = new Dictionary<Piece, Coord>() {
                {piece_t1_s2, c02}, {piece_t2_s2, c22},
                {piece_t3_s2, c00}, {piece_t4_s2, c20}};
            var board = new Board(4);
            board.ReplacePieces(initialState);
            foreach (var piece_coord in initialState)
                Assert.AreEqual(piece_coord.Value, board.Where(piece_coord.Key));
        }

        [Test]
        public void IsRightOf() {
            var board = builder.FromTypeGrid("0111");
            var p00 = board.Get(c00);
            var p10 = board.Get(c10);
            var p01 = board.Get(c01);
            var p11 = board.Get(c11);
            Assert.IsTrue(board.IsRightOf(p10, p00));
            Assert.IsTrue(board.IsRightOf(p11, p01));
        }

        [Test]
        public void IsNotRightOf() {
            var board = builder.FromTypeGrid("0111");
            var p10 = board.Get(c10);
            var p01 = board.Get(c01);
            Assert.IsFalse(board.IsRightOf(p01, p10));
        }

        [Test]
        public void IsAbove() {
            var board = builder.FromTypeGrid("0121");
            var p00 = board.Get(c00);
            var p01 = board.Get(c01);
            var p10 = board.Get(c10);
            var p11 = board.Get(c11);
            Assert.IsTrue(board.IsAbove(p11, p10));
            Assert.IsTrue(board.IsAbove(p01, p00));
        }

        [Test]
        public void IsNotAbove() {
            var board = builder.FromTypeGrid("0121");
            var p00 = board.Get(c00);
            var p01 = board.Get(c01);
            var p10 = board.Get(c10);
            var p11 = board.Get(c11);
            Assert.IsFalse(board.IsAbove(p11, p00));
            Assert.IsFalse(board.IsAbove(p01, p10));
        }

        private IEnumerable<Coord> AllCordsStepping(int size, int step) {
            for (int y = 0 ; y < size ; y += size)
                for (int x = 0; x < size; x += size)
                    yield return new Coord(x, y);
        }

        [Test]
        public void PieceCoordsContainPiece() {
            var board = new Board(4);
            foreach (var size in new int[]{1, 2, 4}) {
                foreach (var coord in AllCordsStepping(board.Size, size)) {
                    var piece = new Piece(PieceType.Get(1), size);
                    board.Set(coord, piece);
                    var pieceCoords = board.PieceCoords(piece);
                    foreach (var c in pieceCoords) {
                        Assert.AreEqual(piece, board.Get(c));
                        Assert.GreaterOrEqual(c.col, coord.col);
                        Assert.GreaterOrEqual(c.row, coord.row);
                        Assert.Less(c.col, coord.col + size);
                        Assert.Less(c.row, coord.row + size);
                    }
                    board.Remove(coord);
                }
            }
        }

        [Test]
        public void AreMatchingNeighbours() {
            var board = builder.FromTypeGrid("0111");
            var p00 = board.Get(c00);
            var p10 = board.Get(c10);
            var p11 = board.Get(c11);
            Assert.IsTrue(board.IsMatchingNeighbour(p00, p10));
            Assert.IsTrue(board.IsMatchingNeighbour(p11, p10));
        }

        [Test]
        public void AreNotMatchingNeighbours() {
            var board = builder.FromTypeGrid("0111");
            var p00 = board.Get(c00);
            var p01 = board.Get(c01);
            var p10 = board.Get(c10);
            var p11 = board.Get(c11);
            Assert.IsFalse(board.IsMatchingNeighbour(p00, p01));
            Assert.IsFalse(board.IsMatchingNeighbour(p11, p01));
            Assert.IsFalse(board.IsMatchingNeighbour(p00, p11));
            Assert.IsFalse(board.IsMatchingNeighbour(p10, p01));
        }

        [Test]
        public void ClonedBoardHasSamePieceRefs() {
            var board = builder.FromTypeGrid("0120 2100 0121 2201");
            var cloned = board.Clone();
            AssertHaveSamePieceRefs(board, cloned);
        }

        private void AssertHaveSamePieceRefs(Board a, Board b) {
            foreach (var piece in a.Pieces.Keys)
                Assert.IsTrue(b.ContainsPiece(piece));

            foreach (var piece in b.Pieces.Keys)
                Assert.IsTrue(a.ContainsPiece(piece));
        }

        [Test]
        public void RotatedBoardHasSamePieceRefs() {
            var board = builder.FromTypeGrid("0120 2100 0121 2201");

            var rotated = board.Clone();
            rotated.RotateRight();
            AssertHaveSamePieceRefs(board, rotated);

            rotated = board.Clone();
            rotated.RotateLeft();
            AssertHaveSamePieceRefs(board, rotated);
        }

        [Test]
        public void BoardSizeMustBeGreaterThanZero() {
            Assert.Throws(typeof(InvalidBoardStateException),
                new TestDelegate( () => new Board(0)));

            Assert.Throws(typeof(InvalidBoardStateException),
                    new TestDelegate( () => new Board(-3)));
        }
    }

}
