using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day10 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return JoltDifferences(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return JoltPossibles(input);
        }

        // too low: 2070 - forgot to include 0.
        public int JoltDifferences(IEnumerable<string> input)
        {
            List<int> values = input.Select(s => Int32.Parse(s)).ToList();
            values.Sort();
            values.Add(values.Last() + 3);

            int diff1 = 0;
            int diff3 = 0;

            int prev = 0;
            foreach(int val in values)
            {
                int d = val - prev;
                if (d == 1) { diff1++; }
                if (d == 3) { diff3++; }
                prev = val;
            }

            return diff1 * diff3;

        }

        private long JoltPossibles(IEnumerable<string> input)
        {
            List<int> values = input.Select(s => Int32.Parse(s)).ToList();
            values.Sort();
            values.Insert(0, 0);
            values.Add(values.Last() + 3);

            Dictionary<int, long> counts = new Dictionary<int, long>();
            counts[0] = 1;


            // number of ways to get to X is the sum of paths to X-1, x-2 and x-3
            // alternative: count paths between 3s, multiply together.
            foreach (int val in values)
            {
                AddTo(counts, val + 1, counts[val]);
                AddTo(counts, val + 2, counts[val]);
                AddTo(counts, val + 3, counts[val]);
            }

            return counts[values.Last()];

        }

        private void AddTo(Dictionary<int, long> counts, int key, long val)
        {
            if (!counts.ContainsKey(key)) { counts[key] = 0; }

            counts[key] += val;
        }
    }

}
