using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Advent2020
{
    class Day2 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return this.CountValid(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return this.CountValid_B(input);
        }

        struct LineDetail
        {
            public int Min;
            public int Max;
            public string Letter;
            public string Input;

        }

        public int CountValid(IEnumerable<string> lines)
        {
            int validCount = 0;

            foreach (string l in lines)
            {
                LineDetail detail = GetDetail(l);

                if (IsValid(detail))
                {
                    validCount++;
                }
            }

            return validCount;

        }

        public int CountValid_B(IEnumerable<string> lines)
        {
            int validCount = 0;

            foreach (string l in lines)
            {
                LineDetail detail = GetDetail(l);

                if (IsValid_B(detail))
                {
                    validCount++;
                }
            }

            return validCount;

        }

        private LineDetail GetDetail(string line)
        {
            var r = new Regex("([0-9]+)-([0-9]+) ([a-z]): (.*)");
            var m = r.Match(line);

            return new LineDetail {
                Min = Int32.Parse(m.Groups[1].Value),
                Max = Int32.Parse(m.Groups[2].Value),
                Letter = m.Groups[3].Value,
                Input = m.Groups[4].Value
            };
        }

        private bool IsValid(LineDetail d)
        {
            int matchCount = d.Input.Where(c => d.Letter.Contains(c)).Count();
            bool valid = (d.Min <= matchCount && d.Max >= matchCount);
            if (!valid)
            {
                Console.WriteLine("Invalid: {0} - MatchCount: {1} vs {2}-{3} {4}", d.Input, matchCount, d.Min, d.Max, d.Letter);
            }
            return valid;
        }

        private bool IsValid_B(LineDetail d)
        {
            char low = d.Input.ElementAtOrDefault(d.Min - 1);
            char high = d.Input.ElementAtOrDefault(d.Max - 1);

            bool valid = d.Letter.Contains(low) ^ d.Letter.Contains(high);
            if (valid)
            {
                Console.WriteLine("YES: {0} - '{1}+{2}' at {3}-{4} {5}", d.Input, low, high, d.Min, d.Max, d.Letter);
            }
            return valid;
        }
    }
}
