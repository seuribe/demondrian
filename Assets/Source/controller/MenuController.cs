using com.perroelectrico.demondrian.core;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// WIP
    /// </summary>
    public class MenuController : MonoBehaviour {
        public Menu menu;
        public Transform parent;
        public GameObject boardPrefab;
        public PieceGenerator generator;

        BoardController controller;

        void Awake() {
            if (parent == null)
                parent = transform;
        }

        void Start() {
            Debug.Log("Build");
            menu = Menu.MainMenu;

            var boardGO = GameObject.Instantiate<GameObject>(boardPrefab);
            boardGO.transform.parent = parent;
            controller = boardGO.GetComponent<BoardController>();

            foreach (var pc in menu.board.Pieces) {
                AddPiece(pc.Key, pc.Value, boardGO);
            }
        }

        void AddPiece(Piece piece, Coord coord, GameObject boardGo) {
            Debug.LogFormat("AddPiece: {0}, {1}, {2}", piece, coord, boardGo);
/*
            var pos = layout.CoordToPosition(coord, piece.size);
            var go = generator.NewPiece(piece);
            var pc = go.GetComponent<PieceController>();
            controller.PositionPieceInBoard(pc, coord);
*/
//            go.transform.parent = boardGo.transform;
//            go.transform.position = pos;
        }
    }
}
