using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day14 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return SimMemory(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return SimDecoder(input);
        }

        private long SimMemory(IEnumerable<string> input)
        {
            // map of address -> value
            Dictionary<long, long> set = new Dictionary<long, long>();

            long mapHigh = 0L;
            long mapLow = long.MaxValue;

            foreach(string s in input)
            {
                string[] inst = s.Split(" = ");

                if (inst[0] == "mask")
                {
                    string strHigh = inst[1].Replace("X", "0");
                    string strLow = inst[1].Replace("X", "1");

                    mapHigh = 0L | Convert.ToInt64(strHigh, 2);
                    mapLow = long.MaxValue & Convert.ToInt64(strLow, 2);
                }
                else
                {
                    string digit = inst[0].Substring(4, inst[0].Length - 5);
                    long addr = long.Parse(digit);
                    long value = long.Parse(inst[1]);

                    long masked = (value & mapLow) | mapHigh;

                    set[addr] = masked;
                }

            }

            return set.Values.Sum();
        }

        private long SimDecoder(IEnumerable<string> input)
        {
            // map of address -> value
            Dictionary<long, long> set = new Dictionary<long, long>();

            string mask = "";

            foreach (string s in input)
            {
                string[] inst = s.Split(" = ");

                if (inst[0] == "mask")
                {
                    mask = inst[1];
                }
                else
                {
                    string digit = inst[0].Substring(4, inst[0].Length - 5);
                    long addr = long.Parse(digit);
                    long value = long.Parse(inst[1]);

                    IEnumerable<long> alist = AllAddr(mask, addr);

                    foreach(long l in alist)
                    {
                        set[l] = value;
                    }
                }

            }

            return set.Values.Sum();
        }

        private IEnumerable<long> AllAddr(string mask, long addr)
        {
            // count up, 0-36, for each bit of the mask, take a corresponding value of the address, and apply to the result.
            // for X, duplicate the list with both values.
            HashSet<long> result = new HashSet<long>();
            result.Add(0L);

            long bitmask = 1L;
            for (int i = 0; i < 36; i++)
            {
                char key = mask[35 - i];
                HashSet<long> next = new HashSet<long>();
                switch(key)
                {
                    case '0':
                        if ((addr & bitmask) > 0)
                        {
                            // set 1 as the next bit in all values
                            foreach (long l in result)
                            {
                                next.Add(l | bitmask);
                            }
                        } else
                        {
                            // set 0 as the next bit in all values (no change)
                            next = result;
                        }
                        break;
                    case '1':
                        foreach(long l in result)
                        { 
                            next.Add(l | bitmask);
                        }
                        break;
                    case 'X':
                        foreach (long l in result)
                        {
                            next.Add(l);
                            next.Add(l | bitmask);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid character: " + key);
                        break;
                }

                result = next;
                bitmask = bitmask << 1;
            }

            return result;
        }
    }
}
