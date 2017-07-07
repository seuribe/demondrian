using System;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    public enum Orientation {
        Top,
        Right,
        Bottom,
        Left
    };

    public class Game {
        private Board board;
        private PieceType[] types;

        private Stack<Move> moves;
        private IncomingQueue incoming;

        public GameRules Rules { get; set; }

        public IncomingQueue Incoming {
            get { return incoming; }
        }

        public bool IsSolved {
            get {
                return !board.IsEmpty(Coord.c00) && board.Get(Coord.c00).size == board.Size;
            }
        }
        
        public Move LastMove {
            get { return moves.Peek(); }
        }

        public List<Move> History {
            get {
                List<Move> ret = new List<Move>(moves);
                ret.Reverse();
                return ret;
            }
        }

        public int NumMoves {
            get { return moves.Count; }
        }

        public Orientation Orientation {
            get { return board.Orientation; }
        }

        public Board Board {
            get { return board; }
            set { this.board = value; }
        }

        public PieceType[] Types {
            get { return types; }
        }

        public Piece NextPieceIfPlaying(Coord coord) {
            var piece = board.Get(coord);
            if (Rules.NextTypePolicy == NextTypePolicy.RemovedType)
                return piece;

            return new Piece(incoming.PeekNext(), piece.size);
        }

        public Piece NextPieceIfPlaying(PotentialMove move) {
            var playedCoord = board.TransportCoord(move.coord, move.orientation);
            return NextPieceIfPlaying(playedCoord);
        }

        public Game(Board board, GameRules rules, PieceType[] types) {
            if (types == null || types.Length == 0)
                throw new InvalidGameStateException("Empty or null types");
            if (board == null)
                throw new InvalidGameStateException("Empty or null board");

            this.Rules = rules;
            this.board = board;
            this.types = types;
            this.incoming = IncomingQueue.GetQueue(rules, types);
            this.moves = new Stack<Move>();
        }

        public Game(int size, GameRules rules, PieceType[] types) : this(new Board(size), rules, types) { }

        public Game Clone() {
            return new Game(board.Clone(), Rules, types);
        }

        public void Reset() {
            moves.Clear();
            board.Reset();
            incoming.Reset();
        }

        public void Execute(PotentialMove move) {
            var simulator = MoveSimulator.FromPotentialMove(board, move, NextPieceIfPlaying(move));
            simulator.Simulate();
            var executedMove = simulator.GetMove();
            Execute(executedMove);
        }

        void Execute(Move move, Action<BoardAction> doAfterAction = null) {
            foreach (var action in move.consequences) {
                Execute(action);
                if (doAfterAction != null)
                    doAfterAction(action);
            }
            ExecuteEndMove(move);
        }

        public void Execute(BoardAction action) {
            action.Do(board);
        }

        public void ExecuteEndMove(Move move) {
            moves.Push(move);
            incoming.Next();
        }

        public static void ThrowInvalidOp(string msg, params object[] values) {
            throw new InvalidGameOperationException(
                string.Format(msg, values)
            );
        }

        public bool CanUndo() {
            return moves.Count > 0 && Rules.AllowUndo;
        }

        public void Undo() {
            var move = moves.Pop();
            var actions = new List<BoardAction>(move.consequences);
            actions.Reverse();

            foreach (var action in actions)
                action.Undo(board);

            incoming.Undo();
        }

        public void RotateTo(Orientation dest) {
            foreach (var act in RotateAction.GenerateActions(Orientation, dest))
                Rotate(act.dir);
        }

        public void Rotate(RotationDir dir) {
            if (dir == RotationDir.Left)
                board.RotateLeft();
            else
                board.RotateRight();
        }

        public JSONNode ToJSON() {
            var json = new JSONClass();
            json["types"] = PieceType.ToJSONArray(types);
            json["board"] = board.ToJSON();
            json["rules"] = Rules.ToJSON();
            var arr = new JSONArray();
            foreach (var move in moves) {
                arr.Add(move.ToJSON());
            }
            json["moves"] = arr;
            return json;
        }

        public static Game FromJSON(JSONNode json) {
            var types = PieceType.FromJSONArray(json["numTypes"].AsArray);
            var board = Board.FromJSON(json["board"]);
            var rules = GameRules.FromJSON(json["rules"]);
            var game = new Game(board, rules, types);
            foreach (JSONNode js in json["moves"].AsArray) {
                game.moves.Push(Move.FromJSON(js));
            }
            return game;
        }

        public string SaveToString() {
            var sb = new StringBuilder();

            sb.Append("[G")
                .Append(board.SaveToString())
                .Append("]");

            return sb.ToString();
        }

        public bool Matches(Game other) {
            if (!board.Matches(other.board)) {
                return false;
            }
            // TODO: compare move and history stack -- Seu
            if (moves.Count != other.moves.Count) {
                return false;
            }
            // TODO: compare incoming

            return true;
        }
    }
}