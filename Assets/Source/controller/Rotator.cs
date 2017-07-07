using System.Collections;
using com.perroelectrico.demondrian.core;
using UnityEngine;

namespace com.perroelectrico.demondrian.controller {

    /// <summary>
    /// Helper for rotating the board
    /// </summary>
    public class Rotator {

        readonly Transform transform;
        readonly AnimationCurve rotationCurve;
        readonly float rotationTime;
        readonly Vector3 axis;

        float targetRotation = 0;
        float lastRotation = 0;
        float rotationProgress = 1f;

        public Rotator(Transform transform, float rotationTime, AnimationCurve rotationCurve, Vector3? axis = null) {
            this.transform = transform;
            this.rotationTime = rotationTime;
            this.rotationCurve = rotationCurve;
            this.axis = axis ?? Vector3.forward;
        }

        void Start(RotationDir dir) {
            targetRotation = (lastRotation + (int)dir) % 360;
            rotationProgress = 0f;
        }

        public IEnumerator Animate(RotationDir dir) {
            Start(dir);
            while (Rotating) {
                Update();
                yield return new WaitForEndOfFrame();
            }
        }

        void Update() {
            Progress();
            if (ShouldEnd())
                End();

            AdjustToProgress();
        }

        void AdjustToProgress() {
            float progress = rotationCurve.Evaluate(rotationProgress);
            transform.localRotation = Quaternion.AngleAxis(Mathf.LerpAngle(lastRotation, targetRotation, progress), axis);
        }

        bool ShouldEnd() {
            return rotationProgress > 1f;
        }

        void Progress() {
            rotationProgress += Time.deltaTime / rotationTime;
        }

        void End() {
            lastRotation = targetRotation;
            rotationProgress = 1f;
        }

        public bool Rotating {
            get { return targetRotation != lastRotation; }
        }


        public void Set(float angle) {
            lastRotation = targetRotation = angle;
            transform.localRotation = Quaternion.AngleAxis(angle, axis);
        }
    }
}