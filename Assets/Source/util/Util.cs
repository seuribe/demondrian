using System;
using System.Collections;
using System.Collections.Generic;

namespace com.perroelectrico.demondrian.util {
    public class Util {

        public static IEnumerator Wait(System.TimeSpan ts) {
            yield return new UnityEngine.WaitForSeconds((float)ts.TotalSeconds);
        }

        public static float Random(float min, float max) {
            return UnityEngine.Random.Range(min, max);
        }

        public static float GetGlobalXScale(UnityEngine.Transform transform) {
            float scale = 1;
            do {
                scale *= transform.localScale.x;
                transform = transform.parent;
            } while (transform != null);
            return scale;
        }

        public List<T> Shuffle<T>(List<T> orig) {
            var rnd = new System.Random();
            var a = new List<T>(orig);
            var b = new List<T>();
            for (int i = 0 ; i < 5 ; i++) {
                while (a.Count > 0) {
                    int index = rnd.Next(a.Count);
                    b.Add(a[index]);
                    a.RemoveAt(index);
                }
                var temp = a;
                a = b;
                b = temp;
            }
            return a;
        }
    }

    public class OneTimeSwitch {
        readonly object useLock = new object();
        bool used;

        public bool Get() {
            lock (useLock) {
                return !used && (used = true);
            }
        }
    }

    public static class TimeSpanExtensions {
        public static TimeSpan AddSeconds(this TimeSpan ts, float seconds) {
            return ts.Add(TimeSpan.FromSeconds(seconds));
        }
    }
}