using System.Collections;
using System.Collections.Generic;

namespace com.perroelectrico.demondrian {

    public interface Browseable<T> : IEnumerable<T> {
        int Count { get; }
        T this[int index] { get; }
    }

    public class Browseables {
        public class Singleton<T> : Browseable<T> {

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public IEnumerator<T> GetEnumerator() {
                yield return item;
            }

            public T this[int index] {
                get { return index == 0 ? item : default(T); }
            }
            public int Count {
                get { return 1; }
            }

            readonly T item;
            public Singleton(T item) {
                this.item = item;
            }
        }
    }
}