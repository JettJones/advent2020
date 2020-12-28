using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2020
{
    class Day15 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return Sim2020(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return Sim2020(input, max: 30 * 1000 * 1000);
        }

        public long Sim2020(IEnumerable<string> input, int max=2020)
        {
            var seeds = input.First().Split(",").Select(s => Int32.Parse(s));

            Dictionary<long, long> lastSeen = new Dictionary<long, long>();

            long ix = 1;
            long spoken = seeds.First();
            
            foreach (int i in seeds.Skip(1))
            {
                lastSeen[spoken] = ix;

                spoken = i;
                ix++;
            }

            while (ix < max)
            {
                long nxt;
                if (lastSeen.ContainsKey(spoken))
                {
                    nxt = ix - lastSeen[spoken];
                }
                else
                {
                    nxt = 0;
                }

                if (ix % 1000000 == 0)
                {
                    Console.WriteLine("[{0}] heard {1} spoke {2}", ix + 1, spoken, nxt);
                }
                lastSeen[spoken] = ix;
                spoken = nxt;
                ix++;
            }

            return spoken;
        }
    }
}
