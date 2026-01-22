namespace PamelloV7.Core.Difference
{
    public class DifferenceResult<TType>
    {
        public Dictionary<int, TType> Added { get; set; }
        public Dictionary<int, int> Moved { get; set; }
        public Dictionary<int, TType> Swapped { get; set; }
        public Dictionary<int, TType> Changed { get; set; }
        public Dictionary<int, TType> Deleted { get; set; }

        public int Changes { get; private set; }

        private DifferenceResult() {
            Added = [];
            Moved = [];
            Changed = [];
            Deleted = [];
        }

        public static DifferenceResult<TType> From(IEnumerable<TType> ebefore, IEnumerable<TType> eafter, Func<TType, TType, bool>? equals = null, bool withMoved = false) {
            var result = new DifferenceResult<TType>();

            var before = ebefore as IList<TType> ?? ebefore.ToList();
            var after = eafter as IList<TType> ?? eafter.ToList();

            equals ??= (b, a) => EqualityComparer<TType>.Default.Equals(b, a);

            var at = (IList<TType> collection, int position) => collection[position];

            var sameAt = (int posBefore, int posAfter) => {
                if (posAfter >= after.Count) return false;
                if (posBefore >= before.Count) return false;

                return equals(at(before, posBefore), at(after, posAfter));
            };
            var sameIn = (IList<TType> first, IList<TType> second, int position) => {
                return equals(at(first, position), at(second, position));
            };

            var afterCount = after.Count;
            var beforeCount = before.Count;

            int b, a, b1, a1;
            var end = false;
            for (b = 0, a = 0; b < beforeCount || a < afterCount;) {
                if (sameAt(b, a)) {
                    b++;
                    a++;
                    continue;
                }

                end = false;
                a1 = a;
                b1 = b;

                while (!end) {
                    for (a1 = a; a1 < afterCount && !sameAt(b1, a1); a1++) ;

                    if (a1 >= afterCount) {
                        b1++;
                        if (b1 >= beforeCount) end = true;
                    }
                    else {
                        end = true;
                    }
                }

                for (var i = b; i < b1 && i < beforeCount; i++) {
                    result.Deleted.Add(/*fixedPos ? b :*/ i, at(before, i));
                }
                for (var i = a; i < a1 && i < afterCount; i++) {
                    result.Added.Add(/*fixedPos ? a :*/ i, at(after, i));
                }

                a = a1;
                b = b1;
            }

            if (withMoved) {
                var addedMap = new Dictionary<TType, Queue<int>>();
                foreach (var kv in result.Added) {
                    if (!addedMap.TryGetValue(kv.Value, out var q))
                        addedMap[kv.Value] = q = new Queue<int>();
                    q.Enqueue(kv.Key);
                }

                foreach (var kv in result.Deleted.ToList()) {
                    if (addedMap.TryGetValue(kv.Value, out var q) && q.Count > 0) {
                        var newPos = q.Dequeue();
                        result.Moved.Add(kv.Key, newPos);
                        result.Added.Remove(newPos);
                        result.Deleted.Remove(kv.Key);
                    }
                }
            }

            /*
            for (int i = 0; i < result.Deleted.Count(); i++) {
                var kvp = result.Deleted.ElementAt(i);
                if (result.Added.Contains(kvp)) {
                    result.Added.Remove(kvp.Key);
                    result.Deleted.Remove(kvp.Key);
                    i--;
                }
            }
            */

            return result;
        }

        public void ExcludeMoved() {
            foreach (var key in Moved.Keys.Reverse()) {
                Deleted.Remove(key);
            }
            foreach (var value in Moved.Values) {
                Added.Remove(value);
            }
        }

        public void Apply<T>(IList<T> target, Func<TType, T> get) {
            var lastIndex = -1;
            var offset = 0;

            var add = (KeyValuePair<int, TType> kvp) => {
                if (lastIndex != kvp.Key) {
                    offset = 0;
                    lastIndex = kvp.Key;
                }

                target.Insert(kvp.Key + offset++, get(kvp.Value));
            };
            var delete = (KeyValuePair<int, TType> kvp) => {
                target.RemoveAt(kvp.Key);
            };
            var move = (KeyValuePair<int, int> kvp) => {
                var value = target[kvp.Key];

                target.RemoveAt(kvp.Key);
                target.Insert(kvp.Value - (kvp.Key < kvp.Value ? 1 : 0), value);
            };

            for (var i = Deleted.Count - 1; i >= 0; i--) {
                delete(Deleted.ElementAt(i));
            }
            for (var i = 0; i < Added.Count; i++) {
                add(Added.ElementAt(i));
            }
        }
    }
}
