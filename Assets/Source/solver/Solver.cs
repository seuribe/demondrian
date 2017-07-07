using System.Collections.Generic;
using System.Linq;
using com.perroelectrico.demondrian.core;

namespace com.perroelectrico.demondrian.solver {

    public class SolverException : System.Exception {
        public SolverException() {}
        public SolverException(string msg) : base(msg) {}
    }

    public class SolveResult {
        public enum State {
            Solvable,
            BackTracked,
            Cancelled
        }

        readonly IEnumerable<PotentialMove> moves;
        readonly State state;

        public bool IsUnsolvable { get { return state == State.BackTracked; }}
        public bool IsSolvable { get { return state == State.Solvable; }}
        public bool IsCancelled { get { return state == State.Cancelled; }}
        public IEnumerable<PotentialMove> Moves {
            get { return moves; }
        }

        public bool Finished {
            get {
                return state == State.Solvable ||
                state == State.Cancelled;
            }
        }

        private SolveResult(IEnumerable<PotentialMove> moves, State solvable) {
            this.moves = moves.ToList();
            this.state = solvable;
        }

        public static SolveResult Solvable(IEnumerable<PotentialMove> moves) {
            return new SolveResult(moves, State.Solvable);
        }
        public static SolveResult Cancelled(IEnumerable<PotentialMove> moves) {
            return new SolveResult(moves, State.Cancelled);
        }
        public static readonly SolveResult BackTracked =
                new SolveResult(new List<PotentialMove>(), State.BackTracked);

        public override string ToString() {
            return "state: " + state.ToString() + ", moves: " + string.Join(", ", moves.Select( m => m.ToString()).ToArray());
        }
    }

    /// <summary>
    /// Finds solutions to the puzzles. not very good yet...
    /// </summary>
    public class Solver {
        readonly Game game;

        int currentIterations;
        int maxIterations;
        int maxMoves;

        public int MaxIterations {
            get { return maxIterations; }
        }
        public int CurrentIterations {
            get { return currentIterations; }
        }

        public Solver(Game game) {
            this.game = game.Clone();
        }

        public SolveResult Solve(int maxIterations = int.MaxValue, int maxMoves = 100) {
            this.maxIterations = maxIterations;
            this.maxMoves = maxMoves;
            return Solve(game, new List<PotentialMove>());
        }

        SolveResult Solve(Game game, List<PotentialMove> moveHistory) {
            if (game.IsSolved)
                return SolveResult.Solvable(moveHistory);

            if (++currentIterations > maxIterations)
                return SolveResult.Cancelled(moveHistory);

            if (moveHistory.Count >= maxMoves)
                return SolveResult.BackTracked;

            var moves = AvailableMoves(game);
            foreach (var move in moves) {
                game.Execute(move);

                var result = Solve(game, Append(moveHistory, move));
                if (result.Finished)
                    return result;
                else
                    game.Undo();
            }
            return SolveResult.BackTracked;
        }

        List<PotentialMove> Append(List<PotentialMove> moves, PotentialMove move) {
            return moves.Concat(new List<PotentialMove>() { move }).ToList();
        }

        List<PotentialMove> AvailableMoves(Game game) {
            return new PlayLogic(game.Board).AvailableMoves().ToList();
        }

    }
}