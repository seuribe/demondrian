
using UnityEngine;

namespace com.perroelectrico.demondrian.controller.scene {

    public abstract class SceneConstructor {

        protected abstract string SceneId { get; }

        public void Goto() {
            var instantiator = GameObject.FindObjectOfType<SceneInstantiator>();
            instantiator.Load(SceneId, this);
        }

        public virtual void SetUp() {}

        public virtual void TearDown() {}
    }
}