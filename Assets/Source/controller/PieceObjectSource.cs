using System.Collections.Generic;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// A source of piece's game objects.
    /// </summary>
    public class PieceObjectSource {
        readonly GameObject pieceModel;
        readonly bool poolPieces;

        Queue<GameObject> queue = new Queue<GameObject>();
        static int pieceSerial = 0;

        public PieceObjectSource(GameObject pieceModel, bool poolPieces = true) {
            this.pieceModel = pieceModel;
            this.poolPieces = poolPieces;
        }

        bool CanDequeue() {
            return poolPieces && queue.Count > 0;
        }

        public GameObject Get() {
            return CanDequeue() ? Reactivate() : GenerateNew();
        }

        GameObject GenerateNew() {
            var go = GameObject.Instantiate<GameObject>(pieceModel);
            go.name = "piece_" + (++pieceSerial);
            return go;
        }

        public void Return(GameObject go) {
            if (poolPieces)
                Deactivate(go);
            else
                GameObject.Destroy(go);
        }

        GameObject Reactivate() {
            var go = queue.Dequeue();
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true);
            return go;
        }

        void Deactivate(GameObject go) {
            queue.Enqueue(go);
            go.transform.parent = null;
            go.SetActive(false);
        }

        public void Clear() {
            foreach (var go in queue)
                GameObject.Destroy(go);

            queue.Clear();
        }
    }
}