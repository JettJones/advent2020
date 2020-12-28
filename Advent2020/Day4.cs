using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Advent2020
{
    class Day4 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return this.ValidCount(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return this.ValidCount2(input);
        }

        public int ValidCount(IEnumerable<string> input)
        {
            IEnumerable<Dictionary<string, string>> passports = this.Parse(input);

            var allp = passports.ToList();

            Console.WriteLine("All Passport count: " + allp.Count());
            var valid = allp.Where(ValidKeys);

            return valid.Count();
        }

        public int ValidCount2(IEnumerable<string> input)
        {
            IEnumerable<Dictionary<string, string>> passports = this.Parse(input);

            var allp = passports.Where(ValidKeys);

            Console.WriteLine("Valid Passport count: " + allp.Count());
            var valid = allp.Where(StrictValid).ToList();

            foreach (var p in valid)
            {
                PrintPassport(p);
            }

            return valid.Count();
        }

        private bool ValidKeys(Dictionary<string, string> passport)
        {
            string[] required = new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid", /* "cid" */ };

            foreach (string r in required)
            {
                if (!passport.ContainsKey(r))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IntBetween(string key, string val, int min, int max, int? digits=null)
        {
            if (digits != null && val.Length != digits.Value)
            {
                Console.WriteLine("Invalid {0} {1} should have {2} digits", key, val, digits.Value);
                return false;
            }

            int ival = Int32.Parse(val);

            if (ival < min)
            {
                Console.WriteLine("Invalid {0} {1} is lower than {2}", key, val, min);
                return false;
            }

            if (ival > max)
            {
                Console.WriteLine("Invalid {0} {1} is higher than {2}", key, val, max);
                return false;
            }

            return true;
        }

        private bool ValidHeight(string val)
        {
            string intpart = val.Substring(0, val.Length - 2);
            string suffix = val.Substring(val.Length - 2, 2);
            switch(suffix)
            {
                case "in":
                    return IntBetween("hgt", intpart, 59, 76);
                case "cm":
                    return IntBetween("hgt", intpart, 150, 193);
                default:
                    Console.WriteLine("height did not end in cm or in: {0} for {1}", suffix, val);
                    return false;
            }
        }

        private bool ValidEye(string val)
        {
            HashSet<string> valid = new HashSet<string>() {"amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            bool found =  valid.Contains(val);
            if (!found)
            {
                Console.WriteLine("Invalid eye: {0}", val);
            }
            return found;
        }


        private bool ValidHair(string val)
        {
            bool isMatch = Regex.IsMatch(val, "^#[0-9a-f]{6}$");
            if (!isMatch)
            {
                Console.WriteLine("Invalid hair: {0}", val);
            }

            return isMatch;
        }

        private bool ValidPid(string val)
        {
            bool isMatch = Regex.IsMatch(val, "^[0-9]{9}$");
            if (!isMatch)
            {
                Console.WriteLine("Invalid pid: {0}", val);
            }

            return isMatch;
        }

        private bool StrictValid(Dictionary<string, string> passport)
        {
            if (!IntBetween("byr", passport["byr"], 1920, 2002, 4)) { return false; }
            if (!IntBetween("iyr", passport["iyr"], 2010, 2020, 4)) { return false; }
            if (!IntBetween("eyr", passport["eyr"], 2020, 2030, 4)) { return false; }
            if (!ValidHeight(passport["hgt"])) { return false; }
            if (!ValidHair(passport["hcl"])) { return false; }
            if (!ValidEye(passport["ecl"])) { return false; }
            if (!ValidPid(passport["pid"])) { return false; }

            return true;

        }

        private void PrintPassport(Dictionary<string, string> passport)
        { 
            Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}", passport["pid"], passport["byr"], passport["iyr"], passport["eyr"],  passport["hcl"], passport["ecl"], passport["hgt"]);
        }

        private IEnumerable<Dictionary<string, string>> Parse(IEnumerable<string> input)
        {
            Dictionary<string, string> current = new Dictionary<string, string>();

            foreach (string l in input)
            {
                if (String.IsNullOrWhiteSpace(l))
                {
                    yield return current;
                    current = new Dictionary<string, string>();
                    continue;
                }

                string[] pairs = l.Split(" ");
                foreach (string p in pairs)
                {
                    string[] keyval = p.Split(':', 2);

                    if (keyval.Length != 2)
                    {
                        Console.WriteLine("Invalid pair: " + p);
                    }

                    if (current.ContainsKey(keyval[0]))
                    {
                        Console.WriteLine("Duplicate key: " + p);
                    }
                    current.Add(keyval[0], keyval[1]);
                }
            }

            yield return current;
        }
    }
}
