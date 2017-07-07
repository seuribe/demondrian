using SimpleJSON;
using System.Collections.Generic;
using System.Collections;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// A 2x2 'square' of pieces on the board: used for compaction
    /// </summary>
    public struct PiecesSquare : IEnumerable<Piece> {
        public Piece p00;
        public Piece p10;
        public Piece p01;
        public Piece p11;

        public bool AllMatch() {
            return p00.Matches(p10) && p00.Matches(p01) && p00.Matches(p11);
        }

        public JSONNode ToJSON() {
            var json = new JSONClass();
            json["p00"] = p00.ToJSON();
            json["p10"] = p10.ToJSON();
            json["p01"] = p01.ToJSON();
            json["p11"] = p11.ToJSON();
            return json;
        }

        public static PiecesSquare FromJSON(JSONNode json) {
            return new PiecesSquare {
                p00 = Piece.FromJSON(json["p00"]),
                p10 = Piece.FromJSON(json["p10"]),
                p01 = Piece.FromJSON(json["p01"]),
                p11 = Piece.FromJSON(json["p11"])
            };
        }

        IEnumerator<Piece> IEnumerable<Piece>.GetEnumerator() {
            yield return p00;
            yield return p10;
            yield return p01;
            yield return p11;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            yield return p00;
            yield return p10;
            yield return p01;
            yield return p11;
        }
    }
}
