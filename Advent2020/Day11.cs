using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{

    public class Layout
    {
        private List<StringBuilder> rows;

        public readonly int RowCount;
        public readonly int ColCount;

        public Layout(IEnumerable<StringBuilder> input)
        {
            this.rows = input.Select(sb => new StringBuilder(sb.ToString())).ToList();
            this.RowCount = this.rows.Count();
            this.ColCount = this.rows[0].Length;
        }

        public Layout(IEnumerable<string> input)
            : this(input.Select(s => new StringBuilder(s)))
        {
        }

        public Layout Clone()
        {
            return new Layout(this.rows);
        }

        public List<char> Neighbors(int r, int c)
        {
            List<char> result = new List<char>();

            for (int ir = - 1; ir <= 1; ir++)
            {
                for (int ic = - 1; ic <= 1; ic++)
                {
                    if (ic == 0 && ir == 0) { continue; }

                    MaybeAdd(result, r + ir, c + ic);
                }
            }

            return result;
        }
        public List<char> NeighborsComplex(int r, int c)
        {
            List<char> result = new List<char>();

            for (int ir = -1; ir <= 1; ir++)
            {
                for (int ic = -1; ic <= 1; ic++)
                {
                    if (ic == 0 && ir == 0) { continue; }

                    MaybeAddComplex(result, r+ir, c+ic, ir, ic);
                }
            }

            return result;
        }

        public char Get(int r, int c)
        {
            return this.rows[r][c];
        }

        public void Set(int r, int c, char seat)
        {
            this.rows[r][c] = seat;
        }

        private void MaybeAdd(List<char> result, int r, int c)
        { 
            if (r >= 0 && r < this.RowCount 
                && c >=0 && c < this.ColCount)
            {
                result.Add(this.rows[r][c]);
            }
        }

        private void MaybeAddComplex(List<char> result, int r, int c, int deltaR, int deltaC)
        {
            if (r >= 0 && r < this.RowCount
                && c >= 0 && c < this.ColCount)
            {
                if (this.rows[r][c] != '.')
                {
                    result.Add(this.rows[r][c]);
                }
                else
                {
                    MaybeAddComplex(result, r + deltaR, c + deltaC, deltaR, deltaC);
                }
            }
        }

        public string PrintMe()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin("\n", this.rows.Select(sb => sb.ToString()));
            sb.Append("\n");
            return sb.ToString();
        }

    }
    class Day11 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return SeatSimple(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return SeatComplex(input);
        }

        // low: 2188
        private int SeatSimple(IEnumerable<string> input)
        {
            Layout next = new Layout(input);
            Layout prev;

            do
            {
                prev = next;
                next = OneGeneration(prev);
                Console.WriteLine(next.PrintMe());
            } while (next != prev);

            return CountOccupied(next);
        }

        private int SeatComplex(IEnumerable<string> input)
        {
            Layout next = new Layout(input);
            Layout prev;

            do
            {
                prev = next;
                next = OneGenerationComplex(prev);
                Console.WriteLine(next.PrintMe());
            } while (next != prev);

            return CountOccupied(next);
        }

        private int CountOccupied(Layout start)
        {
            int count = 0;
            for (int ir = 0; ir < start.RowCount; ir++)
            {
                for (int ic = 0; ic < start.ColCount; ic++)
                {
                    char value = start.Get(ir, ic);

                    if (value == '#') { count++; }
                }
            }

            return count;
        }

        private Layout OneGeneration(Layout start)
        {
            Layout next = start.Clone();
            // for easy equality, return the starting value if nothing is different.
            int changeCount = 0;

            for (int ir = 0; ir < start.RowCount; ir++)
            {
                for (int ic = 0; ic < start.ColCount; ic++)
                {
                    char value = start.Get(ir, ic);

                    if (value == '.') { continue; }


                    var n = start.Neighbors(ir, ic);
                    int occupied = n.Where(c => c == '#').Count();
                    if (value == 'L' && occupied == 0)
                    {
                        next.Set(ir, ic, '#');
                        changeCount++;
                    }
                    else if (value == '#' && occupied >= 4)
                    {
                        next.Set(ir, ic, 'L');
                        changeCount++;
                    }
                }
            }

            return changeCount > 0 ? next : start;
        }


        private Layout OneGenerationComplex(Layout start)
        {
            Layout next = start.Clone();
            // for easy equality, return the starting value if nothing is different.
            int changeCount = 0;

            for (int ir = 0; ir < start.RowCount; ir++)
            {
                for (int ic = 0; ic < start.ColCount; ic++)
                {
                    char value = start.Get(ir, ic);

                    if (value == '.') { continue; }


                    var n = start.NeighborsComplex(ir, ic);
                    int occupied = n.Where(c => c == '#').Count();
                    if (value == 'L' && occupied == 0)
                    {
                        next.Set(ir, ic, '#');
                        changeCount++;
                    }
                    else if (value == '#' && occupied >= 5)
                    {
                        next.Set(ir, ic, 'L');
                        changeCount++;
                    }
                }
            }

            return changeCount > 0 ? next : start;
        }
    }
}
