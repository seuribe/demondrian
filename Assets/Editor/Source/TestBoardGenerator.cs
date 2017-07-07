using System.Linq;
using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.core.test;
using NUnit.Framework;

namespace com.perroelectrico.demondrian.generator.test {

    [TestFixture]
    public class TestBoardGenerator : TestConstants {
        readonly BoardGenerator generator = new BoardGenerator();

        [Test]
        public void GenerateOneTypeOnly() {
            var types = new PieceType[]{t0};
            var board = generator.GenerateRandom(8, types);
            Assert.IsTrue(board.IsFull());
            board.AllCoords.ToList().ForEach ( c =>
                Assert.IsTrue(t0.Matches(board.Get(c).type)));
        }

        [Test]
        public void GenerateMultipleTypes() {
            var types = new PieceType[]{t0, t1, t2};
            var board = generator.GenerateRandom(8, types);
            Assert.IsTrue(board.IsFull());
            board.AllCoords.ToList().ForEach ( c =>
            Assert.IsTrue(
                    t0.Matches(board.Get(c).type) ||
                    t1.Matches(board.Get(c).type) ||
                    t2.Matches(board.Get(c).type)
            ));
        }

        [Test]
        public void GeneratedIsAlwaysCompactedRnd() {
            DoTimes(1000, (i) => {
                var board = generator.GenerateRandom(8, 3, true);
                var compactor = new BoardCompactor(board);
                Assert.IsFalse(compactor.CanCompact());
            });
        }
    }
}