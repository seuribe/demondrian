using System.Collections.Generic;
using System.Linq;
using com.perroelectrico.demondrian.controller;
using com.perroelectrico.demondrian.util;

namespace com.perroelectrico.demondrian.core {

    /// <summary>
    /// The queue of pieces; will be used one by one on each play
    /// </summary>
    public abstract class IncomingQueue {

        private class DeterministicQueue : IncomingQueue {
            private PieceType[] types;
            private int nextTypeIndex = -1;

            public DeterministicQueue(params PieceType[] types) {
                this.types = types;
            }

            protected override PieceType NextType() {
                nextTypeIndex = (nextTypeIndex + 1) % types.Length;
                return types[nextTypeIndex];
            }

            public override void Reset() {
                nextTypeIndex = -1;
            }

            public override int LookAheadSize() {
                return types.Length;
            }
        }

        private class RandomQueue : IncomingQueue {
            RandomPicker<PieceType> randomPicker = new RandomPicker<PieceType>();

            public RandomQueue(GameRules rules, PieceType[] types) {
                randomPicker.Add(PieceType.MatchAll, rules.MatchAllWeight);
                randomPicker.Add(PieceType.MatchNone, rules.MatchNoneWeight);
                foreach (var type in types)
                    randomPicker.Add(type, rules.OtherPiecesWeight / (double)types.Length);
            }

            protected override PieceType NextType() {
                return randomPicker.Get();
            }

            public override int LookAheadSize() {
                return randomPicker.Count;
            }
        }

        private class NoQueue : IncomingQueue {
            public override int LookAheadSize() {
                return 0;
            }

            protected override PieceType NextType() {
                return null;
            }

            public override PieceType Next() {
                return null;
            }
        }

        public static IncomingQueue GetQueue(GameRules rules, params PieceType[] types) {
            switch (rules.NextTypePolicy) {
                case NextTypePolicy.Deterministic:
                    return new DeterministicQueue(types);
                case NextTypePolicy.Random:
                    return new RandomQueue(rules, types);
                case NextTypePolicy.RemovedType:
                    return new NoQueue();
            }
            throw new InvalidGameOperationException("Unknown next type policy: " + rules.NextTypePolicy);
        }

        private Queue<PieceType> lookAheadQueue = new Queue<PieceType>();
        private Stack<PieceType> history = new Stack<PieceType>();

        public virtual PieceType Next() {
            FillLookAhead();
            var next = lookAheadQueue.Dequeue();
            history.Push(next);
            return next;
        }

        public virtual void Reset() { }
        public void Undo() {
            var newQueue = new Queue<PieceType>();
            newQueue.Enqueue(history.Pop());
            foreach (var type in lookAheadQueue)
                newQueue.Enqueue(type);

            lookAheadQueue = newQueue;
        }

        protected abstract PieceType NextType();
        public abstract int LookAheadSize();

        void ClearLookAhead() {
            lookAheadQueue.Clear();
        }

        void FillLookAhead() {
            while (lookAheadQueue.Count < LookAheadSize())
                lookAheadQueue.Enqueue(NextType());
        }

        public List<PieceType> Incoming() {
            FillLookAhead();
            return new List<PieceType>(lookAheadQueue.Take(LookAheadSize()));
        }

        public PieceType PeekNext() {
            return Incoming()[0];
        }
    }
}