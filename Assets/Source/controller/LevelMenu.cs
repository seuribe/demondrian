using com.perroelectrico.demondrian.level;
using Source.controller.scene;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {
    public class LevelMenu : MonoBehaviour {

        public GameObject parent;
        public TextAsset levelsAsset;
        public float separation = 0.75f;
        public float boardWidth = 2f;

        Level current;
        LevelRepository lm;
        BoardInstantiator boardInstantiator;

        void Awake() {
            lm = LevelRepository.Instance;
            lm.Load(levelsAsset.text);
            boardInstantiator = FindObjectOfType<BoardInstantiator>();
        }

        void Start() {
            if (current == null)
                ShowLevel(lm[0]);
        }


        public void ShowLevel(int levelIndex) {
            ShowLevel(lm[levelIndex]);
        }

	    void ShowLevel(Level level) {
	        current = level;
	        var totalWidth = (level.Count * boardWidth) + ((level.Count - 1) * separation);
            var y = - totalWidth / 2 + boardWidth / 2;

            foreach (var puzzle in level) {
                ShowPuzzle(puzzle, y);
                y += separation + boardWidth;
            }
	    }

        void ShowPuzzle(Puzzle puzzle, float y) {
            var bc = CreateBoardController(y);
            SetupBoardController(bc, puzzle);
        }

        void SetupBoardController(BoardController bc, Puzzle puzzle) {
            bc.SetGame(puzzle.NewGame());
            bc.PauseInput();
            if (lm.PuzzleUnlocked(puzzle))
                bc.OnClick += controller => { StartPuzzle(puzzle); };
        }

        BoardController CreateBoardController(float y) {
            return boardInstantiator.Instantiate(
                new Vector3(y, 0, 0),
                new Vector3(boardWidth, boardWidth, 1),
                parent.transform);
        }

        void StartPuzzle(Puzzle puzzle) {
            Debug.LogFormat("StartPuzzle {0}", puzzle);

            var gsc = new GameSceneConstructor(puzzle);
            gsc.Goto();
        }
    }
}
