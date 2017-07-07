using System.Collections.Generic;
using System.Linq;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// Executes compaction operation on boards
    /// </summary>
    public class BoardCompactor {
        Board board;

        public BoardCompactor(Board board) {
            this.board = board;
        }

        public void CompactNext() {
            var coord = NextCompactable();
            if (coord.Valid)
                CompactCoord(coord);
        }

        public void Compact() {
            Piece bigPiece;
            PiecesSquare removed;
            do {
                bigPiece = null;
                var coord = NextCompactable();
                if (coord.Valid)
                    Replace(coord, out bigPiece, out removed);
            } while (bigPiece != null);
        }

        public bool CanCompact() {
            return NextCompactable().Valid;
        }

        public Coord NextCompactable() {
            foreach (var coord in board.AllCoords)
                if (CanCompact(coord))
                    return coord;

            return Coord.Invalid;
        }

        public bool CanCompact(Coord coord) {
            var piece00 = board.Get(coord);
            var coords = coord.SquareCoords(piece00.size);

            foreach (var c in coords) {
                if (board.OutOfRange(c))
                    return false;

                var piece = board.Get(c);
                if (!board.Where(piece).Matches(c))
                    return false;

                if (!piece.Matches(piece00))
                    return false;
            }
            return true;
        }

        public void CompactCoord(Coord coord) {
            Piece bigPiece;
            PiecesSquare removed;
            Replace(coord, out bigPiece, out removed);
        }

        private void Replace(Coord coord, out Piece bigPiece, out PiecesSquare removed) {
            removed = board.PiecesSquare(coord);
            bigPiece = removed.p00.DoubleSize();

            foreach (var piece in removed)
                board.Remove(piece);

            board.Set(coord, bigPiece);
        }

        public bool TryCompact(Coord coord, PieceType newType, out Piece bigPiece, out List<Piece> removed) {
            var piece00 = board.Get(coord);
            removed = new List<Piece>();
            bigPiece = new Piece(newType, piece00.size * 2);

            if (piece00 == null)
                return false;

            removed.AddRange(new Piece[] {
                    piece00,
                    board.Get(coord.Move(piece00.size, 0)),
                    board.Get(coord.Move(0, piece00.size)),
                    board.Get(coord.Move(piece00.size, piece00.size)) } );

            if (!removed.All( p => p.Matches(piece00)))
                return false;

            removed.ForEach( p => board.Remove(p));
            board.Set(coord, bigPiece);

            return true;
        }
    }
}