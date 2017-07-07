using System.Collections;
using com.perroelectrico.demondrian.core;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    public class PieceController : MonoBehaviour {
        public Piece piece;
        public float rotateTime = 0.75f;
        public AudioClip blocked;
        public AudioClip removed;

        public event PieceEvent Clicked;
        public event PieceEvent MouseOver;

        private Animation anim;

        void Awake() {
            this.anim = GetComponent<Animation>();
        }

        public void ScaleToSize(float unitWidth, float duration) {
            StartCoroutine(DoAnimateScale(new Vector3(1,1,1) * (piece.size * unitWidth), duration));
        }

        IEnumerator DoAnimateScale(Vector3 scale, float duration) {
            float progress = 0;
            var currentScale = transform.localScale;
            while (progress < 1) {
                progress += Time.deltaTime / duration;
                gameObject.transform.localScale = Vector3.Lerp(currentScale, scale, progress);
                yield return null;
            }
            gameObject.transform.localScale = scale;
        }

        public void OnAdd() {
            StartCoroutine(DoAnimateRotate(TurnX180(transform.rotation), rotateTime));
        }

        IEnumerator DoAnimateRotate(Quaternion dest, float duration) {
            Quaternion orig = transform.rotation;
            float progress = 0;
            while (progress < 1) {
                progress += Time.deltaTime / duration;
                gameObject.transform.rotation = Quaternion.Lerp(orig, dest, progress);
                yield return null;
            }
            gameObject.transform.rotation = dest;
        }
        
        private Quaternion TurnX180(Quaternion q) {
            var rot = q;
            var euler = rot.eulerAngles;
            euler.x += 180;
            return Quaternion.Euler(euler);
        }

        void PlayClip(AudioClip clip) {
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void OnRemove() {
            PlayClip(removed);
            StartCoroutine(DoAnimateRotate(TurnX180(transform.rotation), rotateTime));
        }

        public void OnObstacle() {
            PlayClip(blocked);
            anim.Play("piece_blocker");
        }

        public void AnimateMove(Vector3 dest, float duration) {
            StartCoroutine(DoAnimateMove(dest, duration));
        }

        IEnumerator DoAnimateMove(Vector3 dest, float duration) {
            float moveProgress = 0;
            Vector3 orig = transform.localPosition;
            while (moveProgress < 1) {
                moveProgress += Time.deltaTime / duration;
                gameObject.transform.localPosition = Vector3.Lerp(orig, dest, moveProgress);
                yield return null;
            }
            gameObject.transform.localPosition = dest;
        }

        void Update() {
            if (Clicked != null || MouseOver != null) {
                if (GetComponent<BoxCollider>().bounds.IntersectRay(Camera.main.ScreenPointToRay(Input.mousePosition))) {
                    if (MouseOver != null)
                        MouseOver(piece);

                    if (Clicked != null && Input.GetMouseButtonDown(0))
                        Clicked(piece);
                }
            }
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Bounds bounds = GetComponent<BoxCollider>().bounds;
            if (bounds.IntersectRay(ray)) {
                Gizmos.DrawRay(ray);
                Gizmos.DrawSphere(transform.position, 0.1f);
            }
        }

        public override string ToString() {
            return string.Format("[PC piece: {0}]", piece.ToString());
        }
    }

}
