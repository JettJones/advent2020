using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day13 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return FirstBus(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return CRTBus(input);
        }

        private int FirstBus(IEnumerable<string> input)
        {
            var list = input.ToList();
            int startTime = Int32.Parse(list.First());

            IEnumerable<int> buses = list[1].Split(",").Where(s => s != "x").Select(s => Int32.Parse(s));

            int min = Int32.MaxValue;
            int bus = 0;

            foreach(int b in buses)
            {
                int div = startTime / b;
                if ( (div * b) < startTime)
                {
                    int next = (div + 1) * b;
                    int delta = next - startTime;

                    if (delta < min)
                    {
                        bus = b;
                        min = delta;
                    }
                }
                else
                {
                    Console.WriteLine("exact!");
                }
            }

            return min * bus;
        }

        // too high: 1182264408870216
        // too low:   440514293298069
        private long CRTBus(IEnumerable<string> input)
        {
            var list = input.ToList();
            int startTime = Int32.Parse(list.First());

            List<Tuple<int, int>> buses = list[1].Split(",")
                .Select((s, i) => new Tuple<String, int>(s, i))
                .Where(t => t.Item1 != "x")
                .Select(t => new Tuple<int, int>(Int32.Parse(t.Item1), t.Item2))
                .ToList();

            // sort descending
            buses.Sort((t1, t2) => t2.Item1.CompareTo(t1.Item1));

            // Translate to Chinese Remainder Theorem
            // we seek a value V, where the later modulo V + Item2 % Item1 == 0
            // so the modulo at V is Item1 - Item2.
            var offsets = buses.Select(t => new Tuple<int, int>(t.Item1, (t.Item1 - (t.Item2 % t.Item1)) % t.Item1));

            long value = CRT(offsets);

            foreach (var pair in buses)
            {
                Console.WriteLine("Value + {0} % {1} = {2}", pair.Item2, pair.Item1, (value + pair.Item2) % pair.Item1);
            }


            return value;
        }

        private long CRT(IEnumerable<Tuple<int, int>> input)
        {
            long increment = 1;
            long value = 0;

            foreach (var pair in input)
            {
                long max = increment * pair.Item1;
                long target = pair.Item2;

                //Console.WriteLine("=====Testing for % {0} = {1} by {2}", pair.Item1, target, increment);

                while (value < max)
                {
                    long mod = value % (long)pair.Item1;

                    if (mod == target)
                    {
                        // Console.WriteLine("== Success for key {0} : {1}", pair.Item1, value);
                        break;
                    }
                    value += increment;
                }

                increment = max;
            }

            return value;
        }
    }
}
