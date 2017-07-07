using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using com.perroelectrico.demondrian.core;

namespace com.perroelectrico.demondrian.controller {

    public class IncomingController : MonoBehaviour {

        static readonly Vector3 IncomingSeparation = new Vector3(1.1f, 0, 0);

        readonly Queue<PieceController> incomingPieces = new Queue<PieceController>();
        PieceGenerator pieceGen;
        GameController gameController;

        public Transform piecesParent;
        [Range(0, 1)]
        public float advanceAnimationTime = 0.2f;

        IncomingQueue incoming;

        void Awake() {
            pieceGen = FindObjectOfType<PieceGenerator>();
            gameController = FindObjectOfType<GameController>();
            piecesParent = piecesParent ?? transform;
            gameController.GameSet += OnGameSet;
        }

        void OnDestroy() {
            gameController.GameSet -= OnGameSet;
        }
        
        private void OnGameSet(Game game) {
            this.incoming = game.Incoming;
            Reset();
        }

        void Add(PieceType type) {
            var pc = GeneratePiece(type);
            PositionPiece(pc, incomingPieces.Count);
            incomingPieces.Enqueue(pc);
        }

        PieceController GeneratePiece(PieceType type) {
            var piece = new Piece(type, 1);
            return pieceGen.NewStandardPiece(piece);
        }

        void PositionPiece(PieceController pc, int index) {
            pc.transform.parent = piecesParent.transform;
            pc.transform.localPosition = IncomingSeparation * (float)((index == -1) ? incoming.LookAheadSize() : index);
        }

        public void Reset() {
            foreach (var pc in incomingPieces)
                pieceGen.DestroyPiece(pc);

            incomingPieces.Clear();
            GeneratePieces();
        }

        void GeneratePieces() {
            foreach (var type in incoming.Incoming())
                Add(type);
        }

        public PieceController DetachIncoming() {
            return incomingPieces.Dequeue();
        }

        public void Advance() {
            AnimateAdvance();
            var queue = incoming.Incoming();
            if (queue.Count > 0)
                Add(queue.Last());
        }

        void AnimateAdvance() {
            foreach (var piece in incomingPieces)
                piece.AnimateMove(piece.transform.localPosition - IncomingSeparation, advanceAnimationTime);
        }
    }
}
