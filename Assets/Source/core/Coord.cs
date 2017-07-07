using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    [Serializable]
    public struct Coord : IComparable<Coord> {
        public int col;
        public int row;

        public bool Valid {
            get {
                return !Equals(Invalid);
            }
        }

        public static readonly Coord Invalid = new Coord(-1, -1);
        public static readonly Coord c00 = new Coord(0, 0);

        public Coord(int col, int row) {
            this.col = col;
            this.row = row;
        }

        public Coord(Coord other) {
            this.col = other.col;
            this.row = other.row;
        }

        public Coord Up() {
            return new Coord(col, row + 1);
        }

        public Coord Down() {
            return new Coord(col, row - 1);
        }

        public Coord Move(int cols, int rows) {
            return new Coord(col + cols, row + rows);
        }

        public Coord[] Neighbors() {
            return new Coord[] {
                new Coord(col, row - 1),
                new Coord(col, row + 1),
                new Coord(col - 1, row),
                new Coord(col + 1, row)
            };
        }

        public override string ToString() {
            return string.Format("[col {0}, row {1}]", col, row);
        }

        public override bool Equals(object obj) {
            return (obj is Coord) && ((Coord)obj).col == col && ((Coord)obj).row == row;
        }

        public override int GetHashCode() {
            unchecked {
                return ((13 * col) + row) * 7;
            }
        }

        public bool Matches(Coord prev) {
            return col == prev.col && row == prev.row;
        }

        public IEnumerable<Coord> SquareCoords(int size) {
            yield return this;
            yield return Move(size, 0);
            yield return Move(0, size);
            yield return Move(size, size);
        }

        /// <summary>
        /// [bottomLeft, topRight)
        /// inclusive, exclusive
        /// </summary>
        public static IEnumerable<Coord> Range(Coord bottomLeft, Coord topRight) {
            for (int y = bottomLeft.row ; y < topRight.row ; y++)
                for (int x = bottomLeft.col ; x < topRight.col ; x++)
                    yield return new Coord(x, y);
        }

        public static IEnumerable<Coord> SquareRange(Coord origin, int size) {
            for (int y = 0 ; y < size ; y++)
                for (int x = 0 ; x < size ; x++)
                    yield return new Coord {
                        col = origin.col + x,
                        row = origin.row + y };
        }

        public Coord Crop(int maxCols, int maxRows) {
            if (col > 0 && row > 0 && col < maxCols && row < maxRows) {
                return this;
            }
            var newCol = col < 0 ? 0 :
                col >= maxCols ? (maxCols - 1) : col;
            int newRow = row < 0 ? 0 :
                row >= maxRows ? (maxRows - 1) : row;

            return new Coord(newCol, newRow);
        }

        public JSONNode ToJSON() {
            var json = new JSONClass();
            json["col"] = new JSONData(col);
            json["row"] = new JSONData(row);
            return json;
        }

        public static Coord FromJSON(JSONNode json) {
            return new Coord(json["col"].AsInt, json["row"].AsInt);
        }

        public string SaveToString() {
            return "[C" + col + "," + row + "]";
        }

        public static Coord RestoreFromString(string str) {
            if (!str.StartsWith("[C") || !str.EndsWith("]")) {
                return Coord.Invalid;
            }
            var parts = str.Substring(2, str.Length - 3).Split(',');
            var col = int.Parse(parts[0]);
            var row = int.Parse(parts[1]);
            return new Coord(col, row);
        }

        public int CompareTo(Coord other) {
            int rowCompare = row.CompareTo(other.row);
            return rowCompare == 0 ? col.CompareTo(other.col) : rowCompare;
        }
    }
}