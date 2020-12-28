using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day12 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return DistanceFromStart(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return DistanceWithWaypoint(input);
        }

        private int DistanceWithWaypoint(IEnumerable<string> input)
        {
            int waypointN = 1;
            int waypointE = 10;

            int boatN = 0;
            int boatE = 0;

            foreach (string s in input)
            {
                int val = Int32.Parse(s.Substring(1));
                string key = s.Substring(0, 1);
                switch (key)
                {
                    case "N":
                        waypointN += val;
                        break;
                    case "S":
                        waypointN -= val;
                        break;
                    case "E":
                        waypointE += val;
                        break;
                    case "W":
                        waypointE -= val;
                        break;
                    case "F":
                        boatN += val * waypointN;
                        boatE += val * waypointE;
                        break;
                    case "L":
                        {
                            int delta = (val / 90);
                            for(int i=0; i < delta; i++)
                            {
                                int tmpE = -waypointN;
                                waypointN = waypointE;
                                waypointE = tmpE;
                            }
                            break;
                        }
                    case "R":
                        {
                            int delta = (val / 90);
                            for (int i = 0; i < delta; i++)
                            {
                                int tmpE = waypointN;
                                waypointN = -waypointE;
                                waypointE = tmpE;
                            }
                            break;
                        }
                    default:
                        Console.Write("Unknown command: " + s);
                        break;
                }
            }

            return Math.Abs(boatN) + Math.Abs(boatE);
        }

        public int DistanceFromStart(IEnumerable<string> input)
        {
            int north = 0;
            int east = 0;

            string[] headings = new string[] { "N", "E", "S", "W" };
            int heading = 1;

            void MoveHeading(string heading, int distance)
            {
                switch (heading)
                {

                    case "N":
                        north += distance;
                        break;
                    case "S":
                        north -= distance;
                        break;
                    case "E":
                        east += distance;
                        break;
                    case "W":
                        east -= distance;
                        break;
                    default:
                        Console.WriteLine("Invalid heading: " + heading);
                        break;
                }
            }

            foreach(string s in input)
            {
                int val = Int32.Parse(s.Substring(1));
                string key = s.Substring(0, 1);
                switch(key)
                {
                    case "N":
                    case "S":
                    case "E":
                    case "W":
                        MoveHeading(key, val);
                        break;
                    case "F":
                        MoveHeading(headings[heading], val);
                        break;
                    case "L":
                        {
                            int delta = -1 * (val / 90);
                            heading = (heading + 4 + delta) % headings.Length;
                            break;
                        }
                    case "R":
                        {
                            int delta = val / 90;
                            heading = (heading + delta) % headings.Length;
                            break;
                        }
                    default:
                        Console.Write("Unknown command: " + s);
                        break;
                }
            }

            return Math.Abs(north) + Math.Abs(east);
        }
    }
}
