using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day24 : DayInterface
    {
        enum Dir
        {
            E,
            W,
            SE,
            SW,
            NE,
            NW,
        }
        public object SolveA(IEnumerable<string> input)
        {
            return CountBlack(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return BlackLife(input);
        }

        private int CountBlack(IEnumerable<string> input)
        {
            HashSet<Tuple<int, int>> black = ParseTiles(input);
            return black.Count;
        }

        private HashSet<Tuple<int, int>> ParseTiles(IEnumerable<string> input)
        {
            HashSet<Tuple<int, int>> black = new HashSet<Tuple<int, int>>();
            foreach (string line in input)
            {
                IEnumerable<Dir> dirs = ParseLine(line);

                Tuple<int, int> coord = ToCoord(dirs);

                if (black.Contains(coord))
                {
                    black.Remove(coord);
                }
                else
                {
                    black.Add(coord);
                }

            }

            return black;
        }

        private int BlackLife(IEnumerable<string> input, int days = 100)
        {
            HashSet<Tuple<int, int>> black = ParseTiles(input);

            for(int i = 0; i < days; i++)
            {
                black = SimDay(black);
            }
            return black.Count;
        }

        private HashSet<Tuple<int, int>> SimDay(HashSet<Tuple<int, int>> black)
        {
            Dictionary<Tuple<int, int>, int> neighbors = new Dictionary<Tuple<int, int>, int>();

            List<List<Dir>> allDirs = AllDirs();

            foreach(var tuple in black)
            {
                foreach(var dir in allDirs)
                {
                    var offset = ToCoord(dir);
                    var n = new Tuple<int, int>(tuple.Item1 + offset.Item1, tuple.Item2 + offset.Item2);
                    if (!neighbors.ContainsKey(n)) { neighbors.Add(n, 0); }

                    neighbors[n]++;
                }
            }

            HashSet<Tuple<int, int>> next = new HashSet<Tuple<int, int>>();
            // apply life rules
            foreach(var tuple in black)
            {
                int nCount;
                if (neighbors.TryGetValue(tuple, out nCount))
                {
                    if (nCount > 0 && nCount <= 2)
                    {
                        next.Add(tuple);
                    }
                    neighbors.Remove(tuple);
                }
            }

            foreach(var tuple in neighbors.Keys)
            {
                int nCount = neighbors[tuple];
                if (nCount == 2)
                {
                    next.Add(tuple);
                }
            }

            return next;
        }

        private List<List<Dir>> AllDirs()
        {
            return new List<List<Dir>>()
            {
                new List<Dir>() { Dir.E },
                new List<Dir>() { Dir.W },
                new List<Dir>() { Dir.SE },
                new List<Dir>() { Dir.SW },
                new List<Dir>() { Dir.NE },
                new List<Dir>() { Dir.NW },
            };
        }

        private Tuple<int, int> ToCoord(IEnumerable<Dir> dirs)
        {
            int x = 0, y = 0;
            foreach(Dir d in dirs)
            {
                switch(d)
                {
                    case Dir.E:
                        x = x + 2;
                        break;
                    case Dir.W:
                        x = x - 2;
                        break;
                    case Dir.SE:
                        y = y - 1;
                        x = x + 1;
                        break;
                    case Dir.SW:
                        y = y - 1;
                        x = x - 1;
                        break;
                    case Dir.NE:
                        y = y + 1;
                        x = x + 1;
                        break;
                    case Dir.NW:
                        y = y + 1;
                        x = x - 1;
                        break;
                    default:
                        throw new Exception("Invalid dir");
                }
            }
            return new Tuple<int, int>(x, y);
        }

        private IEnumerable<Dir> ParseLine(string line)
        {
            for (int i=0; i < line.Length; i++)
            {
                switch(line[i])
                {
                    case 'n':
                        switch(line[++i])
                        {
                            case 'e':
                                yield return Dir.NE;
                                break;
                            case 'w':
                                yield return Dir.NW;
                                break;
                            default:
                                throw new Exception("Invalid");
                        }
                        break;
                    case 's':
                        switch (line[++i])
                        {
                            case 'e':
                                yield return Dir.SE;
                                break;
                            case 'w':
                                yield return Dir.SW;
                                break;
                            default:
                                throw new Exception("Invalid");
                        }
                        break;
                    case 'e':
                        yield return Dir.E;
                        break;
                    case 'w':
                        yield return Dir.W;
                        break;
                    default:
                        throw new Exception("invalid");
                }
            }
        }
    }
}
