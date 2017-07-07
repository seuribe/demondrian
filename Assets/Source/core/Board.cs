using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    public enum RotationDir {
        Right = -90,
        Left = 90
    };

    public class Board {

        private Dictionary<Piece, Coord> allPieces;
        private Piece[,] pieces;
        private int size;
        private Orientation orientation = Orientation.Top;

        public Orientation Orientation {
            get { return orientation; }
        }

        // TODO: return just pieces, have another accessor for piece/coord pairs
        public Dictionary<Piece, Coord> Pieces {
            get { return allPieces; }
        }

        public int Size {
            get { return size; }
        }

        public Board(int size) {
            if (size <= 0)
                throw new InvalidBoardStateException("Board size must be > 0");

            this.size = size;
            this.allPieces = new Dictionary<Piece, Coord>();
            this.pieces = new Piece[size, size];
        }

        public void Reset() {
            Clear();
            orientation = Orientation.Top;
        }

        public void Clear() {
            allPieces.Clear();
            foreach (var c in AllCoords)
                pieces[c.row, c.col] = null;
        }

        public void Remove(Piece piece) {
            foreach (var coord in PieceCoords(piece))
                pieces[coord.row, coord.col] = null;

            allPieces.Remove(piece);
        }

        public Piece Remove(Coord c) {
            var piece = Get(c);
            Remove(piece);
            return piece;
        }

        public IEnumerable<Coord> PieceCoords(Piece piece) {
            foreach (var coord in Coord.SquareRange(Where(piece), piece.size))
                yield return coord;
        }

        public bool OutOfRange(Coord c) {
            return c.col < 0 || c.row < 0 || c.col >= size || c.row >= size;
        }

        public bool AllEmpty(IEnumerable<Coord> coords) {
            return coords.All( c => IsEmpty(c));
        }

        public bool AnyEmpty(IEnumerable<Coord> coords) {
            return coords.Any( c => IsEmpty(c));
        }

        public bool IsEmpty(Coord coord, int size = 1) {
            return (size == 1) ? // Base case is used often, so optimize it
                Get(coord) == null :
                AllEmpty(Coord.SquareRange(coord, size));
        }

        public static void ThrowInvalidOp(string msg, params object[] values) {
            throw new InvalidBoardOperationException(
                string.Format(msg, values)
            );
        }

        Coord LastCoord(Coord c, Piece piece) {
            return c.Move(piece.size - 1, piece.size - 1);
        }

        public bool ContainsPiece(Piece piece) {
            return allPieces.ContainsKey(piece);
        }

        public bool CanSet(Coord c, Piece piece) {
            return !OutOfRange(c) &&
                    !OutOfRange(LastCoord(c, piece)) &&
                    !ContainsPiece(piece) &&
                    IsEmpty(c, piece.size);
        }

        public void Set(Coord coord, Piece piece) {
            if (OutOfRange(coord) || OutOfRange(LastCoord(coord, piece)))
                ThrowInvalidOp("Cannot set piece {0} in {1}: Out of Range", piece, coord);

            if (ContainsPiece(piece))
                ThrowInvalidOp("Piece {0} is already in the board", piece);

            if (!IsEmpty(coord, piece.size))
                ThrowInvalidOp("Piece {0} cannot be set in occupied space {1}", piece, coord);

            allPieces[piece] = coord;
            foreach (var c in Coord.SquareRange(coord, piece.size))
                pieces[c.row, c.col] = piece;
        }

        public Coord Where(Piece piece) {
            if (!ContainsPiece(piece))
                ThrowInvalidOp("Piece {0} is not in the board", piece);

            return allPieces[piece];
        }

        private static Orientation[] RightOf = new Orientation[] { Orientation.Right, Orientation.Bottom, Orientation.Left, Orientation.Top };
        private static Orientation[] LeftOf = new Orientation[] { Orientation.Left, Orientation.Top, Orientation.Right, Orientation.Bottom };

        public void ReplacePieces(Dictionary<Piece, Coord> newPieces) {
            Clear();
            foreach (var p_c in newPieces)
                Set(p_c.Value, p_c.Key);
        }

        void Transform(Func<Coord, Coord> coordTransform) {
            Dictionary<Piece, Coord> newPieces = new Dictionary<Piece, Coord>();
            allPieces.Keys.ToList<Piece>().ForEach(
                    p => {
                        var oldCoord = Where(p);
                        var newCoord = coordTransform(oldCoord);
                        newPieces[p] = newCoord;
                    });
            ReplacePieces(newPieces);
        }

        void Transform(Func<Coord, Piece, Coord> coordPieceTransform) {
            Dictionary<Piece, Coord> newPieces = new Dictionary<Piece, Coord>();
            allPieces.Keys.ToList<Piece>().ForEach(
                    p => {
                        var oldCoord = Where(p);
                        var newCoord = coordPieceTransform(oldCoord, p);
                        newPieces[p] = newCoord;
                    });
            ReplacePieces(newPieces);
        }

        public void RotateTo(Orientation dest) {
            foreach (var act in RotateAction.GenerateActions(Orientation, dest)) {
                if (act == RotateAction.Left) {
                    RotateLeft();
                } else {
                    RotateRight();
                }
            }
        }

        public void RotateLeft() {
            Transform((c, p) => AdjustForRotateLeft(RotateCoordLeft(c), p));
            orientation = LeftOf[(int)orientation];
        }

        public void RotateRight() {
            Transform((c, p) => AdjustForRotateRight(RotateCoordRight(c), p));
            orientation = RightOf[(int)orientation];
        }

        public Coord AdjustForRotateLeft(Coord c, Piece p) {
            return c.Move(1 - p.size, 0);
        }

        public Coord AdjustForRotateRight(Coord c, Piece p) {
            return c.Move(0, 1 - p.size);
        }

        public Coord RotateCoordLeft(Coord c) {
            return new Coord { row = c.col, col = size - c.row - 1 };
        }

        public Coord RotateCoordRight(Coord c) {
            return new Coord { row = size - c.col - 1, col = c.row };
        }

        public Coord FlipCoord(Coord c) {
            return new Coord { row = size - c.row - 1, col = size - c.col - 1};
        }

        /// <summary>
        /// transports a coord specified in the original orientation to the current orientation of the board
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="original"></param>
        /// <returns></returns>
        public Coord TransportCoord(Coord coord, Orientation original) {
            int moveIndex = ((int)orientation - (int)original + 4) % 4;
            switch (moveIndex) {
                case 0: return coord;
                case 1: return RotateCoordRight(coord);
                case 2: return FlipCoord(coord);
                case 3: return RotateCoordLeft(coord);
            }
            ThrowInvalidOp("Invalid orientation");
            // Stupid compiler cannot see that this throws an exception, so it must return something
            return coord;
        }

        public IEnumerable<Piece> GetAll(params Coord[] coords) {
            return coords.Select( c => Get(c) );
        }

        public Piece Get(Coord c) {
            if (OutOfRange(c))
                throw new InvalidBoardOperationException(string.Format("Coord {0} out of range", c));

            return pieces[c.row, c.col];
        }

        public bool IsEmpty() {
            return allPieces.Count == 0;
        }

        public bool IsFull() {
            return AllCoords.ToList<Coord>().All( c => !IsEmpty(c));
        }

        public IEnumerable<Coord> AllCoords {
            get { return Coord.SquareRange(Coord.c00, size); }
        }

        public bool Matches(Board other) {
            return size == other.size &&
                AllCoords.ToList<Coord>().All( c =>
                    (IsEmpty(c) && other.IsEmpty(c)) ||
                    Get(c).Matches(other.Get(c)));
        }

        public bool IsMatchingNeighbour(Piece a, Piece b) {
            if (!a.Matches(b))
                return false;

            var ca = Where(a);
            var cb = Where(b);
            return (Math.Abs(ca.col - cb.col) == a.size && ca.row == cb.row) ||
                    (Math.Abs(ca.row - cb.row) == a.size  && ca.col == cb.col);
        }

        public List<Coord> GetNeighbouringCoords(Piece piece) {
            var coord = Where(piece);
            return new List<Coord> {
                coord.Move(0, -piece.size),
                coord.Move(0, piece.size),
                coord.Move(-piece.size, 0),
                coord.Move(piece.size, 0) }
            .Where( c => !OutOfRange(c)).ToList<Coord>();
        }

        public List<Piece> GetMatchingNeighbours(Piece piece) {
            if (!ContainsPiece(piece))
                throw new InvalidBoardOperationException("Piece not in board");

            var nCoords = GetNeighbouringCoords(piece);

            var nPieces = nCoords.Select<Coord, Piece>( c => Get(c));

            return nPieces.Where( p => IsMatchingNeighbour(p, piece))
                          .ToList<Piece>();
        }

        public PiecesSquare PiecesSquare(Coord coord) {
            var piece00 = Get(coord);
            return new PiecesSquare {
                p00 = piece00,
                p10 = Get(coord.Move(piece00.size, 0)),
                p01 = Get(coord.Move(0, piece00.size)),
                p11 = Get(coord.Move(piece00.size, piece00.size))
            };
        }

        public string TypeGrid() {
            var sb = new StringBuilder();
            for (int row = size-1 ; row >= 0 ; row--) {
                for (int col = 0 ; col < size ; col++) {
                    var piece = pieces[row, col];
                    sb.Append(piece != null ? piece.type.index.ToString() : ".");
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            for (int row = size-1 ; row >= 0 ; row--) {
                for (int col = 0 ; col < size ; col++) {
                    var piece = pieces[row, col];
                    sb.Append(piece != null ? piece.ToString() : "[.]").Append(' ');
                }
                sb.Append('\n');
            }
            sb.Append("orientation: " + orientation + "\n");
            return sb.ToString();
        }

        public JSONNode ToJSON() {
            var json = new JSONClass();
            json["size"] = new JSONData(size);
            var rows = new JSONArray();
            for (int row = 0 ; row < size ; row++) {
                var pcs = new JSONArray();
                for (int col = 0 ; col < size ; col++) {
                    pcs[col] = pieces[(size - row - 1), col].ToJSON();
                }
                rows[row] = pcs;
            }
            json["rows"] = rows;
            return json;
        }

        public static Board FromJSON(JSONNode json) {
            var builder = new BoardBuilder();
            if (json["rows"] != null) {
                return builder.FromJSONRows(json["rows"].AsArray);
            } else if (json["pointers"] != null) {
                return builder.FromJSONPointers(json["pointers"].AsArray, json["size"].AsInt);
            } else if (json["compact"] != null) {
                return builder.FromTypeGrid(json["compact"]);
            }
            return new Board(0);
        }


        public string SaveToString() {
            var sb = new StringBuilder();
            sb.Append("[B").Append(size).Append('\n');
            for (int row = size - 1 ; row >= 0 ; row--) {
                for (int col = 0 ; col < size ; col++) {
                    sb.Append(pieces[row, col].SaveToString()).Append(" ");
                }
                if (row > 0) {
                    sb.Append("\n");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        public bool IsAbove(Piece above, Piece below) {
            var aCoord = Where(above);
            var bCoord = Where(below);
            return bCoord.Move(0, below.size).Matches(aCoord);
        }

        public bool IsRightOf(Piece right, Piece left) {
            var rCoord = Where(right);
            var lCoord = Where(left);
            return lCoord.Move(left.size, 0).Matches(rCoord);
        }

        public Board Clone() {
            Board other = new Board(size);
            other.ReplacePieces(allPieces);
            other.orientation = orientation;
            return other;
        }

        public void Fill(PieceType t) {
            foreach (var coord in AllCoords)
                Set(coord, new Piece(t, 1));
        }

        public int CoordOrder(Coord c) {
            return c.col + (c.row * size);
        }
    }
}