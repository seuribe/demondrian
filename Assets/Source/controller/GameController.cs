using System.Collections;
using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.level;
using com.perroelectrico.demondrian.solver;

using UnityEngine;
using UnityEngine.UI;

namespace com.perroelectrico.demondrian.controller {

    public delegate void GameEvent(Game game);

    /// <summary>
    /// Manages general game interaction
    /// </summary>
    public class GameController : MonoBehaviour {

        public Vector3 boardPos = new Vector3(0, 0, 0);
        public Vector3 boardScale = new Vector3(8f, 8f, 1f);

        public Text movesText;

        public event GameEvent GameSet;
        public event GameEvent GameSolved;

        private BoardController boardController;
        private IncomingController ic;
        private Game game;
        private bool undoing;

        public bool InputEnabled {
            get { return !undoing && (game != null) && boardController.InputEnabled && !game.IsSolved; }
        }

        void Awake() {
            ic = FindObjectOfType<IncomingController>();
            InitializeBoardController();
        }

        void InitializeBoardController() {
            Debug.Log("InitializeBoardController");
            var boardInstantiator = FindObjectOfType<BoardInstantiator>();
            boardController = boardInstantiator.Instantiate(boardPos, boardScale);
            boardController.IncomingPieceProvider = GetNextPieceController;
            boardController.MoveFinished += OnMoveFinished;
        }

        void OnMoveFinished(BoardController bc) {
            if (game.IsSolved)
                NotifyGameSolved();
            else
                ic.Advance();
        }

        private void NotifyGameSolved() {
            if (GameSolved != null)
                GameSolved(game);
        }

        private PieceController GetNextPieceController(Piece piece) {
            var pc = ic.DetachIncoming();
            pc.piece = piece;
            return pc;
        }

        public void SetPuzzle(Puzzle puzzle) {
            game = puzzle.NewGame();
            boardController.SetGame(game, doAfterSet: EndSetPuzzle);
        }

        void EndSetPuzzle() {
            NotifyGameSet();
            ic.Reset();
        }

        void NotifyGameSet() {
            if (GameSet != null)
                GameSet(game);
        }

        // called from the interface
        public void RotateRight() {
            Rotate(RotationDir.Right);
        }

        // called from the interface
        public void RotateLeft() {
            Rotate(RotationDir.Left);
        }

        public void Rotate(RotationDir dir) {
            if (!InputEnabled)
                return;

            boardController.AnimateRotation(dir);
            game.Rotate(dir);
        }

        public void Undo() {
            if (!game.CanUndo())
                return;

            undoing = true;

            game.Undo();
            boardController.Sync();
            ic.Reset();

            undoing = false;
        }

        public void Reset() {
            if (!InputEnabled) {
                return;
            }
            ic.Reset();
        }

        public void Solve() {
            Debug.Log("Solve");

            StartCoroutine(RunSolver());
        }

        IEnumerator RunSolver() {
            boardController.PauseInput();
            Solver solver = new Solver(game);

            var state = solver.Solve(maxMoves: 10);
            if (state.IsSolvable) {
                Debug.Log("Replay solution");
                foreach (var move in state.Moves) {
                    Debug.Log("move: " + move);
                    game.Execute(move);
                    boardController.ReGenerate();
                    yield return new WaitForSeconds(0.25f);
                }
            }

            boardController.ResumeInput();
            yield return null;
        }
    }
}
