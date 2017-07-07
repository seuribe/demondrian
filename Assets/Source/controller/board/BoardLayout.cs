using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.util;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// Manages the exact positioning of pieces on the board
    /// </summary>
    public class BoardLayout {
        readonly Transform piecesParent;
        readonly float unitWidth;
        readonly float globalScale;
        readonly int size;

        public int Size {
            get { return size; }
        }

        public BoardLayout(Board board, Transform piecesParent) {
            this.piecesParent = piecesParent;
            this.unitWidth = 1f / board.Size;
            this.size = board.Size;
            this.globalScale = Util.GetGlobalXScale(piecesParent);
        }

        public Vector3 CoordToPosition(Coord coord, int pieceSize) {
            var zeroCoord = new Vector3(
                -0.5f + unitWidth / 2,
                -0.5f + unitWidth / 2,
                0);

            var pos = new Vector3(
                zeroCoord.x + unitWidth * coord.col,
                zeroCoord.y + unitWidth * coord.row,
                0);

            if (pieceSize > 1) {
                var diff = (pieceSize - 1) * unitWidth / 2;
                pos += new Vector3(diff, diff, 0);
            }

            return Quaternion.Inverse(piecesParent.transform.localRotation) * pos;
        }

        public Coord MouseToCoord(Vector3 mousePos) {
            var worldPos = GetWorldPos(mousePos);
            if (OutOfBoard(worldPos))
                return Coord.Invalid;
            var movedPos = DeCenterPosition(worldPos);
            return TransformToCoord(movedPos);
        }

        Coord TransformToCoord(Vector3 movedPos) {
            var scaledWidth = globalScale * unitWidth;
            return new Coord {
                col = (int)(movedPos.x / scaledWidth),
                row = (int)(movedPos.y / scaledWidth)
            };
        }

        bool OutOfBoard(Vector3 worldPos) {
            return Mathf.Abs(worldPos.x) > globalScale / 2f || Mathf.Abs(worldPos.y) > globalScale / 2f;
        }

        Vector3 DeCenterPosition(Vector3 worldPos) {
            return worldPos + new Vector3(globalScale / 2f, globalScale / 2f, 0);
        }

        Vector3 GetWorldPos(Vector3 mousePos) {
            var z = piecesParent.transform.position.z - Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, z));
        }

        public void PositionPieceAboveBoard(PieceController pc, Coord coord) {
            var aboveCoord = new Coord(coord.col, size);
            PositionPieceInBoard(pc, aboveCoord);
        }

        public void PositionPieceInBoard(PieceController pc, Coord coord) {
            var pieceWidth = unitWidth * pc.piece.size;
            var tr = pc.transform;
            tr.parent = piecesParent.transform;
            tr.localScale = new Vector3(pieceWidth, pieceWidth, pieceWidth);
            tr.localRotation = Quaternion.Euler(180, 0, 0);
            tr.localPosition = CoordToPosition(coord, pc.piece.size);
        }

        public void AnimateIntoPosition(PieceController pc, Coord coord, float delay) {
            pc.transform.parent = piecesParent.transform;
            pc.AnimateMove(CoordToPosition(coord, pc.piece.size), delay / 2);
            pc.ScaleToSize(unitWidth, delay / 2);
        }

    }
}
