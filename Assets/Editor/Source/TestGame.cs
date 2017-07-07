using NUnit.Framework;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestGame : TestConstants {
        BoardBuilder builder = new BoardBuilder();

        [Test]
        public void IncomingQueueAsExpectedOnDeterministic() {
            var board = builder.FromTypeGrid("01 12");
            var rules = new GameRules {
                NextTypePolicy = NextTypePolicy.Deterministic
            };
            var types = PieceType.GetRange(0, 3);
            var game = new Game(board, rules, types);
            Assert.AreEqual(types, game.Incoming.Incoming());
        }

        [Test]
        public void NextPieceIsExpectedOnDeterministic() {
            var board = builder.FromTypeGrid("01 12");
            var rules = new GameRules {
                NextTypePolicy = NextTypePolicy.Deterministic
            };
            var types = PieceType.GetRange(0, 3);
            var game = new Game(board, rules, types);
            foreach (var coord in board.AllCoords)
                Assert.IsTrue(new Piece(types[0], 1).Matches(game.NextPieceIfPlaying(new PotentialMove(coord, board.Orientation))));
        }

        [Test]
        public void NextTypeIsExpectedOnRemovedType() {
            var board = builder.FromTypeGrid("01 12");
            var rules = new GameRules {
                NextTypePolicy = NextTypePolicy.RemovedType
            };
            var types = PieceType.GetRange(0, 3);
            var game = new Game(board, rules, types);
            foreach (var coord in board.AllCoords)
                Assert.IsTrue(new Piece(board.Get(coord).type, 1).Matches(game.NextPieceIfPlaying(new PotentialMove(coord, board.Orientation))));
        }

        [Test]
        public void CannotBeBuildWithNullBoard() {
            var rules = new GameRules {
                NextTypePolicy = NextTypePolicy.RemovedType
            };
            var types = PieceType.GetRange(0, 3);
            Assert.Throws(typeof(InvalidGameStateException),
                    new TestDelegate( () => new Game(null, rules, types)));
        }

        [Test]
        public void CannotBeBuildWithNullTypes() {
            var board = builder.FromTypeGrid("01 12");
            var rules = new GameRules {
                NextTypePolicy = NextTypePolicy.RemovedType
            };
            Assert.Throws(typeof(InvalidGameStateException),
                    new TestDelegate( () => new Game(board, rules, null)));
        }

        [Test]
        public void CannotBeBuildWithNoTypes() {
            var board = builder.FromTypeGrid("01 12");
            var rules = new GameRules {
                NextTypePolicy = NextTypePolicy.RemovedType
            };
            var types = new PieceType[0];
            Assert.Throws(typeof(InvalidGameStateException),
                    new TestDelegate( () => new Game(board, rules, types)));
        }

        [Test]
        public void NotSolvedGameDetected() {
            var board = builder.FromTypeGrid("01 11");
            var types = PieceType.GetRange(0, 2);
            var game = new Game(board, GameRules.ClassicRules, types);
            Assert.IsFalse(game.IsSolved);
        }

        [Test]
        public void SolvedGameDetected() {
            var board = builder.FromTypeGrid("10 00");
            var types = PieceType.GetRange(0, 2);
            var game = new Game(board, GameRules.ClassicRules, types);
            game.Execute(new PotentialMove(c01, game.Orientation));
            Assert.IsTrue(game.IsSolved);
        }
    }
}