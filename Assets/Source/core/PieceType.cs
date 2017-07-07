using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    public class PieceType {
        public readonly int index;
        private const int NUM_BASICS = 16;
        private static PieceType[] basics = new PieceType[NUM_BASICS];

        static PieceType() {
            for (int i = 0; i < NUM_BASICS; i++) {
                basics[i] = new PieceType(i);
            }
        }

        public static readonly PieceType MatchAll = new PieceType(1000);
        public static readonly PieceType MatchNone = new PieceType(1001);

        public static PieceType[] GetRange(int from, int count) {
            var types = new PieceType[count];
            for (int i = 0 ; i < count ; i++)
                types[i] = Get(i + from);
            return types;
        }

        public static PieceType Get(int i) {
            if (i >= NUM_BASICS) {
                return new PieceType(i);
            }
            return basics[i];
        }

        public PieceType(int index) {
            this.index = index;
        }

        public bool Matches(PieceType other) {
            return (this != MatchNone && other != MatchNone) &&
            ((this == MatchAll || other == MatchAll) || (index == other.index));
        }

        public override bool Equals(object obj) {
            return (obj is PieceType) && ((PieceType)obj).index == index;
        }

        public override int GetHashCode() {
            return index.GetHashCode();
        }

        public override string ToString() {
            return string.Format("[t{0}]", index);
        }

        public JSONNode ToJSON() {
            var json = new JSONClass();
            json["index"] = new JSONData(index);
            return json;
        }

        public static PieceType FromJSON(JSONNode json) {
            if (json["index"] == null) {
                return null;
            }
            return Get(json["index"].AsInt);
        }

        public string SaveToString() {
            string typeId = (this == MatchAll) ? "A" :
            (this == MatchNone) ? "N" :
            ("" + index);
            return "[T" + typeId + "]";
        }

        public static PieceType RestoreFromString(string str) {
            if (!str.StartsWith("[T") || !str.EndsWith("]")) {
                return null;
            }
            var value = str.Substring(2, str.Length - 3);
            if (value.Equals("A")) {
                return MatchAll;
            } else if (value.Equals("N")) {
                return MatchNone;
            } else {
                return new PieceType(int.Parse(value));
            }
        }

        public static PieceType[] FromJSONArray(JSONArray jsonArray) {
            var ret = new PieceType[jsonArray.Count];
            for (int i = 0 ; i < ret.Length ; i++)
                ret[i] = FromJSON(jsonArray[i]);

            return ret;
        }

        public static JSONArray ToJSONArray(PieceType[] types) {
            var array = new JSONArray();
            foreach (var type in types)
                array.Add(type.ToJSON());

            return array;
        }

    }
}