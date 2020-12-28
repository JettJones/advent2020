using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day6 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return GroupSum(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return GroupSumEveryone(input);
        }

        public int GroupSum(IEnumerable<string> input)
        {
            var grouping = SplitGroups(input);
            var groups = Anyone(grouping);

            var groupCount = groups.Select(hs => hs.Count());

            return groupCount.Sum();
        }

        public int GroupSumEveryone(IEnumerable<string> input)
        {
            var grouping = SplitGroups(input);
            var groups = Everyone(grouping);

            var groupCount = groups.Select(hs => hs.Count());

            return groupCount.Sum();
        }

        private IEnumerable<List<string>> SplitGroups(IEnumerable<string> input)
        {
            List<string> current = new List<string>();

            foreach (string l in input)
            {
                if (String.IsNullOrWhiteSpace(l))
                {
                    yield return current;
                    current = new List<string>();
                    continue;
                }

                current.Add(l);
            }

            yield return current;

        }


        private IEnumerable<HashSet<char>> Anyone(IEnumerable<List<string>> groups)
        {
            foreach (var g in groups)
            {
                HashSet<char> forGroup = new HashSet<char>();
                foreach (string l in g)
                {
                    foreach (char c in l)
                    {
                        forGroup.Add(c);
                    }
                }
                yield return forGroup;
            }
        }
        private IEnumerable<HashSet<char>> Everyone(IEnumerable<List<string>> groups)
        {
            foreach (var g in groups)
            {
                HashSet<char> current = null;
                foreach (string line in g)
                {
                    HashSet<char> forLine = new HashSet<char>();
                    foreach (char c in line)
                    {
                        forLine.Add(c);
                    }

                    if (current == null) { current = forLine; }
                    else { current.IntersectWith(forLine); }

                }
                yield return current;
            }
        }

    }
}
