using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advent2020
{
    class Day21 : DayInterface
    {
        class Food
        {
            public HashSet<string> Ingredients;
            public HashSet<string> Allergens;
        }
        public object SolveA(IEnumerable<string> input)
        {
            return NonAllergenCount(input);
        }

        public object SolveB(IEnumerable<string> input)
        {
            return DangerousIngredients(input);
        }

        public int NonAllergenCount(IEnumerable<string> input)
        {
            // parse foods to ingredient set, allergen set
            // for each allergen - set intersection of foods tells you what ingredient it might be.
            List<Food> food = MyParse(input).ToList();

            HashSet<string> allergens = new HashSet<string>();
            food.ForEach(f => allergens.UnionWith(f.Allergens));

            Dictionary<string, HashSet<string>> allergySource = AllergySource(food);

            HashSet<string> unsafeIng = new HashSet<string>();
            foreach (var set in allergySource.Values)
            {
                unsafeIng.UnionWith(set);
            }

            int sum = 0;
            foreach (var f in food)
            {
                HashSet<string> ing = f.Ingredients.ToHashSet();
                ing.ExceptWith(unsafeIng);
                sum += ing.Count;
            }
            return sum;

        }

        private static Dictionary<string, HashSet<string>> AllergySource(List<Food> food)
        {
            Dictionary<string, HashSet<string>> allergySource = new Dictionary<string, HashSet<string>>();
            foreach (var f in food)
            {
                foreach (var a in f.Allergens)
                {
                    if (allergySource.ContainsKey(a))
                    {
                        allergySource[a].IntersectWith(f.Ingredients);
                    }
                    else
                    {
                        allergySource[a] = new HashSet<string>(f.Ingredients);
                    }
                }
            }

            return allergySource;
        }

        private string DangerousIngredients(IEnumerable<string> input)
        {
            List<Food> food = MyParse(input).ToList();

            HashSet<string> allergens = new HashSet<string>();
            food.ForEach(f => allergens.UnionWith(f.Allergens));

            Dictionary<string, HashSet<string>> allergySource = AllergySource(food);

            Queue<string> clear = new Queue<string>();
            foreach(HashSet<string> hs in allergySource.Values)
            {
                if (hs.Count == 1)
                {
                    clear.Enqueue(hs.First());
                }
            }

            while(clear.Count > 0)
            {
                string ing = clear.Dequeue();
                foreach(string a in allergens)
                {
                    if (allergySource[a].Count > 1 &&
                        allergySource[a].Contains(ing))
                    {
                        allergySource[a].Remove(ing);
                        if (allergySource[a].Count == 1)
                        {
                            clear.Enqueue(allergySource[a].First());
                        }
                    }
                }
            }

            // sort result

            var keys = allergens.ToList();
            keys.Sort();

            return String.Join(",", keys.Select(k => allergySource[k].First()));
        }


        private IEnumerable<Food> MyParse(IEnumerable<string> input)
        {
            foreach (string s in input)
            {
                var lists = s.Split(" (contains ");
                var ing = lists[0].Split(" ");
                var all = lists[1].TrimEnd(')').Split(", ");

                yield return new Food() { Ingredients = ing.ToHashSet(), Allergens = all.ToHashSet() };
            }
        }
    }
}
