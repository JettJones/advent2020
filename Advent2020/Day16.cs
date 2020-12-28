using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day16 : DayInterface
    {

        private enum ParseStep
        {
            Fields,
            Yours,
            Nearby,
        }

        private struct TicketData
        {
            public Dictionary<string, List<Tuple<int, int>>> Fields;
            public List<int> Yours;
            public List<List<int>> Nearby;
        }
        public object SolveA(IEnumerable<string> input)
        {
            return ErrorSum(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return DepartureProduct(input);
        }


        private int ErrorSum(IEnumerable<string> input)
        {
            var parsed = MyParse(input);

            int minErr = parsed.Fields.Values.Select(f => f.First().Item1).Min();
            int maxErr = parsed.Fields.Values.Select(f => f.Last().Item2).Max();

            int sum = 0;

            Dictionary<int, HashSet<string>> possibleMap = new Dictionary<int, HashSet<string>>();
            for(int i=0; i < parsed.Yours.Count; i++)
            {
                possibleMap[i] = parsed.Fields.Keys.ToHashSet();
            }

            foreach (var tix in parsed.Nearby)
            {
                for (int i = 0; i < parsed.Yours.Count; i++)
                {
                    HashSet<string> broke = new HashSet<string>();
                }
            }

            return sum;
        }

        private long DepartureProduct(IEnumerable<string> input)
        {
            var parsed = MyParse(input);

            int minErr = parsed.Fields.Values.Select(f => f.First().Item1).Min();
            int maxErr = parsed.Fields.Values.Select(f => f.Last().Item2).Max();

            var legal = parsed.Nearby.Where(tix => tix.Where(i => i < minErr || i > maxErr).Count() == 0).Append(parsed.Yours).ToList();

            var matches = new Dictionary<int, HashSet<string>>();


            for (int i = 0; i < parsed.Yours.Count; i++)
            {
                matches[i] = parsed.Fields.Keys.ToHashSet();
            }

            foreach (var tix in legal)
            {
                for (int i = 0; i < parsed.Yours.Count; i++)
                {
                    var fieldSet = matches[i].ToList();
                    foreach (var fieldName in fieldSet)
                    {
                        var field = parsed.Fields[fieldName];
                        if (!IsLegal(field, tix[i]))
                        {
                            Console.WriteLine("Removing {0} for value {1}", fieldName, tix[i]);
                            matches[i].Remove(fieldName);
                        }
                    }
                }
            }

            // now we have all the legal matches.
            Queue<int> changes = new Queue<int>();
            for (int i = 0; i < parsed.Yours.Count; i++)
            {
                if (matches[i].Count == 1)
                {
                    changes.Enqueue(i);
                }
            }

            while(changes.Count() > 0)
            {
                int checkMe = changes.Dequeue();
                string value = matches[checkMe].First();
                for (int i = 0; i < parsed.Yours.Count; i++)
                {
                    if (i == checkMe) { continue; }
                    if (matches[i].Remove(value) && matches[i].Count == 1)
                    {
                        changes.Enqueue(i);
                    }
                }
            }

            // logically reduced
            long product = 1;
            for (int i = 0; i < parsed.Yours.Count; i++)
            {
                if (matches[i].First().StartsWith("departure"))
                {
                    product = product * parsed.Yours[i];
                }
            }

            return product;
        }

        private bool IsLegal(List<Tuple<int, int>> field, int v)
        {
            foreach (var tup in field)
            {
                if (tup.Item1 > v)
                {
                    return false;
                }

                if (tup.Item2 >= v)
                {
                    return true;
                }
            }
            return false;
        }

        private TicketData MyParse(IEnumerable<string> input)
        {
            var fields = new Dictionary<string, List<Tuple<int, int>>>();
            List<int> yours = null;
            var nearby = new List<List<int>>();


            ParseStep step = ParseStep.Fields;

            foreach(string s in input)
            {
                switch(step)
                {
                    case ParseStep.Fields:
                        {
                            if (String.IsNullOrWhiteSpace(s))
                            {
                                step = ParseStep.Yours;
                                continue;
                            }

                            var nameSplit = s.Split(":");
                            var rangeSplit = nameSplit[1].Split(" or ");
                            List<Tuple<int, int>> parsed = new List<Tuple<int, int>>();
                            foreach(string r in rangeSplit)
                            {
                                var vals = r.Split("-").Select(s => Int32.Parse(s)).ToList();
                                parsed.Add(new Tuple<int, int>(vals[0], vals[1]));
                            }
                            fields.Add(nameSplit[0], parsed);
                            break;
                        }
                    case ParseStep.Yours:
                        {
                            if (String.IsNullOrWhiteSpace(s))
                            {
                                step = ParseStep.Nearby;
                                continue;
                            }

                            if (s == "your ticket:")
                            {
                                continue;
                            }

                            yours = s.Split(",").Select(s => Int32.Parse(s)).ToList();

                            break;
                        }
                    case ParseStep.Nearby:
                        {
                            if (s == "nearby tickets:")
                            {
                                continue;
                            }

                            var tix = s.Split(",").Select(s => Int32.Parse(s)).ToList();
                            nearby.Add(tix);

                            break;
                        }
                    default:
                        Console.WriteLine("Invalid step: " + step);
                        break;
                }
            }

            return new TicketData()
            {
                Fields = fields,
                Yours = yours,
                Nearby = nearby,
            };
        }

    }
}
