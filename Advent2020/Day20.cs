using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Advent2020
{
    class Day20 : DayInterface
    {

        public class Tile
        {
            public readonly int Id;
            public readonly List<string> Content;
            public readonly List<Side> Sides;

            public List<SideLink> Matches;
            public List<SideLink> FlipMatches;

            // Store matches here

            public Tile(int id, IEnumerable<string> content)
            {
                this.Id = id;
                this.Content = content.ToList();
                this.Sides = MakeSides(this.Content);
                this.Matches = new List<SideLink>();
                this.FlipMatches = new List<SideLink>();
            }

            private List<Side> MakeSides(List<string> content)
            {
                List<Side> result = new List<Side>(4);

                result.Add(new Side(content.First()));                             // N
                result.Add(new Side(content.Select(s => s.Last())));               // E
                result.Add(new Side(content.Last().Reverse()));                    // S
                result.Add(new Side(content.Select(s => s.First()).Reverse()));    // W

                return result;
            }

            internal void AddMatch(int mySide, int otherId, int otherSide, SideMatch smatch)
            {
                if (smatch == SideMatch.None)
                {
                    throw new Exception("Invalid match type");
                }

                var link = new SideLink() { FirstId = this.Id, SecondId = otherId, SideFirst = mySide, SideSecond = otherSide };
                if (smatch == SideMatch.FlipMatch)
                {
                    this.FlipMatches.Add(link);
                }
                else
                {
                    this.Matches.Add(link);
                }
            }

            internal Tile Rotate(int toSide, int mySide, bool flip = false)
            {
                int rotate = (4 + toSide - mySide) % 4;

                // rotate & flip content
                List<string> newContent = this.Content.RotateContent(rotate);
                if (flip)
                {
                    var flipAxis = (toSide % 2);
                    newContent = newContent.FlipContent(flipAxis);
                }

                // rotate matches
                List<SideLink> newMatch = new List<SideLink>();
                List<SideLink> newFlip = new List<SideLink>();

                foreach (SideLink sl in this.Matches)
                {
                    var target = flip ? newFlip : newMatch;
                    target.Add(sl.Rotate(toSide, rotate, flip));
                }

                foreach (SideLink sl in this.FlipMatches)
                {
                    var target = flip ? newMatch : newFlip;
                    target.Add(sl.Rotate(toSide, rotate, flip));
                }

                var newTile = new Tile(this.Id, newContent);
                newTile.Matches = newMatch;
                newTile.FlipMatches = newFlip;
                return newTile;
            }

            internal IEnumerable<string> Pixels()
            {
                // return the content without the borders.
                return this.Content.Skip(1).SkipLast(1).Select(s => s.Substring(1, s.Length - 2));
            }
        }

        public struct SideLink
        {
            public int FirstId;
            public int SecondId;
            public int SideFirst;
            public int SideSecond;

            internal SideLink Rotate(int toSide, int rotate, bool flip)
            {
                // Rotate - number of compass points clockwise.
                var newSide = ((this.SideFirst + rotate + 4) % 4);

                // FlipConst - if the side is flipped, non polar sides reverse.
                // eg. when flipping a piece attached to North, East and West reverse.
                var flipConst = (flip && (newSide % 2) != (toSide % 2)) ? 2 : 0;
                newSide = (newSide + flipConst) % 4;

                return new SideLink()
                {
                    FirstId = this.FirstId,
                    SecondId = this.SecondId,
                    SideSecond = this.SideSecond,
                    SideFirst = newSide
                };
            }
        }

        public enum SideMatch
        {
            None,
            Match,
            FlipMatch,
        }

        public static class SideEnum
        {
            public const int N = 0;
            public const int E = 1;
            public const int S = 2;
            public const int W = 3;
        }

        public struct Side
        {
            public int SideBits;
            public int Reverse;


            public Side(IEnumerable<char> input)
            {
                int flag = 1;
                SideBits = 0;
                Reverse = 0;
                foreach (char c in input)
                {
                    int bit = (c == '#') ? int.MaxValue : 0;

                    Reverse = Reverse << 1;
                    SideBits = SideBits | (bit & flag);
                    Reverse = Reverse | (bit & 0x1);
                    flag = flag << 1;
                }
            }

            public SideMatch Match(Side other)
            {
                if (this.SideBits == other.Reverse)
                    return SideMatch.Match;
                if (this.SideBits == other.SideBits)
                    return SideMatch.FlipMatch;

                return SideMatch.None;
            }

            public override string ToString()
            {
                return Convert.ToString(SideBits, toBase: 2);
            }

        }
        public object SolveA(IEnumerable<string> input)
        {
            return ImageCorners(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return FindMonsters(input);
        }

        public long ImageCorners(IEnumerable<string> input)
        {
            var tiles = MyParse(input).ToList();
            // 144 tiles, with 8 edges each vs 2^10 possible edges

            // all matches for 1 tile will be either direct or flipped. 
            // say we index N E S W.  4,3,2 matches are valid, but 2 must be adjacent.

            FindMatches(tiles);

            var corners = tiles.Where(t => t.Matches.Count + t.FlipMatches.Count <= 2).Select(t => t.Id).ToList();
            return 1L * corners[0] * corners[1] * corners[2] * corners[3];
        }

        // too high: 2414
        public int FindMonsters(IEnumerable<string> input)
        {
            var tiles = MyParse(input).ToList();
            FindMatches(tiles);

            var corners = tiles.Where(t => t.Matches.Count + t.FlipMatches.Count <= 2).Select(t => t.Id).ToList();
            Dictionary<int, Tile> tileDic = new Dictionary<int, Tile>();
            tiles.ForEach(t => tileDic.Add(t.Id, t));

            // Solve the puzzle

            // solved puzzle has 144 spots, math for rows and columns, convert 0123 NESW into offsets. 
            // Q: pick a corner that matches NESW or pick orientation based on corner.

            // Orientation from corner - first chosen is E, second is S.
            // opposite direction is (ix + 2 % 4)
            // E -> +1, W = -1, N = -12, S = +12
            // convert later pieces to the orientation of the first. 

            // flip and rotate - a function on the tile, given two orientations (0-3) {r1, r2}, rotate so r2 faces r1, optionally flip.
            // get the sides set from the first corner, find (max + 1 % 4) - rotate that side to west, without flipping.
            // for the outgoing sides, flip the target pieces to match, then add them to be processed at the next indexes (+E, +S)
            // transforming the target lists is also key.
            //  * flip -> swap contents of match + flipmatch
            //  * rotate -> rotation is + 0-3 (distance between old and new facing), in the matches, alter source sideId by the same value % 4.

            Stack<int> next = new Stack<int>();
            int[] ordered = new int[tiles.Count];
            HashSet<int> seen = new HashSet<int>();

            ordered[0] = corners.First();

            Tile active = tileDic[ordered[0]];
            List<int> sides = new List<int>();
            sides.AddRange(active.Matches.Select(m => m.SideFirst));
            sides.AddRange(active.FlipMatches.Select(m => m.SideFirst));

            int west = sides.Max() + 1 % 4;

            tileDic[ordered[0]] = active.Rotate(SideEnum.W, west);
            next.Push(0);
            seen.Add(ordered[0]);

            while (next.Count > 0)
            {
                int nextIx = next.Pop();
                int tileId = ordered[nextIx];
                active = tileDic[tileId];

                foreach (SideLink sl in active.Matches)
                {
                    if (seen.Contains(sl.SecondId)) { continue; }
                    if (sl.SideFirst == SideEnum.E)
                    {
                        tileDic[sl.SecondId] = tileDic[sl.SecondId].Rotate(SideEnum.W, sl.SideSecond);
                        ordered[nextIx + 1] = sl.SecondId;
                        next.Push(nextIx + 1);
                        seen.Add(sl.SecondId);

                    }
                    else if (sl.SideFirst == SideEnum.S)
                    {
                        tileDic[sl.SecondId] = tileDic[sl.SecondId].Rotate(SideEnum.N, sl.SideSecond);
                        ordered[nextIx + 12] = sl.SecondId;
                        next.Push(nextIx + 12);
                        seen.Add(sl.SecondId);
                    }
                }

                foreach (SideLink sl in active.FlipMatches)
                {
                    if (seen.Contains(sl.SecondId)) { continue; }
                    if (sl.SideFirst == SideEnum.E)
                    {
                        tileDic[sl.SecondId] = tileDic[sl.SecondId].Rotate(SideEnum.W, sl.SideSecond, flip: true);
                        ordered[nextIx + 1] = sl.SecondId;
                        next.Push(nextIx + 1);
                        seen.Add(sl.SecondId);
                    }
                    else if (sl.SideFirst == SideEnum.S)
                    {

                        tileDic[sl.SecondId] = tileDic[sl.SecondId].Rotate(SideEnum.N, sl.SideSecond, flip: true);
                        ordered[nextIx + 12] = sl.SecondId;
                        next.Push(nextIx + 12);
                        seen.Add(sl.SecondId);
                    }
                }
            }

            List<string> image = MakeImage(ordered, tileDic);

            // for four rotations and two flips, find sea monsters.
            

            List<Tuple<int, int>> pos = AllOrientationMonster(image);

            int chop = image.Select(s => s.Where(c => c == '#').Count()).Sum();
            int monsterChop = pos.Count * 15;

            return chop - monsterChop;
        }

        private List<Tuple<int, int>> AllOrientationMonster(List<string> image)
        {
            for(int rotate = 0; rotate <3; rotate++)
            {
                var rotated = image.RotateContent(rotate);
                var result = RegexMonster(rotated).ToList();
                
                if (result.Count > 0) { return result; }
            }

            image = image.FlipContent(0);

            for (int rotate = 0; rotate < 3; rotate++)
            {
                var rotated = image.RotateContent(rotate);
                var result = RegexMonster(rotated).ToList();

                if (result.Count > 0) { return result; }
            }

            throw new Exception("No Monsters");
        }

        private IEnumerable<Tuple<int, int>> RegexMonster(List<string> image)
        {
            Regex row0 = new Regex("..................#.");
            Regex row1 = new Regex("#....##....##....###");
            Regex row2 = new Regex(".#..#..#..#..#..#...");

            for (int i=1; i < image.Count -1; i++)
            {
                var matches = row1.Matches(image[i]);
                foreach(var m in matches.AsEnumerable())
                {
                    string above = image[i - 1].Substring(m.Index, 20);
                    string below = image[i + 1].Substring(m.Index, 20);
                    if (row0.IsMatch(above) && row2.IsMatch(below))
                    {
                        yield return new Tuple<int, int>(i - 1, m.Index);
                    }
                }
            }
        }
        private List<string> MakeImage(int[] ordered, Dictionary<int, Tile> tileDic)
        {
            var sb = Enumerable.Repeat(0, 12 * 8).Select(x => new StringBuilder(12 * 8)).ToList();
            for (int i = 0; i < ordered.Length; i++)
            {
                int row = i / 12;
                int col = i % 12;
                int tileId = ordered[i];

                Tile t = tileDic[tileId];

                IEnumerable<string> cell = t.Pixels();

                int offset = 0;
                foreach (string s in cell)
                {
                    sb[(row * 8) + offset].Append(s);
                    offset++;
                }
            }

            return sb.Select(sb => sb.ToString()).ToList();
        }

        private IEnumerable<Tile> MyParse(IEnumerable<string> input)
        {
            int id = 0;
            List<string> content = new List<string>();
            foreach(string s in input)
            {
                if (String.IsNullOrWhiteSpace(s))
                {
                    yield return new Tile(id, content);
                    id = 0;
                    content = new List<string>();
                }
                else if (s.StartsWith("Tile "))
                {
                    id = Int32.Parse(s.Substring(5).TrimEnd(':'));
                }
                else
                {
                    content.Add(s);
                }
            }

            if (content.Count > 0)
            {
                yield return new Tile(id, content);
            }
        }

        private void FindMatches(IList<Tile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                for (int j = i + 1; j < tiles.Count; j++)
                {
                    for (int si = 0; si < 4; si++)
                    {
                        Side sidei = tiles[i].Sides[si];

                        for (int sj = 0; sj < 4; sj++)
                        {
                            Side sidej = tiles[j].Sides[sj];
                            var smatch = sidei.Match(sidej);

                            if (smatch != SideMatch.None)
                            {
                                tiles[i].AddMatch(si, tiles[j].Id, sj, smatch);
                                tiles[j].AddMatch(sj, tiles[i].Id, si, smatch);
                            }
                        }

                    }
                }
            }
        }
    }


    static class ImageExtensions
    {
        public static List<string> RotateContent(this List<string> content, int rotate)
        {
            // rotate = 90 degree steps clockwise.

            int len = content.Count;
            List<string> newContent;
            switch (rotate)
            {
                case 0:
                    newContent = content.ToList();
                    break;
                case 1:
                    newContent = new List<string>();
                    for (int i = 0; i < len; i++)
                    {
                        newContent.Add(string.Join("", content.Reverse<string>().Select(s => s[i])));
                    }
                    break;
                case 2:
                    newContent = content.Select(s => String.Join("", s.Reverse())).ToList();
                    newContent.Reverse();
                    break;
                case 3:
                    newContent = new List<string>();
                    for (int i = 0; i < len; i++)
                    {
                        newContent.Add(string.Join("", content.Select(s => s[len - i - 1])));
                    }
                    break;
                default:
                    throw new Exception("Invalid rotation");
            }
            return newContent;
        }

        public static List<string> FlipContent(this List<string> content, int axis)
        {
            // axis = 0 -> North-South ;; 1 -> East-West

            if (axis == 0)
            {
                return content.Select(s => string.Join("", s.Reverse())).ToList();
            }
            else
            {
                return content.Reverse<string>().ToList();
            }
        }
    }
}
