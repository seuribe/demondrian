using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.perroelectrico.demondrian.controller.scene {

    [Serializable]
    public struct GameScene {
        public string id;
    }

    public class SceneInstantiator : MonoBehaviour {

        public GameScene[] scenes;
        private SceneConstructor currentConstructor;

        public void Load(string id, SceneConstructor constructor) {
            StartCoroutine(LoadScene(id, constructor, LoadSceneMode.Single));
        }
        public void Add(string id, SceneConstructor constructor) {
            StartCoroutine(LoadScene(id, constructor, LoadSceneMode.Additive));
        }

        IEnumerator LoadScene(string id, SceneConstructor constructor, LoadSceneMode mode) {
            var asyncOp = SceneManager.LoadSceneAsync(id, mode);
            while (!asyncOp.isDone) {
                yield return null;
            }
            constructor.SetUp();
            currentConstructor = constructor;
        }

        public void Unload(string id) {
            currentConstructor.TearDown();

        }
    }
}