using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day8 : DayInterface
    {

        struct Inst
        {
            public string Code;
            public int Arg;
        }

        struct Result
        {
            public int ExitCode;
            public int Acc;
        }

        public object SolveA(IEnumerable<string> input)
        {
            List<Inst> parsed = input.Select(MyParse).ToList();
            var result = Simulate(parsed);
            return result.Acc;
        }

        public object SolveB(IEnumerable<string> input)
        {
            List<Inst> parsed = input.Select(MyParse).ToList();

            var result = ReplaceEach(parsed, "nop", "jmp");
            if (result.ExitCode == 0)
            {
                return result.Acc;
            }

            result = ReplaceEach(parsed, "jmp", "nop");
            if (result.ExitCode == 0)
            {
                return result.Acc;
            }

            Console.WriteLine("No Solution");
            return "";
        }

        private Result ReplaceEach(List<Inst> input, string from, string to)
        {
            for(int i=0; i < input.Count; i++)
            {
                var curr = input[i];
                if (curr.Code == from)
                {
                    input[i] = new Inst() { Code = to, Arg = curr.Arg };
                    var result = Simulate(input);
                    if (result.ExitCode == 0)
                    {
                        Console.WriteLine("Successfully changed {0} to {1} at {2}", from, to, i);
                        return result;
                    }

                    input[i] = curr;
                }
            }

            // tried all, no dice
            return new Result() { ExitCode = 1, Acc = 0 };
        }

        private Result Simulate(List<Inst> input)
        {
            // detect loops: the second execution of an instruction
            int pc = 0;
            int acc = 0;
            var seen = new HashSet<int>();

            while(pc < input.Count())
            {
                if (seen.Contains(pc))
                {
                    Console.WriteLine("Duplicate instruction {0}. Current acc: {1}", pc, acc);
                    return new Result(){ ExitCode = 1, Acc = acc};
                }

                seen.Add(pc);

                Inst curr = input[pc];
                switch(curr.Code)
                {
                    case "nop":
                        pc++;
                        break;
                    case "acc":
                        acc += curr.Arg;
                        pc++;
                        break;
                    case "jmp":
                        pc += curr.Arg;
                        break;
                    default:
                        Console.WriteLine("Unknown inst: {0} at {1}", curr.Code, pc);
                        return new Result() { ExitCode = 3, Acc = acc };
                }
            }

            Console.WriteLine("Normal exit.  pc: {0}, acc: {1}", pc, acc);
            return new Result() { ExitCode = 0, Acc = acc };
        }

        private Inst MyParse(string line)
        {
            return new Inst()
            {
                Code = line.Substring(0, 3),
                Arg = Int32.Parse(line.Substring(3))
            };
        }
    }
}
