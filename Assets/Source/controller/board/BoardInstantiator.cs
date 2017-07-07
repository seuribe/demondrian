using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    public class BoardInstantiator : MonoBehaviour {
        public GameObject boardPrefab;

        public BoardController Instantiate(Vector3? position = null, Vector3? scale = null, Transform parent = null) {
            var go = Instantiate(boardPrefab);
            go.transform.localScale = scale ?? Vector3.one;
            go.transform.position = position ?? Vector3.zero;
            go.transform.parent = parent == null ? null : parent;
            return go.GetComponent<BoardController>();
        }
    }

}