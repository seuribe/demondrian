using System;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

namespace com.perroelectrico.demondrian.level {

    [Serializable]
    public struct PuzzleRef {
        public readonly int levelIndex;
        public readonly int puzzleIndex;

        public PuzzleRef(int levelIndex, int puzzleIndex) {
            this.levelIndex = levelIndex;
            this.puzzleIndex = puzzleIndex;
        }
    }

    /// <summary>
    /// A collection of levels
    /// </summary>
    public class LevelRepository : Browseable<Level> {

        private Level[] levels;

        private static object instanceLock = new object();
        private static LevelRepository instance;
        public static LevelRepository Instance {
            get {
                lock (instanceLock) {
                    if (instance == null)
                        instance = new LevelRepository();
                    return instance;
                }
            }
        }
        private LevelRepository() { }

        public Level this[int index] {
            get {
                return levels[index];
            }
        }

        public void Load(string levelsDef) {
            var levelsJSON = JSON.Parse(levelsDef).AsArray;
            levels = new Level[levelsJSON.Count];
            for (int i = 0 ; i < levelsJSON.Count ; i++)
                levels[i] = Level.FromJSON(levelsJSON[i]);
        }

        public Level[] Levels {
            get { return levels; }
        }

        public int Count {
            get { return (levels == null) ? 0 : levels.Length; }
        }

        public Puzzle GetPuzzle(PuzzleRef pref) {
            return levels[pref.levelIndex][pref.puzzleIndex];
        }

        public bool PuzzleUnlocked(Puzzle puzzle) {
            return true;
        }

        public IEnumerator<Level> GetEnumerator() {
            return new List<Level>(levels).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
