using System.Collections;
using System.Collections.Generic;
using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.util;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    public delegate void PieceEvent(Piece piece);
    public delegate void BoardEvent(BoardController bc);

    public delegate PieceController ControllerProvider(Piece piece);

    /// <summary>
    /// Resposible for managing interactions with the board, pieces & animations
    /// </summary>
    public class BoardController : MonoBehaviour {

        public Transform piecesParent;
        public GameObject frame;
        public GameObject back;

        public GameObject incoming;

        [Range(0, 1)]
        public float frameWidth = 0.05f;
        private Rotator rotator;

        public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Range(0, 1)]
        public float rotationTime = 0.25f;
        [Range(0, 1)]
        public float pieceFallTime = 0.1f;
        [Range(0, 1)]
        public float removeTime = 0.25f;
        [Range(0, 1)]
        public float addPieceTime = 0.25f;
        [Range(0, 1)]
        public float generationTime = 0.25f;
        [Range(0, 1)]
        public float postActionDelay = 0.1f;

        public event PieceEvent PieceSet;
        public event PieceEvent PieceRemoved;

        public event BoardEvent OnClick;
        public event BoardEvent MoveFinished;

        public ControllerProvider IncomingPieceProvider;

        private bool processingInput;
        public BoardLayout layout;

        public Game Game { get; private set; }
        public int Size { get { return board == null ? 0 : board.Size; } }

        PieceGenerator pieceGenerator;
        GhostPieceGenerator ghostGenerator;

        Board board;

        PieceController[,] pcs;

        public void SetGame(Game game, Texture image = null, System.Action doAfterSet = null) {
            this.Game = game;
            this.board = game.Board;
            layout = new BoardLayout(board, piecesParent);
            ghostGenerator = new GhostPieceGenerator(pieceGenerator, layout, removeTime);
            SyncRotation();
            Generate(image, doAfterSet);
        }

        public bool InputEnabled {
            get { return !rotator.Rotating && !processingInput && board != null; }
        }

        public bool Rotating { get { return rotator.Rotating; } }
        
        void Awake() {
            pieceGenerator = FindObjectOfType<PieceGenerator>();
            if (piecesParent == null)
                piecesParent = transform;

            rotator = new Rotator(piecesParent, rotationTime, rotationCurve);
        }

        public IEnumerator RePlay(IEnumerable<Move> moves) {
            foreach (var move in moves)
                yield return StartCoroutine(ExecuteAndAnimate(move));
        }

        void Execute(Move move) {
            StartCoroutine(ExecuteAndAnimate(move));
        }

        void DisplayObstacles(IEnumerable<Piece> obstacles) {
            StartCoroutine(HighlightObstaclePieces(obstacles));
        }

        public void PauseInput() {
            processingInput = true;
        }

        public void ResumeInput() {
            processingInput = false;
        }

        public void Sync() {
            SyncRotation();
            SyncGame();
        }

        private void SyncRotation() {
            rotator.Set(OrientationToDegrees(board.Orientation));
        }

        IEnumerator ExecuteAndAnimate(Move move) {
            foreach (var action in move.consequences) {
                yield return ExecuteAndAnimate(action);
            }
            Game.ExecuteEndMove(move);

            MoveDone();
        }

        IEnumerator ExecuteAndAnimate(BoardAction action) {
            Game.Execute(action);
            AnimateAction(action);
            yield return Util.Wait(AnimationTime(action));
        }

        private System.TimeSpan AnimationTime(BoardAction action) {
            if (action is NewPieceAction)
                return System.TimeSpan.FromSeconds(addPieceTime);

            if (action is RemoveAction)
                return System.TimeSpan.FromSeconds(removeTime);

            if (action is FallAction)
                return System.TimeSpan.FromSeconds(pieceFallTime);

            if (action is CompactAction)
                return System.TimeSpan.FromSeconds(removeTime);

            return System.TimeSpan.Zero;
        }

        void AnimateAction(BoardAction action) {
            if (action is RotateAction)
                AnimateRotateAction((RotateAction)action);
            else if (action is NewPieceAction)
                AnimateNewPiece((NewPieceAction)action);
            else if (action is RemoveAction)
                AnimateRemoveAction((RemoveAction)action);
            else if (action is FallAction)
                AnimateFallAction((FallAction)action);
            else if (action is CompactAction)
                AnimateCompactAction((CompactAction)action);
        }

        void AnimateFallAction(FallAction action) {
            var c = action.coord;
            var pc = Remove(c);
            var newc = c.Move(0, -action.rows);
            AnimateFallTo(pc, newc);
            Set(newc, pc);
        }

        void AnimateFallTo(PieceController pc, Coord coord) {
            layout.AnimateIntoPosition(pc, coord, pieceFallTime);
        }

        void AnimateRemoveAction(RemoveAction action) {
            AnimateRemove(action.coord);
        }

        void AnimateRemove(Coord coord) {
            var pc = Remove(coord);
            pc.OnRemove();
            pieceGenerator.DestroyPiece(pc, removeTime);
        }

        void AnimateNewPiece(NewPieceAction action) {
            var piece = action.newPiece;
            var coord = action.coord;

            var pc = Game.Rules.HasIncomingQueue ?
                IncomingPieceProvider(piece) :
                GenerateNewPiece(piece, coord);

            if (piece.size != 1)
                pieceGenerator.ReapplyMaterial(pc);

            layout.AnimateIntoPosition(pc, coord, addPieceTime);
            Set(coord, pc);
        }

        public void AnimateRotation(RotationDir dir) {
            StartCoroutine(rotator.Animate(dir));
            RotatePCs(dir);
        }

        void AnimateRotateAction(RotateAction action) {
            AnimateRotation(action.dir);
        }

        void AnimateCompactAction(CompactAction action) {
            var newPiece = action.bigPiece;
            var coord = action.coord;
            foreach (var c in coord.SquareCoords(newPiece.size / 2))
                AnimateRemove(c);

            AddNewPiece(newPiece, coord);
        }

        public void AnimateRemovePieces(float maxDelay) {
            foreach (var coord in board.Pieces.Values)
                StartCoroutine(RemoveDelayed(coord, Util.Random(0, maxDelay)));
        }

        private IEnumerator HighlightObstaclePieces(IEnumerable<Piece> pieces) {
            foreach (var piece in pieces)
                HighlightObstacle(board.Where(piece));

            yield return new WaitForSeconds(postActionDelay);
            ResumeInput();
        }

        private void HighlightObstacle(Coord coord) {
            var pc = Get(coord);
            pc.OnObstacle();
        }

        private static float OrientationToDegrees(Orientation or) {
            return (270 * (int)or) % 360;
        }

        public bool IsMouseOverBoard() {
            return GetComponent<BoxCollider>().bounds
                        .IntersectRay(Camera.main.ScreenPointToRay(Input.mousePosition));
        }

        void Update() {
            var mouseDown = Input.GetMouseButtonDown(0);

            if (mouseDown)
                ProcessBoardClick();

            if (!InputEnabled)
                return;

            var overCoord = layout.MouseToCoord(Input.mousePosition);
            if (!overCoord.Valid)
                OnInvalidCoordHover();
            else if (mouseDown)
                ClickCoord(overCoord);
            else
                OnCoordHover(overCoord);
        }

        void ProcessBoardClick() {
            if (OnClick != null && IsMouseOverBoard())
                OnClick(this);
        }

        void OnCoordHover(Coord coord) {
            if (Game.Rules.ShowGhostPiece)
                if (IsValidMove(coord))
                    ghostGenerator.HoverOver(coord, Game.NextPieceIfPlaying(coord));
                else
                    ghostGenerator.Clear();
        }

        bool IsValidMove(Coord coord) {
            return !Game.IsSolved && new PlayLogic(Game.Board).IsValidMove(coord);
       }

        void OnInvalidCoordHover() {
            if (Game.Rules.ShowGhostPiece)
                ghostGenerator.Clear();
        }

        void ClickCoord(Coord coord) {
            if (!InputEnabled)
                return;

            PauseInput();

            if (Game.Rules.ShowGhostPiece)
                ghostGenerator.ClickedOn(coord);

            ProcessClickedCoord(coord);
        }

        Coord GetBaseCoord(Coord coord) {
            return board.Where(board.Get(coord));
        }

        void ProcessClickedCoord(Coord coord) {
            coord = GetBaseCoord(coord);
            var simulator = MoveSimulator.FromCoord(board, coord, Game.NextPieceIfPlaying(coord));
            simulator.Simulate();
            if (simulator.IsMovePossible())
                Execute(simulator.GetMove());
            else
                DisplayObstacles(simulator.Obstacles());
        }

        void MoveDone() {
            ResumeInput();
            NotifyMoveFinished();
        }

        void NotifyMoveFinished() {
            if (MoveFinished != null)
                MoveFinished(this);
        }

        // TODO: abstract board access shared between this and the Board class
        #region low level pc board access
        private PieceController Get(Coord coord) {
            return pcs[coord.row, coord.col];
        }

        bool IsEmpty(Coord coord) {
            return pcs[coord.row, coord.col] == null;
        }

        private void Set(Coord coord, PieceController controller) {
            if (!IsEmpty(coord))
                throw new InvalidBoardStateException(string.Format("Cannot set PC on non empty coord {0}", coord));

            pcs[coord.row, coord.col] = controller;
            NotifyPieceSet(controller.piece);
        }

        private PieceController Remove(Coord coord) {
            if (IsEmpty(coord))
                throw new InvalidBoardStateException(string.Format("No PC to remove from {0}", coord));

            var controller = pcs[coord.row, coord.col];
            pcs[coord.row, coord.col] = null;
            NotifyPieceRemoved(controller.piece);

            return controller;
        }

        void Reset() {
            if (pcs != null)
                Clear();
            pcs = new PieceController[Size, Size];
        }

        void Clear() {
            foreach (var pc in PieceControllers)
                pieceGenerator.DestroyPiece(pc);
            pcs = null;
        }

        void NotifyPieceRemoved(Piece piece) {
            if (PieceRemoved != null)
                PieceRemoved(piece);
        }

        void NotifyPieceSet(Piece piece) {
            if (PieceSet != null)
                PieceSet(piece);
        }
        #endregion

        void RotatePCs(RotationDir dir) {
            var newPcs = new PieceController[Size, Size];
            foreach (var c_pc in CoordAndControllers) {
                var oldCoord = c_pc.Key;
                var pc = c_pc.Value;
                var newCoord = RotatedCoord(pc, oldCoord, dir);
                newPcs[newCoord.row, newCoord.col] = pc;
            }
            pcs = newPcs;
        }

        Coord RotatedCoord(PieceController pc, Coord coord, RotationDir dir) {
            return (dir == RotationDir.Left) ?
                board.AdjustForRotateLeft(board.RotateCoordLeft(coord), pc.piece) :
                board.AdjustForRotateRight(board.RotateCoordRight(coord), pc.piece);
        }

        IEnumerable<KeyValuePair<Coord, PieceController>> CoordAndControllers {
            get {
                foreach (var coord in Coord.SquareRange(Coord.c00, pcs.GetLength(0)))
                    if (!IsEmpty(coord))
                        yield return new KeyValuePair<Coord, PieceController>(coord, pcs[coord.row, coord.col]);
            }
        }

        IEnumerable<PieceController> PieceControllers {
            get {
                foreach (var coord in Coord.SquareRange(Coord.c00, pcs.GetLength(0)))
                    if (!IsEmpty(coord))
                        yield return Get(coord);
            }
        }

        public void ReGenerate() {
            var accountedFor = new HashSet<Piece>();
            foreach (var c_pc in CoordAndControllers) {
                var oldPos = new Coord(c_pc.Key);
                var pc = c_pc.Value;
                var piece = pc.piece;

                if (board.ContainsPiece(piece)) { // piece still there
                    var newPos = board.Where(piece);
                    accountedFor.Add(piece);
                    if (!newPos.Matches(oldPos)) { // piece has moved
                        layout.PositionPieceInBoard(pc, newPos);
                        pc.OnAdd();
                    }
                } else {    // piece no longer there
                    pieceGenerator.DestroyPiece(pc);
                    Remove(oldPos);
                }
            }

            foreach (var kv in board.Pieces) {
                var piece = kv.Key;
                var coord = kv.Value;
                if (accountedFor.Contains(piece)) {
                    continue;
                }
                AddNewPiece(piece, coord);
            }
        }

        void OnDestroy() {
            pieceGenerator.Clear();
        }

        private IEnumerator RemoveDelayed(Coord coord, float delay) {
            yield return new WaitForSeconds(delay);
            Get(coord).OnRemove();
            Remove(coord);
        }

        PieceController GenerateNewPiece(Piece piece, Coord coord, Texture image = null) {
            return (image == null) ?
                pieceGenerator.NewStandardPiece(piece) :
                pieceGenerator.NewTexturedPiece(piece, coord, image, Size);
        }

        private PieceController AddNewPiece(Piece piece, Coord coord, Texture image = null) {
            var pc = GenerateNewPiece(piece, coord, image);
            layout.PositionPieceInBoard(pc, coord);
            Set(coord, pc);
            pc.OnAdd();
            return pc;
        }

        void SyncGame() {
            Reset();
            foreach (var p in board.Pieces) {
                var piece = p.Key;
                var coord = p.Value;
                AddNewPiece(piece, coord);
            }
        }

        void Generate(Texture image = null, System.Action doAfterSet = null) {
            Reset();
            StartCoroutine(GenerateWithDelay(image, doAfterSet));
        }

        IEnumerator GenerateWithDelay(Texture image = null, System.Action doAfterSet = null) {
            foreach (var p in board.Pieces) {
                var piece = p.Key;
                var coord = p.Value;
                StartCoroutine(GenerateWithDelay(piece, coord, image));
            }
            yield return new WaitForSeconds(generationTime);
            if (doAfterSet != null)
                doAfterSet();
        }

        IEnumerator GenerateWithDelay(Piece piece, Coord coord, Texture image = null) {
            var delay = Random.Range(0f, generationTime);
            yield return new WaitForSeconds(delay);
            AddNewPiece(piece, coord, image);
        }
    }
}
