using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day18 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return SumMaths(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return SumMaths2(input);
        }

        // low: 7471564075
        private long SumMaths(IEnumerable<string> input)
        {
            long total = 0;
            var order = DefaultOrder;
            foreach (string line in input)
            {
                long lineSum = Evaluate(line.Split(" "), order);
                total += lineSum;
            }

            return total;
        }


        // low: 7471564075
        private long SumMaths2(IEnumerable<string> input)
        {
            long total = 0;
            var order = new Dictionary<string, int>
            {
                ["+"] = 1,
                ["*"] = 2
            };
            foreach (string line in input)
            {
                long lineSum = Evaluate(line.Split(" "), order);
                total += lineSum;
            }

            return total;
        }

        private static Dictionary<string, int> DefaultOrder = new Dictionary<string, int>() { ["+"] = 1, ["*"] = 1 };

        private long Evaluate(IEnumerable<string> line, Dictionary<string, int> opOrder)
        {
            Stack<long> expValue = new Stack<long>();
            Stack<string> ops = new Stack<string>();

            foreach(string s in line)
            {
                string trimmed = s;

                while (trimmed.StartsWith("("))
                {
                    ops.Push("(");
                    trimmed = trimmed.Substring(1);
                }

                trimmed = trimmed.TrimEnd(')');
                long val;
                if (Int64.TryParse(trimmed, out val))
                {
                    expValue.Push(val);
                }
                else
                {
                    if (ops.Count == 0)
                    {
                        // no previous operation.
                    }
                    else 
                    {
                        string peek = ops.Peek();
                        if (peek == "(")
                        {
                            // New parens, no-op
                        }
                        else if (opOrder[peek] <= opOrder[trimmed])
                        {
                            // previous operator activates before the current one
                            long calc = Calculate(ops.Pop(), expValue.Pop(), expValue.Pop());
                            expValue.Push(calc);
                        }
                    }

                    ops.Push(trimmed);
                }

                trimmed = s;
                while(trimmed.EndsWith(")"))
                {
                    string pop;
                    while(ops.TryPop(out pop) && pop != "(")
                    {
                        long calc = Calculate(pop, expValue.Pop(), expValue.Pop());
                        expValue.Push(calc);
                    }

                    trimmed = trimmed.Substring(0, trimmed.Length - 1);
                }
            }

            while (ops.Count > 0)
            {
                long calc = Calculate(ops.Pop(), expValue.Pop(), expValue.Pop());
                expValue.Push(calc);
            }

            if (expValue.Count != 1)
            {
                throw new Exception("Unexpected leftover integers");
            }

            return expValue.Pop();
        }

        private long Calculate(string op, long left, long right)
        {
            switch(op)
            {
                case "+":
                    return left + right;
                case "*":
                    return left * right;
                default:
                    throw new Exception("Unknown Operator");
            }
        }
    }
}
