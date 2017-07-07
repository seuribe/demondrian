using System.Collections.Generic;

using com.perroelectrico.demondrian.core;

using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// Manages the interactions with the Board when it comes to editing by users
    /// </summary>
    public class BoardEditor : MonoBehaviour {

        public GameObject editorOptions;

        BoardController controller;
        IncomingController incoming;
        PieceGenerator pieceGenerator;
        PieceController cursorPiece;

        int typeIndex = 0;
        Coord cursorEditCoord;
        bool editing = false;

        int CursorSize {
            get { return cursorPiece.piece.size; }
        }

        int BoardSize {
            get { return board.Size; }
        }

        Board board {
            get { return controller.Game.Board; }
        }

        BoardLayout Layout {
            get { return controller.layout; }
        }

        void Awake() {
            pieceGenerator = FindObjectOfType<PieceGenerator>();
        }

        void SetCursor(Piece piece) {
            if (cursorPiece == null) {
                cursorPiece = pieceGenerator.NewStandardPiece(piece, true);
                cursorPiece.transform.parent = controller.piecesParent;
                cursorPiece.name = "EditorCursor";
            }
            else {
                cursorPiece.piece = piece;
                pieceGenerator.ReapplyMaterial(cursorPiece);
            }
            cursorPiece.gameObject.SetActive(editing);
        }

        void Start() {
            controller = controller ?? FindObjectOfType<BoardController>();
            incoming = incoming ?? FindObjectOfType<IncomingController>();
        }

        public void Edit() {
            if (!editing)
                StartEdit();
            else
                StopEdit();

            cursorPiece.gameObject.SetActive(editing);
            editorOptions.SetActive(editing);
        }

        void StartEdit() {
            editing = true;
            controller.PauseInput();
            SetCursor(new Piece(controller.Game.Types[typeIndex], 1));
        }

        void StopEdit() {
            editing = false;
            controller.ResumeInput();
        }

        public void Save() {
            Debug.Log("Save");
        }

        public void Load() {
            Debug.Log("Load");
        }

        void Update() {
            if (!editing)
                return;

            UpdateCursorSize();
            UpdateCursorType();
            UpdateCursorPosition();

            var overBoard = controller.IsMouseOverBoard();
            cursorPiece.gameObject.SetActive(overBoard);

            if (overBoard) {
                if (RemovePiecesPressed())
                    ClearBoardAtCursor();
                else if (SetPiecesPressed())
                    SetPieceAtCursor();
            }
        }
        bool RemovePiecesPressed() {
            return Input.GetMouseButtonDown(1);
        }
        bool SetPiecesPressed() {
            return Input.GetMouseButtonDown(0);
        }
        bool NextPieceTypePressed() {
            return Input.GetKeyDown(KeyCode.KeypadPlus);
        }
        bool PrevPieceTypePressed() {
            return Input.GetKeyDown(KeyCode.KeypadMinus);
        }
        bool IncreasePieceSizePressed() {
            return Input.GetAxis("Mouse ScrollWheel") > 0f;
        }
        bool DecreasePieceSizePressed() {
            return Input.GetAxis("Mouse ScrollWheel") < 0f;
        }

        IEnumerable<Coord> CursorCoords {
            get {
                foreach (var coord in Coord.SquareRange(cursorEditCoord, CursorSize))
                    yield return coord;
            }
        }

        void SetPieceAtCursor() {
            if (board.IsEmpty(cursorEditCoord, CursorSize)) {
                board.Set(cursorEditCoord, cursorPiece.piece.Clone());
                controller.ReGenerate();
            }
        }

        void ClearBoardAtCursor() {
            bool anyRemoved = false;
            foreach (var coord in CursorCoords)
                if (!board.IsEmpty(coord)) {
                    board.Remove(coord);
                    anyRemoved = true;
                }

            if (anyRemoved)
                controller.ReGenerate();
        }

        void UpdateCursorPosition() {
            cursorEditCoord = controller.layout.MouseToCoord(Input.mousePosition)
                                .Crop(controller.Size - CursorSize + 1, controller.Size - CursorSize + 1);
            controller.layout.PositionPieceInBoard(cursorPiece, cursorEditCoord);
            cursorPiece.transform.localRotation = Quaternion.identity;
            cursorPiece.transform.localPosition += new Vector3(0, 0, -0.5f);
        }

        void UpdateCursorType() {
            if (NextPieceTypePressed())
                NextPieceType();
            else if (PrevPieceTypePressed())
                PrevPieceType();
        }

        void NextPieceType() {
            typeIndex = (typeIndex + 1) % controller.Game.Types.Length;
            ReapplyPieceType();
        }

        void PrevPieceType() {
            typeIndex = (typeIndex + 1) % controller.Game.Types.Length;
            ReapplyPieceType();
        }

        void ReapplyPieceType() {
            cursorPiece.piece = new Piece(controller.Game.Types[typeIndex], CursorSize);
            pieceGenerator.ReapplyMaterial(cursorPiece);
        }

        void UpdateCursorSize() {
            if (IncreasePieceSizePressed())
                IncreaseCursorSize();
            if (DecreasePieceSizePressed())
                DecreaseCursorSize();
        }

        void IncreaseCursorSize() {
            var newSize = CursorSize == BoardSize ? 1 : CursorSize * 2;
            SetCursorSize(newSize);
        }

        void DecreaseCursorSize() {
            var newSize = CursorSize == 1 ? BoardSize : CursorSize / 2;
            SetCursorSize(newSize);
        }

        void SetCursorSize(int size) {
            cursorPiece.piece = new Piece(cursorPiece.piece.type, size);
        }
    }
}