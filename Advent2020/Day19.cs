using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day19 : DayInterface
    {
        public interface IRule
        {
            public HashSet<string> Match(Dictionary<int, IRule> lookup, string input);
        }

        public class ListRule : IRule
        {
            private readonly List<int> rules;

            public ListRule(IEnumerable<int> keys)
            {
                this.rules = new List<int>(keys);
            }

            public HashSet<string> Match(Dictionary<int, IRule> lookup,  string input)
            {
                HashSet<string> result = new HashSet<string>() { "" };
                foreach(IRule r in this.rules.Select(i => lookup[i]))
                {
                    HashSet<string> next = new HashSet<string>();
                    foreach (string s in result)
                    {
                        string remain = input.Substring(s.Length);

                        var stepResult = r.Match(lookup, remain);

                        foreach (string sr in stepResult)
                        {
                            next.Add(s + sr);
                        }
                    }

                    result = next;

                }

                return result;
            }

        }

        public class OrRule : IRule
        {
            private readonly List<IRule> rules;

            public OrRule(IEnumerable<IRule> rules)
            {
                this.rules = new List<IRule>(rules);
            }

            public HashSet<string> Match(Dictionary<int, IRule> lookup, string input)
            {
                HashSet<string> result = new HashSet<string>();

                foreach (IRule r in this.rules)
                {
                    foreach(string sr in r.Match(lookup, input))
                    {
                        result.Add(sr);
                    }
                }
                return result;
            }
        }

        public class LetterRule : IRule
        {
            private static readonly HashSet<string> EMPTY = new HashSet<string>();
            private readonly string letter;

            public LetterRule(string l)
            {
                this.letter = l;
            }
            public HashSet<string> Match(Dictionary<int, IRule> _, string input)
            {
                if (input.StartsWith(this.letter))
                {
                    return new HashSet<string>() { this.letter };
                }

                return EMPTY;
            }
        }


        public object SolveA(IEnumerable<string> input)
        {
            return MatchRule(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return MatchModified(input);
        }

        private ListRule ToListRule(string s)
        {
            return new ListRule(s.Split(" ").Select(Int32.Parse));
        }

        private int MatchModified(IEnumerable<string> input)
        {
            Dictionary<int, IRule> rules;
            List<string> strings;
            MyParse(input, out rules, out strings);

            // Change the definition of 8 and 11.
            var r8 = new List<IRule>()
            {
                new ListRule(new int[]{42}),
                new ListRule(new int[]{42, 8}),
            };
            rules[8] = new OrRule(r8);

            var r11 = new List<IRule>()
            {
                new ListRule(new int[]{42, 31}),
                new ListRule(new int[]{42, 11, 31}),
            };

            rules[11] = new OrRule(r11);

            int success = CountRule0Matches(rules, strings);

            return success;
        }

        private int MatchRule(IEnumerable<string> input)
        {
            Dictionary<int, IRule> rules;
            List<string> strings;
            MyParse(input, out rules, out strings);

            int success = CountRule0Matches(rules, strings);

            return success;
        }

        private static int CountRule0Matches(Dictionary<int, IRule> rules, List<string> strings)
        {
            IRule r0 = rules[0];

            int success = 0;

            foreach (string ins in strings)
            {
                var matches = r0.Match(rules, ins);
                if (matches.Where(m => m.Length == ins.Length).Count() > 0)
                {
                    success++;
                }
            }

            return success;
        }
        private void MyParse(IEnumerable<string> input, out Dictionary<int, IRule> rules, out List<string> strings)
        {
            rules = new Dictionary<int, IRule>();
            strings = new List<string>();
            bool startStrings = false;
            foreach (string s in input)
            {
                if (String.IsNullOrWhiteSpace(s))
                {
                    startStrings = true;
                    continue;
                }

                if (startStrings)
                {
                    strings.Add(s);
                }
                else
                {
                    var indexSplit = s.Split(": ");
                    int index = Int32.Parse(indexSplit[0]);
                    string rest = indexSplit[1];
                    if (rest.StartsWith("\""))
                    {
                        rules.Add(index, new LetterRule(rest.Trim('"')));
                    }
                    else
                    {
                        var lists = rest.Split(" | ");
                        if (lists.Length == 1)
                        {
                            rules.Add(index, ToListRule(rest));
                        }
                        else
                        {
                            rules.Add(index, new OrRule(lists.Select(ToListRule)));
                        }
                    }
                }
            }
        }
    }
}
