using System;
using System.Collections.Generic;

namespace com.perroelectrico.demondrian.util {

    public class RandomPicker<T> {
        private Random rnd = new Random();

        private Dictionary<T, double> items = new Dictionary<T, double>();
        private double totalWeight = 0d;

        public int Count {
            get { return items.Count; }
        }

        /// <summary>
        /// Will only add items with weight >= 0
        /// </summary>
        public void Add(T obj, double weight) {
            if (weight <= 0d)
                return;
            items.Add(obj, weight);
            totalWeight += weight;
        }

        public void Clear() {
            items.Clear();
        }

        public void Remove(T obj) {
            totalWeight -= items[obj];
            items.Remove(obj);
        }

        public T Get() {
            double r = rnd.NextDouble() * totalWeight;
            double sum = 0d;
            foreach (var o_w in items) {
                sum += o_w.Value;
                if (sum >= r)
                    return o_w.Key;
            }
            throw new Exception("Random Picker Invalid state: none chosen");
        }
    }
}