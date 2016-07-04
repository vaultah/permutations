using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace permutations_console {

    public class Cycle : IEnumerable<int> {
        private Dictionary<int, int> map;
        public List<int> ordered, original;

        public Cycle(params int[] args) {
            if (args.Length == 0)
                throw new CycleException("Empty cycle");
            original = new List<int>(args);
            ordered = rotated_left(Array.IndexOf(args, args.Min()));

            // Build the map
            map = new Dictionary<int, int>();
            try {
                foreach (var pair in ordered.Zip(ordered.Skip(1), Tuple.Create))
                    map.Add(pair.Item1, pair.Item2);
                map.Add(ordered[ordered.Count - 1], ordered[0]);
            } catch (ArgumentException e) {
                // fix plox
                if (e.Message != "An item with the same key has already been added.")
                    throw;
                throw new CycleException("Duplicate element");
            }
        }

        public static Cycle generate(int min = 1, int max = 9, int min_count = 3, int max_count = 7) {
            var rnd = StaticRandom.Instance;
            var nums = Enumerable.Range(min, max).OrderBy(x => rnd.NextDouble())
                                                 .Take(rnd.Next(min_count, max_count))
                                                 .ToArray();
            return new Cycle(nums);
        }

        public static Cycle from_string(string s) {
            s = s.Trim();                          
            if (!s.EndsWith(")") || !s.StartsWith("(") || s.Count(c => c == ')' || c == '(') != 2)
                throw new InvalidInput("Cycles must be surrounded by 2 round brackets");
            // Trim whitespace before trimming brackets
            var pieces = s.Trim('(', ')')
                          .Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return new Cycle(pieces.Select(int.Parse).ToArray());
        }

        public int this[int key] {
            get { return map[key]; }
        }

        public int length {
            get { return original.Count; }
        }

        public Cycle reversed()
        {
            var temp = new List<int>(original);
            temp.Reverse();
            return new Cycle(temp.ToArray());
        }

        public List<int> rotated_left(int pos) {
            var temp = new LinkedList<int>(original);
            for (int i = 0; i < pos; i++) {
                var first = temp.First;
                temp.RemoveFirst();
                temp.AddLast(first);
            }
            return temp.ToList();
        }

        public List<int> rotated_right(int pos)
        {
            var temp = new LinkedList<int>(original);
            for (int i = 0; i < pos; i++) {
                var last = temp.Last;
                temp.RemoveLast();
                temp.AddFirst(last);
            }

            return temp.ToList();
        }

        public string format_input()
        {
            // "Cleaned" version of input
            return String.Format("({0})", String.Join(" ", original));
        }

        public override string ToString() {
            return String.Format("({0})", String.Join(" ", ordered));
        }

        public override bool Equals(object obj) {
            Cycle other = obj as Cycle;
            if ((object)other != null)
                return this == other;
            return false;
        }

        public static bool operator ==(Cycle a, Cycle b) {
           if (Object.ReferenceEquals(a, b))
                return true;
           else if (((object)a == null) && ((object)b == null)) // ?
               return true;
           else if (((object)a == null) || ((object)b == null))
                return false;
           else return a.ordered.SequenceEqual(b.ordered);
        }

        public static bool operator !=(Cycle a, Cycle b) {
            return !(a == b);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return original.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode() {
            // Adapted from http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx#fnv
            unchecked
            {
                int hash = (int) 2166136261;
                foreach (var i in ordered)
                    hash = (16777619 * hash) ^ i.GetHashCode();
                return hash;
            }
        }
    }
}
