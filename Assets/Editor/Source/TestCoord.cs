using System.Linq;

using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestCoord : TestConstants {

        [Test]
        public void CoordMatchesSelf() {
            var c00 = new Coord(0, 0);
            Assert.IsTrue(c00.Matches(c00));
        }

        [Test]
        public void CoordMatches() {
            var c00 = new Coord(0, 0);
            var c00b = new Coord(0, 0);
            Assert.IsTrue(c00.Matches(c00b));
            Assert.IsTrue(c00b.Matches(c00));
        }

        [Test]
        public void CoordsEquals() {
            var c00 = new Coord(0, 0);
            var c00b = new Coord(0, 0);
            Assert.AreEqual(c00, c00b);
            var c12 = new Coord(1, 2);
            var c12b = new Coord(1, 2);
            Assert.AreEqual(c12, c12b);
        }

        [Test]
        public void CoordsNotEquals() {
            var c00 = new Coord(0, 0);
            var c12 = new Coord(1, 2);
            Assert.AreNotEqual(c00, c12);
        }

        [Test]
        public void CoordsDontMatch() {
            var c00 = new Coord(0, 0);
            var c12 = new Coord(1, 2);
            Assert.IsFalse(c00.Matches(c12));
            Assert.IsFalse(c12.Matches(c00));
        }

        [Test]
        public void TestSquare() {
            var expected = new Coord[] {
                new Coord(1, 1),
                new Coord(1, 2),
                new Coord(2, 1),
                new Coord(2, 2)
            };
            var range = Coord.SquareRange(new Coord(1,1), 2).ToList<Coord>();
            CollectionAssert.AreEquivalent(expected, range);
        }

        [Test]
        public void HorizontalRange() {
            var expected = new Coord[] {
                c01, c11, c21
            };
            var range = Coord.Range(c01, c32);
            CollectionAssert.AreEquivalent(expected, range);
        }
        [Test]
        public void VerticalRange() {
            var expected = new Coord[] {
                c11, c12, c13
            };
            var range = Coord.Range(c11, new Coord(2, 4));
            CollectionAssert.AreEquivalent(expected, range);
        }

        [Test]
        public void CropBelowZero() {
            var coord = new Coord(-1, 0);
            var cropped = coord.Crop(10, 10);
            Assert.AreEqual(0, cropped.row);
            Assert.AreEqual(0, cropped.col);
        }

        [Test]
        public void CropUpperLimit() {
            var coord = new Coord(20, 10);
            var cropped = coord.Crop(15, 15);
            Assert.AreEqual(10, cropped.row);
            Assert.AreEqual(14, cropped.col);
        }

        [Test]
        public void RestoredSerializedMatchesOriginal() {
            foreach (var c in new Coord[] { c00, c10, c23, c03, c32, c30, c02 }) {
                var json = c.ToJSON();
                var restored = Coord.FromJSON(json);
                Assert.IsTrue(c.Matches(restored), "restored coord matches original");
            }
        }

        [Test]
        public void RestoredSerializedEqualsOriginal() {
            foreach (var c in new Coord[] { c00, c10, c23, c03, c32, c30, c02 }) {
                var json = c.ToJSON();
                var restored = Coord.FromJSON(json);
                Assert.AreEqual(c, restored, "restored coord does not equal original (different reference)");
            }
        }

        [Test]
        public void RestoredStringifiedMatchesOriginal() {
            foreach (var c in new Coord[] { c00, c10, c23, c03, c32, c30, c02 }) {
                var cstr = c.SaveToString();
                var restored = Coord.RestoreFromString(cstr);
                Assert.IsTrue(c.Matches(restored), "restored coord matches original");
            }
        }

        [Test]
        public void RestoredStringifiedEqualsOriginal() {
            foreach (var c in new Coord[] { c00, c10, c23, c03, c32, c30, c02 }) {
                var cstr = c.SaveToString();
                var restored = Coord.RestoreFromString(cstr);
                Assert.AreEqual(c, restored, "restored coord does not equal original (different reference)");
            }
        }
    }
}