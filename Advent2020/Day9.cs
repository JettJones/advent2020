using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day9 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return FirstMissing(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            long missing = FirstMissing(input);
            return RangeEqual(input, missing);
        }

        private long FirstMissing(IEnumerable<string> input)
        {
            IEnumerable<long> values = input.Select(s => long.Parse(s));

            Queue<long> search = new Queue<long>(values.Take(25));
            foreach (long l in values.Skip(25))
            {
                if (!SumFrom(l, search.ToList()))
                {
                    return l;
                }

                search.Dequeue();
                search.Enqueue(l);
            }

            Console.WriteLine("No missing Value found");
            throw new Exception("Welp");
        }

        // answer 1  1398413738
        // too high: 2796827476  (target number in list of size 1)
        // too low:   152136112
        private long RangeEqual(IEnumerable<string> input, long target)
        {
            IEnumerable<long> values = input.Select(s => long.Parse(s));
            Queue<long> search = new Queue<long>();
            long sum = 0;
            // optimization: sum could be partially recalculated

            foreach (long v in values)
            {
                while (sum > target)
                {
                    sum -= search.Dequeue();
                }


                if (sum == target && search.Count() > 1)
                {
                    return search.Min() + search.Max();
                }

                sum += v;
                search.Enqueue(v);


                if (sum == target && search.Count() > 1)
                {
                    return search.Min() + search.Max();
                }
            }

            Console.WriteLine("No matching range found");
            throw new Exception("Welp");

        }

        private bool SumFrom(long value, IEnumerable<long> options)
        {
            HashSet<long> seen = new HashSet<long>();
            foreach (long op in options)
            {
                if (seen.Contains(value - op))
                {
                    return true;
                }

                seen.Add(op);
            }
            return false;
        }

    }
}
