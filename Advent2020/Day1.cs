using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent2020
{
    class Day1 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return this.FindProduct(MyParse(input));
        }

        public object SolveB(IEnumerable<string> input)
        {
            return this.FindProduct3(MyParse(input));
        }

        private List<int> MyParse(IEnumerable<string> input)
        {
            return input.Select(x => Int32.Parse(x)).ToList();
        }
        public int FindProduct(List<int> input)
        {
            Console.WriteLine("Starting input: " + input.Count);

            HashSet<int> seen = new HashSet<int>();
            // foreach value in the list
            // check if 2020 - {v} already exists
            // if yes: done - return product.
            // if no: add {v} to the seen values

            foreach (int x in input)
            {
                if (seen.Contains(2020 - x))
                {
                    return x * (2020 - x);
                }

                seen.Add(x);
            }

            Console.WriteLine("Checked all values, no match");
            throw new Exception("Unsolvable");
        }

        public int FindProduct3(List<int> input)
        {
            // Find a product for three numbers
            // one option: calculate n^2 sums, and check the hash set for the third value.
            // optimization: skip the sum if it's greater than 2020.
            // edge case: don't use a number twice

            SortedSet<int> seen = new SortedSet<int>();
            int smallest = input.FirstOrDefault();
            seen.Add(input[0]);
            seen.Add(input[1]);
            foreach (int x in input.Skip(2))
            {
                if (x < smallest) smallest = x;
                foreach (int y in seen)
                {
                    // optimization
                    if (x + y + smallest > 2020)
                    {
                        Console.WriteLine("Optimization");
                        break;
                    }

                    int diff = (2020 - x - y);
                    if (diff == y)
                    {
                        // edge case
                        Console.WriteLine("Edge case");
                        continue;
                    }
                    if (seen.Contains(diff))
                    {
                        return x * y * diff;
                    }
                }

                seen.Add(x);
            }

            Console.WriteLine("Checked all values, no match");
            throw new Exception("Unsolvable");
        }
    }
}
