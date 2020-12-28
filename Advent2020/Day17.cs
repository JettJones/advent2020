using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day17 : DayInterface
    {
        private class Coord
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Z;
            public readonly int W;

            public Coord(int x, int y, int z, int w=0)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.W = w;
            }

            public Coord(Coord b, Coord offset)
            {
                this.X = b.X + offset.X;
                this.Y = b.Y + offset.Y;
                this.Z = b.Z + offset.Z;
                this.W = b.W + offset.W;
            }

            public override string ToString()
            {
                return String.Format("[{0}, {1}, {2} {3}]", X, Y, Z, W);
            }
            public override int GetHashCode()
            {
                return (X, Y, Z, W).GetHashCode();
            }
            public override bool Equals(object obj)
            {
                Coord ascast = obj as Coord;
                if (ascast != null)
                {
                    return X == ascast.X
                        && Y == ascast.Y
                        && Z == ascast.Z
                        && W == ascast.W;
                }
                return base.Equals(obj);
            }
        }

        private struct State
        {
            public List<Coord> Active;
        }

        public object SolveA(IEnumerable<string> input)
        {
            return CountAfter(input, 6);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return Count4After(input, 6);
        }

        private int CountAfter(IEnumerable<string> input, int steps = 6)
        {
            State next = MyParse(input);

            var offs = Offsets().ToList();

            for (int i = 0; i < steps; i++) 
            {
                next = Tick(next, offs);
            }

            return next.Active.Count();
        }

        private int Count4After(IEnumerable<string> input, int steps = 6)
        {
            State next = MyParse(input);

            var offs = Offset4s().ToList();

            for (int i = 0; i < steps; i++)
            {
                next = Tick(next, offs);
            }

            return next.Active.Count();
        }

        private State MyParse(IEnumerable<string> input)
        {
            int y = 0;
            List<Coord> result = new List<Coord>();
            foreach (string line in input)
            {
                for(int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        result.Add(new Coord(x, y, 0));
                    }
                }
                y--;
            }

            return new State() { Active = result };
        }

        private State Tick(State start, List<Coord> offs)
        {
            Dictionary<Coord, int> neighbor = new Dictionary<Coord, int>();
            foreach(Coord c in start.Active)
            {
                foreach (Coord o in offs)
                {
                    Coord n = new Coord(c, o);
                    if (!neighbor.ContainsKey(n))
                    {
                        neighbor.Add(n, 0);
                    }

                    neighbor[n]++;
                }
            }

            List<Coord> result = new List<Coord>();

            // for active
            foreach (Coord c in start.Active)
            {
                int value;
                if (neighbor.TryGetValue(c, out value) && (value == 2 || value == 3))
                {
                    result.Add(c);
                    neighbor.Remove(c);
                }
            }

            // for non-active
            foreach(Coord c in neighbor.Keys)
            {
                int value = neighbor[c];
                if (value == 3)
                {
                    result.Add(c);
                }
            }

            return new State() { Active = result };
        }

        private IEnumerable<Coord> Offsets()
        {
            for (int z = -1; z <= 1; z++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0 && z == 0) { continue; }

                        yield return new Coord(x, y, z);
                    }
                }
            }
        }

        private IEnumerable<Coord> Offset4s()
        {
            for (int w = -1; w <= 1; w++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            if (x == 0 && y == 0 && z == 0 && w == 0) { continue; }

                            yield return new Coord(x, y, z, w);
                        }
                    }
                }
            }
        }
    }
}
