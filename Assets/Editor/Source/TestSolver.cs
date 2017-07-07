using System.Collections.Generic;
using System.Linq;

using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.core.test;

using NUnit.Framework;

namespace com.perroelectrico.demondrian.solver.test {

    [TestFixture]
    public class TestSolver : TestConstants {
        readonly BoardBuilder builder = new BoardBuilder();

        Game ClassicGameFrom(string typeGrid, int fromType, int countTypes) {
            var board = builder.FromTypeGrid(typeGrid);
            return new Game(board, GameRules.ClassicRules, PieceType.GetRange(fromType, countTypes));
        }

        bool MovesAreSolution(Game game, IEnumerable<PotentialMove> moves) {
            System.Console.WriteLine("moves are solution for game:");
            System.Console.Write("Board Orientation: " + game.Orientation + "\n" + game.Board.TypeGrid());
            foreach (var move in moves) {
                System.Console.WriteLine("Move: " + move);
                game.Execute(move);
                System.Console.Write("Board Orientation: " + game.Orientation + "\n" + game.Board.TypeGrid());
                System.Console.WriteLine("-------------------------------------");
            }

            return game.IsSolved;
        }

        [Test]
        public void NoSolutionFound() {
            var game = ClassicGameFrom("01 01", 0, 2);
            var solver = new Solver(game);
            var result = solver.Solve(10000);
            Assert.IsTrue(result.IsUnsolvable);
            Assert.IsEmpty(result.Moves);
        }

        [Test]
        public void OutOfIterations() {
            var game = ClassicGameFrom("0101 1010 0101 1010", 0, 2);
            var solver = new Solver(game);
            var result = solver.Solve(10);
            Assert.IsTrue(result.IsCancelled);
        }

        [Test]
        public void CanSolveP2x2_1() {
            BoardCanBeSolvedAndVerified("01 11", 0, 2, maxMoves: 2);
        }
        [Test]
        public void CanSolveP2x2_2() {
            BoardCanBeSolvedAndVerified("11 11", 0, 2, maxMoves: 1);
        }
        [Test]
        public void CanSolveP2x2_3() {
            BoardCanBeSolvedAndVerified("00 01", 0, 2, maxMoves: 1);
        }
        [Test]
        public void CanSolveP4x4_1() {
            BoardCanBeSolvedAndVerified("0000 1100 1100 0000", 0, 2, maxMoves: 6);
        }
        [Test]
        public void CanSolveP4x4_2() {
            BoardCanBeSolvedAndVerified("1111 1011 1111 1111", 0, 2, maxMoves: 6);
        }
        [Test]
        public void CanSolveP4x4_3() {
            BoardCanBeSolvedAndVerified("0110 1101 1010 1100", 0, 2, 100000, 8);// tested by hand: it is solvable in 8 moves
        }

        const string P4x4_4 = "0111 0011 1100 1100";
        [Test]
        public void CanSolveP4x4_4() {
            var board = builder.FromTypeGrid(P4x4_4);
            BoardCanBeSolvedAndVerified(board, 0, 2, maxMoves: 8);
        }
        [Test]
        public void CanSolveP4x4_4r() {
            var board = builder.FromTypeGrid(P4x4_4);
            board.RotateTo(Orientation.Right);
            BoardCanBeSolvedAndVerified(board, 0, 2, maxMoves: 4);
        }
        [Test]
        public void CanSolveP4x4_4l() {
            var board = builder.FromTypeGrid(P4x4_4);
            board.RotateTo(Orientation.Left);
            BoardCanBeSolvedAndVerified(board, 0, 2, maxMoves: 4);
        }
        [Test]
        public void CanSolveP4x4_4b() {
            var board = builder.FromTypeGrid(P4x4_4);
            board.RotateTo(Orientation.Bottom);
            BoardCanBeSolvedAndVerified(board, 0, 2, maxMoves: 4);
        }
        [Test]
        public void CanSolveP4x4_5() {
            BoardCanBeSolvedAndVerified("1111 1111 0000 0001", 0, 2, maxMoves: 4);
        }

        void CannotBeSolved(string typeGrid, int fromType, int countTypes, int maxIterations = 10000, int maxMoves = 25) {
            var board = builder.FromTypeGrid(typeGrid);
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(fromType, countTypes));
            var solver = new Solver(game);
            var result = solver.Solve(maxIterations, maxMoves);
            Assert.IsTrue(result.IsUnsolvable);
        }

        string Orientate(string typeGrid, Orientation orientation) {
            var board = builder.FromTypeGrid(typeGrid);
            board.RotateTo(orientation);
            return board.TypeGrid();
        }

        void BoardCanBeSolvedAndVerified(string typeGrid, int fromType, int countTypes, int maxIterations = 10000, int maxMoves = 25) {
            BoardCanBeSolvedAndVerified(builder.FromTypeGrid(typeGrid), fromType, countTypes, maxIterations, maxMoves);
        }

        void BoardCanBeSolvedAndVerified(Board board, int fromType, int countTypes, int maxIterations = 10000, int maxMoves = 25) {
            var game = new Game(board, GameRules.ClassicRules, PieceType.GetRange(fromType, countTypes));
            var solver = new Solver(game);
            var result = solver.Solve(maxIterations, maxMoves);
            Assert.IsTrue(result.IsSolvable, "result: " + result);
            Assert.GreaterOrEqual(maxMoves, result.Moves.Count());
            Assert.IsTrue(MovesAreSolution(game, result.Moves), result.ToString());
        }
    }
}