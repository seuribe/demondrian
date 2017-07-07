using System.Collections.Generic;
using com.perroelectrico.demondrian.controller.scene;
using com.perroelectrico.demondrian.level;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    public delegate void SessionEvent();

    /// <summary>
    /// Manages a play session (going from one board to another inside the 'play' scene)
    /// </summary>
    public class SessionController : MonoBehaviour {

        [Range(0, 2)]
        public float nextPuzzleDelay = 0.5f;
        public TextAsset levelsAsset;
        public int levelIndex;

        public event SessionEvent SessionStarted;
        public event SessionEvent NoMorePuzzles;

        GameController gameController;
        IEnumerator<Puzzle> puzzleEnumerator;

        void Awake() {
            Debug.LogFormat("Awake");

            gameController = FindObjectOfType<GameController>();
            gameController.GameSolved += game => {
                Invoke("NextPuzzle", nextPuzzleDelay);
            };
        }

        void LoadLevel() {
            Debug.LogFormat("LoadLevel");
            if (puzzleEnumerator == null) {
                var lm = LevelRepository.Instance;
                lm.Load(levelsAsset.text);
                SetPuzzles(lm[levelIndex]);
            }
        }

        public void SetPuzzles(Browseable<Puzzle> puzzles) {
            Debug.LogFormat("SetPuzzles {0}", puzzles);
            puzzleEnumerator = puzzles.GetEnumerator();
        }

        void NextPuzzle() {
            Debug.LogFormat("NextPuzzle");
            if (puzzleEnumerator.MoveNext())
                gameController.SetPuzzle(puzzleEnumerator.Current);
            else
                NotifyNoMorePuzzles();
        }

        public void StartSession() {
            Debug.Log("Start Session");
            LoadLevel();
            NextPuzzle();
            NotifySessionStarted();
	    }

        void NotifyNoMorePuzzles() {
            Debug.Log("NotifyNoMorePuzzles");
            if (NoMorePuzzles != null)
                NoMorePuzzles();

            CloseScene();
        }

        void CloseScene() {
            Debug.Log("CloseScene");
            new LevelMenuSceneConstructor(0).Goto();
        }

        void NotifySessionStarted() {
            if (SessionStarted != null)
                SessionStarted();
        }
    }
}
