using System.Collections;
using com.perroelectrico.demondrian.util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.perroelectrico.util {

    /// <summary>
    /// Simple helper behaviour to jump to another scene either by clicking or timeout
    /// </summary>
    public class JumpToScene : MonoBehaviour {

        public float jumpTime = 2f;
        public bool jumpOnClick = true;
        public string clickAxisName = "Fire1";
        public string sceneName = "LevelMenu";

        readonly OneTimeSwitch jumped = new OneTimeSwitch();

	    void Start () {
	        if (jumpTime > 0)
                StartCoroutine(DelayedJump());
	    }

	    void Update () {
	        if (jumpOnClick && Input.GetButtonDown(clickAxisName))
                JumpNow();
	    }

        IEnumerator DelayedJump() {
            yield return new WaitForSeconds(jumpTime);
            JumpNow();
        }

        private void JumpNow() {
            if (!jumped.Get())
                return;

            SceneManager.LoadScene(sceneName);
        }
    }
}
