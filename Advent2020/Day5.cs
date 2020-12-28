using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2020
{
    class Day5 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return this.HighestID(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return this.MissingSeat(input);
        }

        public int HighestID(IEnumerable<string> input)
        {
            return input.Select(ToID).Max();
        }

        public int MissingSeat(IEnumerable<string> input)
        {
            var allIDs = input.Select(ToID);
            var high = allIDs.Max();
            var low = allIDs.Min();

            var allSeats = Enumerable.Range(low, high).ToHashSet();

            foreach (int s in allIDs)
            {
                allSeats.Remove(s);
            }

            return allSeats.First();
        }

        private int ToID(string input)
        {
            string binary = input.Replace("F", "0").Replace("B", "1").Replace("L", "0").Replace("R", "1");
            return Convert.ToInt32(binary, 2);

        }
    }
}
