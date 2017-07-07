using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace com.perroelectrico.demondrian.level {

    /// <summary>
    /// A level is a collection (browseable) of puzzles
    /// </summary>
    public class Level : Browseable<Puzzle> {

        Puzzle[] puzzles;

        public string Id { get; private set; }

        public Puzzle this[int i] {
            get { return puzzles[i]; }
        }

        public int Count {
            get { return puzzles.Length; }
        }

        public static Level FromJSON(JSONNode json) {
            return new Level {
                Id = json["id"].ToString(),
                puzzles = Puzzle.FromJSONArray(json["puzzles"].AsArray)
            };
        }

        public IEnumerator<Puzzle> GetEnumerator() {
            return new List<Puzzle>(puzzles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
