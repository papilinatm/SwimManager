using System;
using System.Collections.Generic;
using System.Linq;

namespace SwimManager
{
    public class Swimmer
    {
        public Swimmer(string name, Gender gender, int year)
        {
            Name = name;
            Gender = gender;
            Year = year;
        }

        public Swimmer()
        {
        }
        public int ID { get; internal set; }
        public string Name { get; internal set; }
        public int? Year { get; internal set; }
        public Gender? Gender { get; internal set; }

        public ICollection<Result> AllResults { get; set; } = new List<Result>();

        public override string ToString()
        {
            return $"{Name}, {((char?)Gender) ?? '-'}, {Year}";
        }
        public override bool Equals(object? obj)
        {
            return obj is Swimmer && AreSame(this, (Swimmer)obj);
        }
        public string PrintWithPersonalBest(Style style, int distance)
        {
            return ToString() + $", {GetPersonalBest(style, distance)?.Time.ToString(@"mm\:ss\.ff") ?? "N/A"}";
        }
        public Result? GetPersonalBest(Style style, int distance, bool isShort = true)
        {
            return AllResults.Where(r => r.Style == style && r.Distance == distance && r.ShortWater == isShort)
                .ToList()
                .MinBy(r => r.Time);
        }

        public static List<Swimmer> GenerateSwimmers(int count)
        {
            List<Swimmer> swimmers = new List<Swimmer>(count);
            Random rnd = new Random();
            int boys = 0;
            int girls = 0;
            for (int i = 0; i < count; i++)
            {
                bool isBoy = rnd.Next(0, 2) == 0;
                swimmers.Add(new Swimmer()
                {
                    Name = isBoy ? Data.Boys[boys++ % 100] : Data.Girls[girls++ % 100],
                    Gender = isBoy ? SwimManager.Gender.Male : SwimManager.Gender.Female,
                    Year = rnd.Next(2000, 2018),
                    AllResults = new List<Result>() { Result.GeneratePersonalBest((Style)rnd.Next(2, 5)), Result.GeneratePersonalBest((Style)rnd.Next(2, 5), 25) }
                }); ;
            }
            return swimmers;
        }

        public static bool AreSame(Swimmer main, Swimmer extra)
        {
            return (main.Year == extra.Year) && (main.Gender == extra.Gender) && (main.Name.Contains(extra.Name) || extra.Name.Contains(main.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="extra"></param>
        /// <returns>true if merged, false if main and extra are different</returns>
        public static bool MergeSwimmers(Swimmer main, Swimmer extra)
        {
            if (AreSame(main, extra))
            {
                if (main.Name.Length < extra.Name.Length)
                    main.Name = extra.Name;

                List<Result> all = new List<Result>(main.AllResults);
                all.AddRange(extra.AllResults);
                HashSet<Result> result = new HashSet<Result>(all);
                main.AllResults = result.ToList();

                return true;
            }
            return false;
        }
        public static void MergeSwimmers(List<Swimmer> all, IEnumerable<Swimmer> new_items)
        {
            foreach (var i in new_items)
            {
                var origin = all.FirstOrDefault(it => Swimmer.AreSame(it, i));
                if (origin == null)
                    all.Add(i);
                else
                    Swimmer.MergeSwimmers(origin, i);
            }
        }
    }
}
