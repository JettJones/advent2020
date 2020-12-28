using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day25 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return GuessSharedKey(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            throw new NotImplementedException();
        }

        private long GuessSharedKey(IEnumerable<string> input)
        {
            var ints = input.Select(Int32.Parse).Take(2).ToList();

            List<int> counts = CountLoops(ints);

            return Transform(ints[0], counts[1]);
        }

        private List<int> CountLoops(List<int> ints)
        {
            Dictionary<long, int> seek = new Dictionary<long, int>();

            for (int j = 0; j < ints.Count; j++)
            {
                seek[ints[j]] = j;
            }

            int[] result = new int[ints.Count];

            int count = 0;
            long subject = 7;
            long value = 1;
            int ixOut;
            while (seek.Count > 0)
            {
                value = value * subject;
                value = value % 20201227;
                count++;

                if (seek.TryGetValue(value, out ixOut))
                {
                    result[ixOut] = count;
                    seek.Remove(value);
                }
            }

            return new List<int>(result);
        }

        private long Transform(int subject, int loop)
        {
            long value = 1;
            for(int i=0; i < loop; i++)
            {
                value = value * subject;
                value = value % 20201227;
            }

            return value;
        }
    }
}
