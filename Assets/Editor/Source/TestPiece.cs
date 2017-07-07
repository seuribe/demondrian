using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestPiece : TestConstants {

        [Test]
        public void RestoredStringifiedMatchesOriginal() {
            foreach (var p in new Piece[] { piece_t0_s1, piece_t0_s2, piece_t1_s4 }) {
                var pstr = p.SaveToString();
                var restored = Piece.RestoreFromString(pstr);
                Assert.IsTrue(p.Matches(restored), "restored piece matches original");
            }
        }

        [Test]
        public void RestoredStringifiedNotEqualsOriginal() {
            foreach (var p in new Piece[] { piece_t0_s1, piece_t0_s2, piece_t1_s4 }) {
                var pstr = p.SaveToString();
                var restored = Piece.RestoreFromString(pstr);
                Assert.AreNotEqual(p, restored, "restored piece does not equal original (different reference)");
            }
        }

        [Test]
        public void RestoredSerializedMatchesOriginal() {
            foreach (var p in new Piece[] { piece_t0_s1, piece_t0_s2, piece_t1_s4 }) {
                var json = p.ToJSON();
                var restored = Piece.FromJSON(json);
                Assert.IsTrue(p.Matches(restored), "restored piece matches original");
            }
        }

        [Test]
        public void RestoredSerializedNotEqualsOriginal() {
            foreach (var p in new Piece[] { piece_t0_s1, piece_t0_s2, piece_t1_s4 }) {
                var json = p.ToJSON();
                var restored = Piece.FromJSON(json);
                Assert.AreNotEqual(p, restored, "restored piece does not equal original (different reference)");
            }
        }

        [Test]
        public void NotEqualPiecesCanMatch() {
            Piece p1 = new Piece(PieceType.Get(0), 1);
            Piece p2 = new Piece(PieceType.Get(0), 1);
            Assert.AreNotEqual(p1, p2);
            Assert.IsTrue(p1.Matches(p2));
            Assert.IsTrue(p2.Matches(p1));
        }

        [Test]
        public void PieceMatchesSelf() {
            Piece p1 = new Piece(PieceType.Get(0), 1);
            Assert.IsTrue(p1.Matches(p1));
        }

        [Test]
        public void DifferentTypesDontMatch() {
            Piece p1 = new Piece(PieceType.Get(0), 1);
            Piece p2 = new Piece(PieceType.Get(1), 1);
            Assert.IsFalse(p1.Matches(p2));
            Assert.IsFalse(p2.Matches(p1));
        }

        [Test]
        public void DifferentSizesDontMatch() {
            Piece p1 = new Piece(PieceType.Get(0), 1);
            Piece p2 = new Piece(PieceType.Get(0), 2);
            Assert.IsFalse(p1.Matches(p2));
            Assert.IsFalse(p2.Matches(p1));
        }

        [Test]
        public void AllMatch() {
            var pieces = new Piece[] {
                new Piece(PieceType.Get(0), 1),
                new Piece(PieceType.Get(0), 1),
                new Piece(PieceType.Get(0), 1)
            };
            Assert.IsTrue(Piece.AllMatch(pieces));
            pieces = new Piece[] {
                new Piece(PieceType.Get(0), 2),
                new Piece(PieceType.Get(0), 2),
                new Piece(PieceType.Get(0), 2)
            };
            Assert.IsTrue(Piece.AllMatch(pieces));
        }

        [Test]
        public void AllDontMatchDifferentSize() {
            var pieces = new Piece[] {
                new Piece(PieceType.Get(0), 1),
                new Piece(PieceType.Get(0), 2),
                new Piece(PieceType.Get(0), 1)
            };
            Assert.IsFalse(Piece.AllMatch(pieces));
        }

        [Test]
        public void AllDontMatchDifferentType() {
            var pieces = new Piece[] {
                new Piece(PieceType.Get(1), 2),
                new Piece(PieceType.Get(0), 2),
                new Piece(PieceType.Get(0), 2)
            };
            Assert.IsFalse(Piece.AllMatch(pieces));
        }

        [Test]
        public void ClonedMatches() {
            foreach (var piece in new Piece[] { piece_t1_s4, piece_t0_s2, piece_t0_s1}) {
                var clone = piece.Clone();
                Assert.IsTrue(clone.Matches(piece));
            }
        }

        [Test]
        public void CloneNotSame() {
            var clone = piece_t1_s4.Clone();
            Assert.AreNotSame(clone, piece_t1_s2);
        }

    }
}