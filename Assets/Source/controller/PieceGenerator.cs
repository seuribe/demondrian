using System.Collections;
using com.perroelectrico.demondrian.core;

using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// Generates new pieces on request (gameobject & controller), applies materials & texturing
    /// </summary>
    public class PieceGenerator : MonoBehaviour {

        public GameObject pieceModel;

        PieceObjectSource objectSource;
        MaterialGenerator materialGenerator;

        void Awake() {
            objectSource = new PieceObjectSource(pieceModel);
            materialGenerator = FindObjectOfType<MaterialGenerator>();
        }

        public PieceController NewStandardPiece(Piece piece, bool ghost = false) {
            var pc = NewPiece(piece);
            materialGenerator.ApplyMaterial(pc, new MaterialDef(piece, ghost));
            return pc;
        }

        public PieceController NewTexturedPiece(Piece piece, Coord coord, Texture image, int boardSize) {
            var pc = NewPiece(piece);
            materialGenerator.ApplyMaterial(pc, new MaterialDef(pc.piece, tex: image));
            materialGenerator.ApplyTexture(pc, coord, image, boardSize);
            return pc;
        }

        // FIXME: Ghost piece loses material alpha when reapplied
        public void ReapplyMaterial(PieceController pc) {
            materialGenerator.ApplyMaterial(pc, new MaterialDef(pc.piece));
        }

        PieceController NewPiece(Piece piece) {
            var go = objectSource.Get();
            var pc = go.GetComponent<PieceController>();
            pc.piece = piece;
            return pc;
        }

        IEnumerator WaitAndDestroy(PieceController pc, float delay) {
            yield return new WaitForSeconds(delay);
            objectSource.Return(pc.gameObject);
        }

        public void DestroyPiece(PieceController pc, float delay = 0f) {
            StartCoroutine(WaitAndDestroy(pc, delay));
        }

        public void Clear() {
            objectSource.Clear();
        }
    }
}
