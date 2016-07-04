using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;


namespace permutations_console {

    public class Permutation : IEnumerable<Cycle>
    {
        public List<Cycle> input;
        public HashSet<Cycle> disjoint_cycles;

        public int max, order, power, sign;

        public Permutation(List<Cycle> arg, int power = 1, int max = -1) {
            input = arg;
            if (input.Count == 0)
                throw new PermutationException("Empty permutation");
            if (max == -1) {
                var maxes = new List<int>();
                foreach (var c in input)
                    maxes.Add(c.original.Max());
                this.max = maxes.Max();
            } else this.max = max;

            this.power = power;
            
            disjoint_cycles = perm_disjoint(input);
            if (power != 1)
                disjoint_cycles = perm_pow(power);

            this.order = perm_order();
            this.sign = perm_sign();
        }

        public static Permutation generate(int min = 3, int max = 5) {
            var cs = new List<Cycle>();
            var rnd = StaticRandom.Instance;

            int count = rnd.Next(min, max);

            // Exactly `count` different cycles

            for (int i = 0; i < count;) {
                var cycle = Cycle.generate();
                if (!cs.Contains(cycle)) {
                    cs.Add(cycle);
                    i++;
                }
            }
            
            int power = rnd.Next(2, 501) * (rnd.Next(0, 2) * 2 - 1);
            return new Permutation(cs, power);
        }

        public static Permutation from_string(string s, int max = -1) {
            /* `s` must have one of the following formats
               [(int ...) ... (int ...)] ^ int
               (int ...) ... (int ...)
             */

            // `max` is passed to the constructor directly
            var pieces = s.Split('^');
            string cycles;
            int power = 1;

            if (pieces.Length > 2)
                throw new InvalidInput();

            if (pieces.Length == 2)
                power = int.Parse(pieces[1]);

            
            int spc = s.Count(c => c == ']' || c == '['), // Total number of square brackets
                opc = s.Count(c => c == '('), // Number of opening round brackets
                cpc = s.Count(c => c == ')'); // Number of closing round brackets

            if (spc != 0 && spc != 2)
                throw new InvalidInput("Unbalanced square brackets");

            if (opc != cpc)
                throw new InvalidInput("Unbalanced round brackets");

            // Trim whitespace before removing square brackets
            cycles = pieces[0].Trim().Trim('[', ']');

            // Extract content of each cycle
            // Find cycles with at least one digit
            var regex = new Regex(@"\(.*?\d+.*?\)");
            var matches = regex.Matches(cycles);

            if (opc != matches.Count)
                throw new InvalidInput("Too many round brackets");

            var prepped = new List<Cycle>();

            foreach (Match match in matches)
                prepped.Add(Cycle.from_string(match.Value));

            return new Permutation(prepped, power, max);
        }

        public bool is_trivial() {
            // True if this is an identity permutation
            return disjoint_cycles.Where(c => c.length > 1).Count() == 0;
        }

        public bool input_disjoint() {
            // True if `input` is a product of disjoint cycles
            var input_set = new HashSet<Cycle>(input);
            return input_set.IsSubsetOf(disjoint_cycles) &&
                   disjoint_cycles.Except(input_set).All(c => c.length == 1) &&
                   power == 1;
        }

        public bool input_transpositions() {
            // True if `input` is a product of transpositions
            return input.All(c => c.length <= 2) && power == 1;
        }

        public Permutation pow(int p) {
            return new Permutation(perm_pow(p).ToList());
        }

        public Permutation inverse() {
            return new Permutation(disjoint_cycles.ToList(), power: -1);
        }

        private HashSet<Cycle> perm_disjoint(List<Cycle> cycles) {
            var set = new SortedSet<int>(Enumerable.Range(1, max));
            var retval = new HashSet<Cycle>();
            var cur = new List<int>();

            int start, prev;
            prev = start = set.Min;

            while (set.Count != 0)
            {
                if (cur.Count > 0)
                    start = prev;
                else
                    start = prev = set.Min;

                cur.Add(start);

                prev = maps_to(cycles, prev);

                // Close the current cycle
                if (cur[0] == prev) {
                    retval.Add(new Cycle(cur.ToArray()));
                    cur.Clear();
                }
                set.Remove(start);
            }
            return retval;
        }

        private HashSet<Cycle> perm_pow(int p) {
            var retval = new HashSet<Cycle>();
            int mod;
            foreach (var c in this) {
                var temp = c;
                mod = Math.Abs(p) % c.length;
                temp = (p < 0) ? c.reversed() : c;

                var result = new List<Cycle>();

                for (int i = 0; i < mod; i++)
                    result.Add(temp);

                retval.UnionWith(perm_disjoint(result));
            }
            return perm_disjoint(retval.ToList());
        }

        private int perm_order() {
            for (int i = 1;;i++)
                if (disjoint_cycles.All(c => i % c.length == 0))
                    return i;
        }

        private int perm_sign() {
            return (int) Math.Pow(-1, disjoint_cycles.Sum(c => c.length - 1));
        }

        private SortedDictionary<int, int> perm_map() {
            var retval = new SortedDictionary<int, int>();
            foreach (var i in Enumerable.Range(1, max))
                retval.Add(i, maps_to(disjoint_cycles.ToList(), i));
            return retval;
        }

        private int maps_to(List<Cycle> cs, int source) {
            for (int i = cs.Count - 1; i >= 0; i--) {
                try { source = cs[i][source]; }
                catch (KeyNotFoundException) { };
            }
            return source;
        }

        public string permutation_rules() {
            return String.Join(" | ", perm_map().Select(p => String.Format("{0} -> {1}", p.Key, p.Value)));
        }

        public string one_line_notation() {
            // Mainly for debugging
            return String.Join(" ", perm_map().Select(p => p.Value));
        }

        public string format_input() {
            // "Cleaned" version of input
            string s = String.Join("", input.Select(c => c.format_input()));
            if (power != 1)
                return String.Format("[{0}] ^ {1}", s, power);
            return s;
        }

        public override string ToString() {
            return String.Join("", disjoint_cycles);
        }

        public static bool operator ==(Permutation a, Permutation b) {
           if (Object.ReferenceEquals(a, b))
                return true;
           else if (((object)a == null) && ((object)b == null)) // ?
               return true;
           else if (((object)a == null) || ((object)b == null))
                return false;
           else return a.disjoint_cycles.SetEquals(b.disjoint_cycles);
        }
        
        public static bool operator !=(Permutation a, Permutation b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            Permutation other = obj as Permutation;
            if ((object)other != null)
                return this == other;
            return false;
        }

        public IEnumerator<Cycle> GetEnumerator()
        {
            return disjoint_cycles.GetEnumerator();
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
                foreach (var c in this)
                    hash = (16777619 * hash) ^ c.GetHashCode();
                return hash;
            }
        }
    }
}
