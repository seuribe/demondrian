using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestPieceType :TestConstants {
        readonly PieceType t1b = new PieceType(1);
        readonly PieceType t2b = new PieceType(2);
        readonly PieceType none = PieceType.MatchNone;
        readonly PieceType all = PieceType.MatchAll;

        [Test]
        public void TypeMatchesSelf() {
            Assert.IsTrue(t1.Matches(t1), "t1 matches itself");
            Assert.IsTrue(t1.Matches(t1b), "t1 matches t1b");
            Assert.IsTrue(t1b.Matches(t1b), "t1b matches t1b");
            Assert.IsTrue(t1b.Matches(t1), "t1b matches t1");
        }

        [Test]
        public void TypeDoesNotMatchDifferent() {
            Assert.IsFalse(t1.Matches(t2), "t1 does not match t2");
            Assert.IsFalse(t1.Matches(t2b), "t1 does not match t2b");
            Assert.IsFalse(t2.Matches(t1), "t1 does not match t2");
            Assert.IsFalse(t2b.Matches(t1), "t1 does not match t2b");
        }

        [Test]
        public void MatchAllMatchesAll() {
            var allT = new PieceType[] { t1, t1b, t2, t2b };
            foreach (var t in allT) {
                Assert.IsTrue(all.Matches(t), "match-all matches all");
                Assert.IsTrue(t.Matches(all), "all match match-all");
            }
        }

        [Test]
        public void MatchNoneMatchesNone() {
            var allT = new PieceType[] { t1, t1b, t2, t2b };
            foreach (var t in allT) {
                Assert.IsFalse(none.Matches(t), "match-none does not match any");
                Assert.IsFalse(t.Matches(none), "none matches match-none");
            }
        }

        [Test]
        public void RestoredStringifiedMatchesOriginal() {
            foreach (var t in new PieceType[] { t0, t1, t2 }) {
                var tstr = t.SaveToString();
                var restoredType = PieceType.RestoreFromString(tstr);
                Assert.AreEqual(t, restoredType, "restored type equals original");
            }
        }

        [Test]
        public void RestoredStringifiedEqualsOriginal() {
            foreach (var t in new PieceType[] { t0, t1, t2 }) {
                var tstr = t.SaveToString();
                var restoredType = PieceType.RestoreFromString(tstr);
                Assert.AreEqual(t, restoredType, "restored type equals original");
            }
        }

        [Test]
        public void RestoredSerializedMatchesOriginal() {
            foreach (var t in new PieceType[] { t0, t1, t2 }) {
                var json = t.ToJSON();
                var restoredType = PieceType.FromJSON(json);
                Assert.IsTrue(t.Matches(restoredType), "restored type matches original");
            }
        }

        [Test]
        public void RestoredSerializedEqualsOriginal() {
            foreach (var t in new PieceType[] { t0, t1, t2 }) {
                var json = t.ToJSON();
                var restoredType = PieceType.FromJSON(json);
                Assert.AreEqual(t, restoredType, "restored type equals original");
            }
        }
    }
}