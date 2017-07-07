using com.perroelectrico.demondrian.generator;
using NUnit.Framework;
using System;
using System.Linq;

namespace com.perroelectrico.demondrian.core.test {

    [TestFixture]
    public class TestMoveSimulation : TestConstants {

        BoardBuilder builder = new BoardBuilder();
        BoardGenerator generator = new BoardGenerator();

        Func<BoardAction, bool> IsFallAction = (action => (action is FallAction));
        Func<BoardAction, bool> IsCompactAction = (action => (action is CompactAction));
        Func<BoardAction, bool> IsNewPieceAction = (action => (action is NewPieceAction));

        [Test]
        public void BoardIsUntouched() {
            var board = builder.FromTypeGrid("1020 1201 2201 0021");
            var backup = board.Clone();
            Assert.IsTrue(board.Matches(backup));
            foreach (var coord in board.AllCoords) {
                var simulator = MoveSimulator.FromCoord(board, coord, piece_t0_s1);
                simulator.Simulate();
                Assert.IsTrue(board.Matches(backup));
            }
        }

        [Test]
        public void StartsAndEndsInCorrectOrientation() {
            var board = builder.FromTypeGrid("10 21");
            for (int i = 0 ; i < 4 ; i++) {
                board.RotateLeft();
                foreach (var coord in board.AllCoords) {
                    var simulator = MoveSimulator.FromCoord(board, coord, piece_t0_s1);
                    simulator.Simulate();
                    var actions = simulator.GetMove().consequences;
                    var expected = OrientateAction.Get(board.Orientation);
                    Assert.AreEqual(expected, actions.First(), "first action orientates in board's current orientation");
                    Assert.AreEqual(expected, actions.Last(), "last action orientates in board's current orientation");
                }
            }
        }

        [Test]
        public void NotValidMoveDetected() {
            var board = builder.FromTypeGrid("10 20");
            foreach (var coord in new Coord[] { c10, c11 }) {
                var simulator = MoveSimulator.FromCoord(board, coord, piece_t0_s1);
                simulator.Simulate();
                Assert.IsFalse(simulator.IsMovePossible());
                Assert.Throws(typeof(InvalidGameOperationException),
                        new TestDelegate( () => {simulator.GetMove(); }));
            }
        }

        [Test]
        public void OnlyOneNewPieceAdded() {
            // all coords have moves
            var board = builder.FromTypeGrid("12 01");
            foreach (var coord in board.AllCoords) {
                var simulator = MoveSimulator.FromCoord(board, coord, piece_t0_s1);
                simulator.Simulate();
                var actions = simulator.GetMove().consequences;
                var newPieceAction = actions.Where( IsNewPieceAction );
                Assert.AreEqual(1, newPieceAction.Count());
            }
        }
        
        [Test]
        public void NewPieceHasExpectedType() {
            var board = builder.FromTypeGrid("10 21");
            foreach (var coord in board.AllCoords) {
                var simulator = MoveSimulator.FromCoord(board, coord, piece_t0_s1);
                simulator.Simulate();
                var actions = simulator.GetMove().consequences;
                var newPieceAction = actions.First( IsNewPieceAction ) as NewPieceAction;
                Assert.AreEqual(t0, newPieceAction.newPiece.type);
            }
        }

        [Test]
        public void NewPieceHasExpectedSize() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 0},
                {2, 1, 0, 2},
                {1, 2, 1, 1},
                {0, 1, 1, 1}});

            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));
            var logic = new PlayLogic(board);
            foreach (var move in logic.AvailableMoves()) {
                var simulator = MoveSimulator.FromPotentialMove(board, move, game.NextPieceIfPlaying(move));
                simulator.Simulate();
                var actions = simulator.GetMove().consequences;
                var newPieceAction = actions.First( IsNewPieceAction ) as NewPieceAction;
                Assert.AreEqual(simulator.oldPiece.size, newPieceAction.newPiece.size);
            }
        }

        [Test]
        public void CanAlwaysSimulateAvailableMovesRnd() {
            for (int i = 0 ; i < 10 ; i++) {
                var board = generator.GenerateRandom(8, 3);
                var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));

                var logic = new PlayLogic(board);
                var moves = logic.AvailableMoves();
                foreach (var move in moves) {
                    var simulator = MoveSimulator.FromPotentialMove(board, move, game.NextPieceIfPlaying(move));
                    simulator.Simulate();
                    Assert.IsTrue(simulator.IsMovePossible());
                }
            }
        }

        [Test]
        public void CanSimulateRotatedMove() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 0},
                {1, 0, 0, 2},
                {1, 1, 1, 2},
                {0, 1, 1, 2}});
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));

            var move = new PotentialMove(c03, Orientation.Right);
            var simulator = MoveSimulator.FromPotentialMove(board, move, game.NextPieceIfPlaying(move));
            simulator.Simulate();
            Assert.IsTrue(simulator.IsMovePossible());
        }

        [Test]
        public void NewPieceHasExpectedCoords() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 0},
                {1, 0, 0, 2},
                {1, 1, 1, 2},
                {0, 1, 1, 2}});
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));

            var moves = new PlayLogic(board).AvailableMoves();
            foreach (var move in moves) {
                var simulator = MoveSimulator.FromPotentialMove(board, move, game.NextPieceIfPlaying(move));
                simulator.Simulate();
                var coord = simulator.moveCoord;
                var piece = simulator.oldPiece;
                var actions = simulator.GetMove().consequences;
                var newPieceAction = actions.First( IsNewPieceAction ) as NewPieceAction;
                Assert.AreEqual(new Coord { col = coord.col, row = board.Size - piece.size}, newPieceAction.coord);
            }
        }

        [Test]
        public void FallActionsStartFromBottom() {
            var board = builder.FromTypeGrid("0100 0200 1212 2021");
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));

            var simulator = MoveSimulator.FromCoord(board, c10, game.NextPieceIfPlaying(new PotentialMove(c10, board.Orientation)));
            simulator.Simulate();
            Assert.IsTrue(simulator.IsMovePossible());
            var move = simulator.GetMove();
            var actions = move.consequences;
            var fallActions = actions.Where(IsFallAction);
            Assert.IsNotEmpty(fallActions);
            var arrActions = fallActions.ToArray();
            Assert.AreEqual(c11, (arrActions[0] as FallAction).coord);
            Assert.AreEqual(c12, (arrActions[1] as FallAction).coord);
            Assert.AreEqual(c13, (arrActions[2] as FallAction).coord);
        }


        [Test]
        public void FallActionsFallPiece2x2() {
            var board = builder.FromTypeGrid(new int[,] {
                {0, 1, 2, 0},
                {1, 0, 0, 2},
                {1, 1, 1, 2},
                {0, 1, 1, 2}});
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));

            var moves = new PotentialMove[] {
                new PotentialMove(c10, Orientation.Top),
                new PotentialMove(c01, Orientation.Right),
                new PotentialMove(c21, Orientation.Left),
            };
            foreach (var move in moves) {
                var simulator = MoveSimulator.FromPotentialMove(board, move, game.NextPieceIfPlaying(move));
                simulator.Simulate();
                Assert.IsTrue(simulator.IsMovePossible());
                var actions = simulator.GetMove().consequences;
                var fallActions = actions.Where(IsFallAction);
                foreach (var action in fallActions)
                    Assert.AreEqual(2, (action as FallAction).rows);
            }
        }

        [Test]
        public void FallActionsFallPieceSizeRnd() {
            for (int i = 0 ; i < 50 ; i++) {
                var board = generator.GenerateRandom(4, 3);
                var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));

                var logic = new PlayLogic(board);
                foreach (var potentialMove in logic.AvailableMoves()) {
                    var simulator = MoveSimulator.FromPotentialMove(board, potentialMove, game.NextPieceIfPlaying(potentialMove));
                    simulator.Simulate();
                    var move = simulator.GetMove();
                    var fallActions = move.consequences.Where(IsFallAction);
                    foreach (var action in fallActions) {
                        var orientatedCoord = board.TransportCoord(potentialMove.coord, potentialMove.orientation);
                        var removedPiece = board.Get(orientatedCoord);
                        Assert.AreEqual(removedPiece.size, (action as FallAction).rows,
                                "Fall Action:\n" + action + ",\nboard:\n" + board + "\nmove:\n" + potentialMove);
                    }
                }
            }
        }

        [Test]
        public void CompactsIfNeeded() {
            var board = builder.FromTypeGrid("01 00");
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(0, 3));

            var simulator = MoveSimulator.FromCoord(board, c11, game.NextPieceIfPlaying(new PotentialMove(c11, board.Orientation)));
            simulator.Simulate();
            Assert.IsTrue(simulator.IsMovePossible());
            var actions = simulator.GetMove().consequences;
            Assert.AreEqual(1, actions.Count( IsCompactAction ));
            var compactAction = actions.First( IsCompactAction ) as CompactAction;
            Assert.AreEqual(c00, compactAction.coord);
            Assert.IsTrue(piece_t0_s2.Matches(compactAction.bigPiece));
        }

        [Test]
        public void CanPlayDoublePieceFromCoord() {
            var board = builder.FromTypeGrid("0101 1101 1100 1110");
            foreach (var coord in Coord.SquareRange(c00, 2)) {
                var simulator = MoveSimulator.FromCoord(board, coord, piece_t0_s2);
                simulator.Simulate();
                Assert.Pass();
            }
        }

        [Test]
        public void CanPlayDoublePieceRotated() {
            var board = builder.FromTypeGrid("1110 1111 1000 0011");
            board.RotateLeft();
            foreach (var coord in Coord.SquareRange(c00, 2)) {
                var simulator = MoveSimulator.FromCoord(board, coord, piece_t0_s2);
                simulator.Simulate();
                Assert.Pass();
            }
        }

        [Test]
        public void CanPlayDoublePieceFromPotentialMove() {
            var board = builder.FromTypeGrid("0101 1101 1100 1110");
            foreach (var coord in Coord.SquareRange(c00, 2)) {
                var simulator = MoveSimulator.FromPotentialMove(board, new PotentialMove(coord, board.Orientation), piece_t0_s2);
                simulator.Simulate();
                Assert.Pass();
            }
        }
    }
}
