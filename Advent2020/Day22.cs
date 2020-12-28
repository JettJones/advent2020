using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day22 : DayInterface
    {
        public object SolveA(IEnumerable<string> input)
        {
            return WinnerScore(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return RecursiveCombat(input);
        }


        public long RecursiveCombat(IEnumerable<string> input)
        {
            IEnumerable<List<int>> hands = MyParse(input).ToList();

            Queue<int> left = new Queue<int>(hands.First());
            Queue<int> right = new Queue<int>(hands.Last());

            GameResult result = RecurGame(left, right);

            return ScoreFor(result.WinnerHand);
        }

        struct GameResult
        {
            public bool LeftWins;
            public Queue<int> WinnerHand;
        }

        private GameResult RecurGame(Queue<int> left, Queue<int> right)
        {
            HashSet<string> repeatCheck = new HashSet<string>();

            while (left.Count > 0 && right.Count > 0)
            {
                string stateString = ToStateString(left, right);
                if (repeatCheck.Contains(stateString))
                {
                    return new GameResult(){ LeftWins = true, WinnerHand = left};
                }
                repeatCheck.Add(stateString);

                int lc = left.Dequeue();
                int rc = right.Dequeue();
                bool leftWins;

                if (lc > left.Count || rc > right.Count)
                {
                    leftWins = (lc > rc);
                }
                else
                {
                    // recursion
                    var leftq = new Queue<int>(left.Take(lc));
                    var rightq = new Queue<int>(right.Take(rc));
                    var result = RecurGame(leftq, rightq);
                    leftWins = result.LeftWins;
                }

                if (leftWins)
                {
                    left.Enqueue(lc);
                    left.Enqueue(rc);
                }
                else
                {
                    right.Enqueue(rc);
                    right.Enqueue(lc);
                }

            }

            return new GameResult()
            {
                LeftWins = left.Count > 0,
                WinnerHand = left.Count > 0 ? left : right,
            };
        }

        private string ToStateString(Queue<int> left, Queue<int> right)
        {
            return String.Join(",", left) + "||" + String.Join(",", right);
        }

        public long WinnerScore(IEnumerable<string> input)
        {
            // whoever has the 10 will win, just a question of how long.
            IEnumerable<List<int>> hands = MyParse(input).ToList();

            Queue<int> left = new Queue<int>(hands.First());
            Queue<int> right = new Queue<int>(hands.Last());

            while(left.Count > 0 && right.Count > 0)
            {
                int lc = left.Dequeue();
                int rc = right.Dequeue();
                if (lc > rc)
                {
                    left.Enqueue(lc);
                    left.Enqueue(rc);
                } else
                {
                    right.Enqueue(rc);
                    right.Enqueue(lc);
                }
            }

            var winner = left.Count > 0  ? left: right;

            return ScoreFor(winner);
        }

        private long ScoreFor(Queue<int> winner)
        {
            long score = 0;
            while(winner.Count > 0)
            {
                score += winner.Count * winner.Dequeue();
            }

            return score;
        }

        private IEnumerable<List<int>> MyParse(IEnumerable<string> input)
        {
            List<int> cards = new List<int>();
            foreach (string s in input)
            {
                if (s.StartsWith("Player")) { continue; }

                if (string.IsNullOrWhiteSpace(s))
                {
                    yield return cards;
                    cards = new List<int>();
                    continue;
                }

                cards.Add(Int32.Parse(s));
            }

            if (cards.Count > 0)
            {
                yield return cards;
            }
        }
    }
}
