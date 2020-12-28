using System;
using System.Collections.Generic;
using System.Text;

namespace Advent2020
{
    class Day3 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return this.TreeCount(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return this.TreeCountMany(input);
        }

        public int TreeCount(IEnumerable<string> input)
        {
            return (int)TreeCountSlope(input, 3);
        }

        private long TreeCountSlope(IEnumerable<string> input, int xDelta, int yDelta = 1)
        {
            int count = 0;

            int xCur = 0;
            int yCur = 0;
            
            foreach (string l in input)
            {
                if (yCur % yDelta == 0)
                {
                    var ix = xCur % l.Length;
                    if (l[ix] == '#')
                    {
                        count++;
                    }

                    xCur += xDelta;
                }
                yCur++;
            }

            return count;
        }

        public long TreeCountMany(IEnumerable<string> input)
        {
            long r1d1 = TreeCountSlope(input, 1);
            long r3d1 = TreeCountSlope(input, 3);
            long r5d1 = TreeCountSlope(input, 5);
            long r7d1 = TreeCountSlope(input, 7);
            long r1d2 = TreeCountSlope(input, 1, 2);

            return r1d1 * r3d1 * r5d1 * r7d1 * r1d2;
        }

    }
}
