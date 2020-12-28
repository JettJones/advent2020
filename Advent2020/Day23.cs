using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day23 : DayInterface
    {
        public class CircleNode<T>
        {
            public T Value;
            public CircleNode<T> Next;

            public IEnumerable<CircleNode<T>> Nodes()
            {
                CircleNode<T> first = this;
                var cur = this;
                do
                {
                    yield return cur;
                    cur = cur.Next;
                } while (cur != first);
            }
        }
        public class CircleLink<T>
        {
            public readonly CircleNode<T> First;

            public CircleLink(IEnumerable<T> values)
            {
                this.First = new CircleNode<T>();
                CircleNode<T> active = null;
                CircleNode<T> next = First;
                foreach (T v in values)
                {
                    active = next;
                    active.Value = v;

                    next = new CircleNode<T>();
                    active.Next = next;
                }
                active.Next = First;
            }

            public IEnumerable<CircleNode<T>> Nodes()
            {
                return this.First.Nodes();
            }

            public IEnumerable<T> Values()
            {
                var cur = this.First;
                do
                {
                    yield return cur.Value;
                    cur = cur.Next;
                } while (cur != this.First);
            }
        }

        public object SolveA(IEnumerable<string> input)
        {
            return MoveCups(input, moves: 100);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return MillionCups(input, moves: 10 * 1000 * 1000);
        }

        public long MillionCups(IEnumerable<String> input, int moves=10)
        {
            List<int> order = input.First().Select(s => Int32.Parse("" + s) - 1).ToList();
            order.AddRange(Enumerable.Range(9, 1000000 - 9));

            order = LinkDance(moves, order);

            int oneIndex = order.IndexOf(0);
            int next = order[(oneIndex + 1) % order.Count] +1;
            int nextnext = order[(oneIndex + 2) % order.Count] +1;

            return 1L * next * nextnext;
        }

        // too high: 471253986  - oops, need to rotate to 1
        public string MoveCups(IEnumerable<string> input, int moves=100)
        {
            List<int> order = input.First().Select(s => Int32.Parse("" + s) - 1).ToList();

            order = CrabDance(moves, order);

            // Find 1, show the string after that.
            int oneIndex = order.IndexOf(0);
            string result = string.Join("", order.Select(i => i + 1));

            return result.Substring(oneIndex + 1) + result.Substring(0, oneIndex);

        }
        private List<int> LinkDance(int moves, List<int> order)
        {
            // Because CrabDance is slow from List.Find, a reimplementation with linked list and value index.
            int cupCount = order.Count;
            CircleLink<int> cups = new CircleLink<int>(order);
            CircleNode<int>[] tmp = new CircleNode<int>[3];

            var index = MakeIndex(cups, cupCount);

            var active = cups.First;
            HashSet<int> values = new HashSet<int>(capacity: 4);
            for (int i = 0; i < moves; i++)
            {
                if ((i % 1000) == 0)
                {
                    Console.WriteLine("{0} : {1}", i, CupString(cups.Values().Take(10)));
                }

                tmp[0] = active.Next;
                tmp[1] = tmp[0].Next;
                tmp[2] = tmp[1].Next;

                // remove cups
                active.Next = tmp[2].Next;

                // choose next value
                values.Clear();
                values.Add(active.Value);
                values.Add(tmp[0].Value);
                values.Add(tmp[1].Value);
                values.Add(tmp[2].Value);
                int nextValue = active.Value;
                while (values.Contains(nextValue))
                {
                    nextValue = (nextValue + cupCount - 1) % cupCount;
                }

                // add cups back
                var place = index[nextValue];
                tmp[2].Next = place.Next;
                place.Next = tmp[0];

                // move our active cup
                active = active.Next;
            }

            return cups.Values().ToList();
        }

        private List<CircleNode<int>> MakeIndex(CircleLink<int> cups, int cupCount)
        {
            var result = new List<CircleNode<int>>(
                Enumerable.Range(0, cupCount)
                .Select<int, CircleNode<int>>(i => null));
            foreach (var node in cups.Nodes())
            {
                result[node.Value] = node;
            }

            return result;
        }

        private List<int> CrabDance(int moves, List<int> order)
        {
            int cupCount = order.Count;

            int curIndex = 0;
            int[] cups = new int[3];
            HashSet<int> values = new HashSet<int>();

            for (int i = 0; i < moves; i++)
            {
                if ((i % 1000) == 0)
                {
                    Console.WriteLine("{0} : {1}", i, CupString(order.Take(10)));
                }

                int curValue = order[curIndex];

                cups[0] = order[(curIndex + 1) % cupCount];
                cups[1] = order[(curIndex + 2) % cupCount];
                cups[2] = order[(curIndex + 3) % cupCount];

                // remove 3 cups
                if (curIndex + 3 >= cupCount)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int nx = (curIndex + 1) >= order.Count ? 0 : curIndex + 1;
                        order.RemoveAt(nx);
                    }

                    curIndex -= (curIndex + 4) % cupCount;
                }
                else
                {
                    order.RemoveRange(curIndex + 1, 3);
                }

                // choose the next value (cur - 1; ignoring missing cups)
                int nextValue = curValue;
                values.Clear();
                values.UnionWith(cups);
                values.Add(curValue);
                while (values.Contains(nextValue))
                {
                    nextValue = (nextValue + cupCount - 1) % cupCount;
                }

                // add cups back
                int nextIndex = order.IndexOf(nextValue);
                order.InsertRange(nextIndex + 1, cups);

                // our active cup moved, find it again
                if (nextIndex < curIndex) { curIndex += 3; }
                curIndex = (curIndex + 1) % cupCount;
            }

            return order;
        }

        private string CupString(IEnumerable<int> cups)
        {
            return string.Join(" ", cups.Select(i => i+1));
        }
    }

}
