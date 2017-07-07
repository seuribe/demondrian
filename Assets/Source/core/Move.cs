using System;
using System.Collections.Generic;
using SimpleJSON;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// A possible move to be played
    /// </summary>
    public struct PotentialMove {
        public readonly Coord coord;
        public readonly Orientation orientation;

        public PotentialMove(Coord coord, Orientation orientation) {
            this.coord = coord;
            this.orientation = orientation;
        }

        internal bool Matches(PotentialMove other) {
            return coord.Matches(other.coord) && orientation == other.orientation;
        }

        public override string ToString() {
            return "[PotentialMove @ " + coord.ToString() + ", rot: " + Enum.GetName(typeof(Orientation), orientation) + "]";
        }
    }

    /// <summary>
    /// A move that has actually been played (and is stored in memory)
    /// </summary>
    public class Move {
        public readonly List<BoardAction> consequences;

        public Move(IEnumerable<BoardAction> actions) {
            this.consequences = new List<BoardAction>(actions);
        }

        public JSONNode ToJSON() {
            var json = new JSONClass();
            var actions = new JSONArray();
            if (consequences != null) {
                foreach (var ga in consequences) {
                    actions.Add(ga.ToJSON());
                }
            }
            json["actions"] = actions;
            return json;
        }

        public static Move FromJSON(JSONNode json) {
            var actions = new List<BoardAction>();
            var arr = json["actions"].AsArray;
            foreach (JSONNode js in arr)
                actions.Add(BoardAction.FromJSON(js));

            return new Move(actions);
        }
    }
}