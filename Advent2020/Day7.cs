using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day7 : DayInterface
    {
        struct BagGraph
        {
            public List<string> Names;
            public Dictionary<string, List<Tuple<int, string>>> Contains;
            public Dictionary<string, List<string>> ContainedBy;
        }


        public object SolveA(IEnumerable<string> input)
        {
            return CountWithGold(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return CountInGold(input);
        }


        public int CountWithGold(IEnumerable<string> input)
        {
            BagGraph graph = ParseInput(input);

            return CanContain(graph, "shiny gold").Count();
        }

        public long CountInGold(IEnumerable<string> input)
        {
            BagGraph graph = ParseInput(input);

            return CountContent(graph, "shiny gold");
        }

        private long CountContent(BagGraph graph, string key)
        {
            long sum = 0;
            foreach(var tup in graph.Contains[key])
            {
                sum += tup.Item1;
                sum += tup.Item1 * CountContent(graph, tup.Item2);
            }
            return sum;
        }


        private List<string> CanContain(BagGraph graph, string key)
        {
            HashSet<string> seen = new HashSet<string>();
            Queue<string> checking = new Queue<string>();
            checking.Enqueue(key);

            while (checking.Count() > 0)
            {
                string active = checking.Dequeue();

                foreach (string s in graph.ContainedBy[active])
                {
                    if (!seen.Contains(s))
                    {
                        checking.Enqueue(s);
                        seen.Add(s);
                    }
                }
            }

            return seen.ToList();
        }

        private BagGraph ParseInput(IEnumerable<string> input)
        {
            HashSet<string> names = new HashSet<string>();
            var forward = new Dictionary<string, List<Tuple<int, string>>>();
            var backward = new Dictionary<string, List<string>>();

            foreach(string l in input)
            {
                string trimmed = l.TrimEnd('.');
                // ... bags contain N ... bags, N ... bags, ... (repeating)

                var nameContent = trimmed.Split(" bags contain ", 2);
                string name = nameContent[0];

                if (!names.Contains(name))
                {
                    names.Add(name);
                    forward[name] = new List<Tuple<int, string>>();
                    backward[name] = new List<string>();
                }

                var content = nameContent[1].Split(", ");
                var contents = content.Select(c => c.Replace(" bags", "").Replace(" bag", ""));
                // N ...
                foreach (var c in contents)
                {
                    if (c == "no other") { continue; }

                    var countName = c.Split(" ", 2);
                    var contentCount = Int32.Parse(countName[0]);
                    var contentName = countName[1];

                    if (!names.Contains(contentName))
                    {
                        names.Add(contentName);
                        forward[contentName] = new List<Tuple<int, string>>();
                        backward[contentName] = new List<string>();
                    }

                    forward[name].Add(new Tuple<int, string>(contentCount, contentName));
                    backward[contentName].Add(name);
                }
            }

            return new BagGraph()
            {
                Contains = forward,
                ContainedBy = backward,
                Names = names.ToList()
            };

        }

    }
}
