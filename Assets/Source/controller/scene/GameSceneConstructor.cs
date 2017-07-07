using com.perroelectrico.demondrian;
using com.perroelectrico.demondrian.controller;
using com.perroelectrico.demondrian.controller.scene;
using com.perroelectrico.demondrian.level;
using UnityEngine;

namespace Source.controller.scene {
    public class GameSceneConstructor : SceneConstructor {
        private readonly Puzzle puzzle;

        public GameSceneConstructor(Puzzle puzzle) {
            this.puzzle = puzzle;
        }

        protected override string SceneId {
            get { return "Game"; }
        }

        public override void SetUp() {
            var sessionController = GameObject.FindObjectOfType<SessionController>();
            sessionController.SetPuzzles(new Browseables.Singleton<Puzzle>(puzzle));
            sessionController.StartSession();
        }

        public override void TearDown() {
        }
    }
}