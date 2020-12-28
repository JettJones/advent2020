using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Advent2020
{

    public interface DayInterface
    {
        public object SolveA(IEnumerable<string> input);
        public object SolveB(IEnumerable<string> input);
    }

    class Program
    {
        static void Main(string[] args)
        {
            string arg = args.FirstOrDefault();
            Match match = Regex.Match(arg, "^([0-9]+)(.*)");
            if (match.Success)
            {
                string dayNum = match.Groups[1].Value;
                string flag = match.Groups[2].Value;

                Console.WriteLine("Running Day{0} {1}", dayNum, flag);

                var modules = typeof(Program).Assembly.GetModules(getResourceModules: false);
                var myModule = modules.First();
                string className = String.Format("Day{0}", dayNum);
                Type t = myModule.FindTypes(Module.FilterTypeName, className).First();

                DayInterface day = (DayInterface)t.GetConstructor(new Type[0]).Invoke(new object[0]);

                string inputName = String.Format("day{0}.input", dayNum);
                string[] input = File.ReadAllLines(inputName);

                object result = flag.StartsWith("b") ? day.SolveB(input) : day.SolveA(input);
                Console.WriteLine("Solution for {0}: {1}", arg, result);
            }

        }
    }
}
