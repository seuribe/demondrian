
using UnityEngine;

namespace com.perroelectrico.demondrian.controller.scene {

    public class LevelMenuSceneConstructor : SceneConstructor{
        private readonly int levelIndex;
        protected override string SceneId {
            get { return "LevelMenu"; }
        }

        public LevelMenuSceneConstructor(int levelIndex) {
            this.levelIndex = levelIndex;
        }

        public override void SetUp() {
            var levelMenu = GameObject.FindObjectOfType<LevelMenu>();
            levelMenu.ShowLevel(levelIndex);
        }
    }

}