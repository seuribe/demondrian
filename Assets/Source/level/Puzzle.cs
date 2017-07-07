using com.perroelectrico.demondrian.core;
using SimpleJSON;

namespace com.perroelectrico.demondrian.level {

    /// <summary>
    /// A puzzle consists of a board and a set of rules
    /// </summary>
    public class Puzzle {
        public Board Board { get; private set; }
        // Todo: piece types should actually be in the rules
        public PieceType[] Types { get; private set; }
        public GameRules Rules { get; private set; }

        public static Puzzle FromJSON(JSONNode json) {
            return new Puzzle {
                Board = Board.FromJSON(json["board"]),
                Rules = GameRules.FromJSON(json["rules"]),
                Types = PieceType.FromJSONArray(json["types"].AsArray)
            };
        }

        public static Puzzle[] FromJSONArray(JSONArray array) {
            var puzzles = new Puzzle[array.Count];
            for (int i = 0 ; i < array.Count ; i++)
                puzzles[i] = Puzzle.FromJSON(array[i]);
            return puzzles;
        }

        public override string ToString() {
            return Board.ToString() + ", " + Types + ", " + Rules.ToString();
        }

        public Game NewGame() {
            return new Game(Board.Clone(), Rules, Types);
        }
    }
}