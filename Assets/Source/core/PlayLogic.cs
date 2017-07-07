using System;
using System.Collections.Generic;
using System.Linq;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// Functionality that deals with potential moves, characteristics of the pieces on the board, etc.
    /// </summary>
    public class PlayLogic {

        public Board board;

        public PlayLogic(Board board) {
            this.board = board;
        }

        public bool IsValidMove(Coord c) {
            return IsAlone(c) && GetObstaclesAbove(board.Get(c)).Count() == 0;
        }

        public bool IsValidMove(Piece piece) {
            return IsValidMove(board.Where(piece));
        }

        public bool IsAlone(Coord c) {
            var piece = board.Get(c);
            return piece != null &&
            board.GetMatchingNeighbours(piece).Count == 0;
        }

        public ICollection<Piece> GetAllObstacles(Coord coord) {
            var piece = board.Get(coord);
            var obstacles = new HashSet<Piece>();
            obstacles.UnionWith(GetObstaclesAbove(piece));
            obstacles.UnionWith(GetBlockingNeighbors(piece));
            return obstacles;
        }

        public List<Piece> GetBlockingNeighbors(Piece piece) {
            return board.GetMatchingNeighbours(piece);
        }

        public IEnumerable<PotentialMove> AvailableMoves() {
            if (!board.IsFull())
                Board.ThrowInvalidOp("Board must be full");

            var clonedBoard = board.Clone();
            for (int i = 0 ; i < 4 ; i++) {
                var subLogic = new PlayLogic(clonedBoard);
                foreach (var move in subLogic.UnrotatedMoves())
                    yield return move;

                clonedBoard.RotateRight();
            }
        }

        IEnumerable<PotentialMove> UnrotatedMoves() {
            foreach (var pieceCoord in board.Pieces.Values) {
                if (IsValidMove(pieceCoord))
                    yield return new PotentialMove(pieceCoord, board.Orientation);
            }
        }

        public bool HasAvailableMoves() {
            return AvailableMoves().Count() > 0;
        }

        private bool IsStrictlyAbove(Piece above, Coord coord) {
            var aCoord = board.Where(above);
            return aCoord.row > coord.row;
        }

        private bool CrossesVerticalLine(Piece piece, int column) {
            var coord = board.Where(piece);
            return coord.col < column && (coord.col + piece.size) > column;
        }

        List<Piece> GetPiecesMatching(Func<Piece, bool> predicate) {
            return new List<Piece>(board.Pieces.Keys.Where(predicate));
        }

        public ICollection<Piece> GetObstaclesAbove(Piece piece) {
            var coord = board.Where(piece);
            return GetPiecesMatching( (otherPiece) =>
                (otherPiece.size >= 2 && IsStrictlyAbove(otherPiece, coord) &&
                (CrossesVerticalLine(otherPiece, coord.col) ||
                CrossesVerticalLine(otherPiece, coord.col + piece.size))));
        }

        bool InsidePieceColumns(Piece testPiece, Piece columnsPiece) {
            var testCoord = board.Where(testPiece);
            var columnsCoord = board.Where(columnsPiece);
            return testCoord.col >= columnsCoord.col &&
                testCoord.col + testPiece.size <= columnsCoord.col + columnsPiece.size;
        }

        int LowerFirst(Piece lhs, Piece rhs) {
            var lhsRow = board.Where(lhs).row;
            var rhsRow = board.Where(rhs).row;
            return lhsRow.CompareTo(rhsRow);
        }

        public ICollection<Piece> GetDroppablesAbove(Piece piece) {
            var pieceCoord = board.Where(piece);
            var matching = GetPiecesMatching( (otherPiece) =>
                (IsStrictlyAbove(otherPiece, pieceCoord) &&
                InsidePieceColumns(otherPiece, piece))
            );
            matching.Sort(LowerFirst);
            return matching;
        }

    }
}