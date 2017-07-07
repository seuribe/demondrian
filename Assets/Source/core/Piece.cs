using System.Linq;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    public class Piece {
        public readonly int size;
        public readonly PieceType type;

        public Piece(PieceType type, int size) {
            System.Diagnostics.Debug.Assert(type != null, "type cannot be null in piece!");
            this.type = type;
            this.size = size;
        }

        public override string ToString() {
            return string.Format("[P{0}:{1}]", type, size);
        }

        public bool Matches(Piece other) {
            return other != null && size == other.size && type.Matches(other.type);
        }

        public Piece DoubleSize() {
            return new Piece(type, size * 2);
        }

        public JSONNode ToJSON() {
            var json = new JSONClass();
            json["size"] = new JSONData(size);
            json["type"] = type.ToJSON();
            return json;
        }

        public static Piece FromJSON(JSONNode json) {
            var size = json["size"].AsInt;
            var type = PieceType.FromJSON(json["type"]);
            return new Piece(type, size);
        }

        public string SaveToString() {
            return "[P" + type.SaveToString() + ":" + size + "]";
        }

        public static Piece RestoreFromString(string str) {
            if (!str.StartsWith("[P") || !str.EndsWith("]")) {
                return null;
            }
            var parts = str.Substring(2, str.Length - 3).Split(':');
            var type = PieceType.RestoreFromString(parts[0]);
            var size = int.Parse(parts[1]);
            return new Piece(type, size);
        }

        public override bool Equals(object obj) {
            return this == obj;
        }

        public override int GetHashCode() {
            unchecked {
                return size * 13 + type.index;
            }
        }

        public Piece Clone() {
            return new Piece(type, size);
        }

        public static bool AllMatch(params Piece[] pieces) {
            return pieces.All( p => p.Matches(pieces[0]));
        }
    }
}