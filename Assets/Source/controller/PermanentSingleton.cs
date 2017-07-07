using System.Linq;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// Helper class to have singletons in Unity that persist beyond a single scene
    /// </summary>
    public class PermanentSingleton : MonoBehaviour {
        public string id;

        private void Awake() {
            if (AlreadyPresent())
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);
        }

        bool AlreadyPresent() {
            return FindObjectsOfType<PermanentSingleton>()
                       .Count(singleton => singleton.id == id) > 1;
        }

    }

}