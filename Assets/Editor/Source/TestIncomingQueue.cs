using System.Collections.Generic;

using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {
    [TestFixture]
    public class TestIncomingQueue : TestConstants {

        [Test]
        public void NextInClassicIsExpected() {
            var queue = IncomingQueue.GetQueue(GameRules.ClassicRules, t0, t1, t2);
            Assert.AreEqual(t0, queue.Next());
            Assert.AreEqual(t1, queue.Next());
            Assert.AreEqual(t2, queue.Next());
            Assert.AreEqual(t0, queue.Next());
        }

        [Test]
        public void OnlyExpectedPiecesReturnedInRandom() {
            var types = PieceType.GetRange(0, 4);
            var queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 0,
                MatchNoneWeight = 0,
                OtherPiecesWeight = 100
            }, types);

            for (int i = 0 ; i < 100 ; i++)
                Assert.Contains(queue.Next(), types);
        }

        [Test]
        public void OnlyMatchAllReturned() {
            var types = PieceType.GetRange(0, 4);
            var queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 1,
                MatchNoneWeight = 0,
                OtherPiecesWeight = 0
            }, types);

            for (int i = 0 ; i < 100 ; i++)
                Assert.AreEqual(PieceType.MatchAll, queue.Next());
        }

        [Test]
        public void OnlyMatchNoneReturned() {
            var types = PieceType.GetRange(0, 4);
            var queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 0,
                MatchNoneWeight = 1,
                OtherPiecesWeight = 0
            }, types);

            for (int i = 0 ; i < 100 ; i++)
                Assert.AreEqual(PieceType.MatchNone, queue.Next());
        }

        [Test]
        public void ReturnsExpectedPiecesAndSpecials() {
            var types = PieceType.GetRange(0, 4);
            var queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 20,
                MatchNoneWeight = 10,
                OtherPiecesWeight = 100
            }, types);

            var allTypes = new List<PieceType>(types);
            allTypes.Add(PieceType.MatchAll);
            allTypes.Add(PieceType.MatchNone);

            for (int i = 0 ; i < 100 ; i++)
                Assert.Contains(queue.Next(), allTypes);
        }

        [Test]
        public void LookAheadHasExpectedSize() {
            var queue = IncomingQueue.GetQueue(GameRules.ClassicRules, t3, t1, t0);
            Assert.AreEqual(3, queue.LookAheadSize());
            queue = IncomingQueue.GetQueue(GameRules.ClassicRules, PieceType.GetRange(0, 100));
            Assert.AreEqual(100, queue.LookAheadSize());

            queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 0, MatchNoneWeight = 0, OtherPiecesWeight = 100,
            }, PieceType.GetRange(0, 37));
            Assert.AreEqual(37, queue.LookAheadSize());
            queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 1, MatchNoneWeight = 0, OtherPiecesWeight = 100,
            }, PieceType.GetRange(0, 37));
            Assert.AreEqual(38, queue.LookAheadSize());
            queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 0, MatchNoneWeight = 0, OtherPiecesWeight = 100,
            }, PieceType.GetRange(0, 37));
            Assert.AreEqual(37, queue.LookAheadSize());
            queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 1, MatchNoneWeight = 1, OtherPiecesWeight = 100,
            }, PieceType.GetRange(0, 37));
            queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.Random,
                MatchAllWeight = 10, MatchNoneWeight = 10, OtherPiecesWeight = 0,
            }, PieceType.GetRange(0, 37));
            Assert.AreEqual(2, queue.LookAheadSize());

            queue = IncomingQueue.GetQueue(new GameRules {
                NextTypePolicy = NextTypePolicy.RemovedType
            }, PieceType.GetRange(0, 4));
            Assert.AreEqual(0, queue.LookAheadSize());
        }

        [Test]
        public void LookAheadOnDeterministicIsCorrect() {
            var queue = IncomingQueue.GetQueue(GameRules.ClassicRules, t3, t1, t0);
            Assert.AreEqual(t3, queue.Incoming()[0]);
            Assert.AreEqual(t1, queue.Incoming()[1]);
            Assert.AreEqual(t0, queue.Incoming()[2]);
        }

        [Test]
        public void IncomingDoesNotAffectNext() {
            var types = new PieceType[] { t1, t2, t3};
            var queue = IncomingQueue.GetQueue(GameRules.ClassicRules, types);
            CollectionAssert.AreEqual(types, queue.Incoming());
            Assert.AreEqual(t1, queue.Next());
            Assert.AreEqual(t2, queue.Next());
            Assert.AreEqual(t3, queue.Next());
            Assert.AreEqual(t1, queue.Next());
            Assert.AreEqual(t2, queue.Next());
            CollectionAssert.AreEqual(new PieceType[] { t3, t1, t2}, queue.Incoming());
            Assert.AreEqual(t3, queue.Next());
        }

        [Test]
        public void LookAheadGoesBackAfterUndo() {
            var types = PieceType.GetRange(0, 3);
            var queue = IncomingQueue.GetQueue(GameRules.ClassicRules, types);
            CollectionAssert.AreEqual(types, queue.Incoming());
            queue.Next();
            queue.Undo();
            CollectionAssert.AreEqual(types, queue.Incoming());
        }

        [Test]
        public void UndoReturnsToExpectedState() {
            var types = PieceType.GetRange(0, 3);
            var queue = IncomingQueue.GetQueue(GameRules.ClassicRules, types);
            Assert.AreEqual(t0, queue.Next());
            queue.Undo();
            Assert.AreEqual(t0, queue.Next());
            Assert.AreEqual(t1, queue.Next());
            queue.Undo();
            Assert.AreEqual(t1, queue.Next());
            Assert.AreEqual(t2, queue.Next());
            queue.Undo();
            Assert.AreEqual(t2, queue.Next());
            Assert.AreEqual(t0, queue.Next());
            queue.Undo();
            Assert.AreEqual(t0, queue.Next());
            queue.Undo();
            queue.Undo();
            Assert.AreEqual(t2, queue.Next());
        }
    }
}