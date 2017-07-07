using com.perroelectrico.demondrian.controller;
using com.perroelectrico.demondrian.core;
using com.perroelectrico.demondrian.generator;
using UnityEngine;

namespace com.perroelectrico.demondrian {
    public class TestImageGenerator : MonoBehaviour {

        public BoardController bc;
        public Texture texture;

	    void Start () {
            var types = new PieceType[]{
                PieceType.Get(0),
                PieceType.Get(1),
                PieceType.Get(2)};

            var game = new Game(
                    new BoardGenerator().GenerateRandom(16, types),
                    GameRules.ClassicRules,
                    PieceType.GetRange(0, 3));
            bc.SetGame(game, texture);
	    }
	
    }
}
