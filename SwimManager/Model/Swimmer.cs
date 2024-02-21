using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SwimManager
{
    public class Swimmer : IEquatable<Swimmer>
    {
        private string full_name;
        private string core_name;
        private string Core_name
        {
            get
            {
                if (core_name == null)
                {
                    var items = full_name.Split(' ');
                    core_name = (items.Length > 2) ? string.Join(' ', items.Take(2)) : full_name;
                }
                return core_name;
            }
        }

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
        public string Name
        {
            get
            {
                return full_name;
            }
            set
            {
                full_name = Regex.Replace(value.Trim(), @"(\s)\1+", "$1");
            }
        }
        public int Year { get; set; }
        public Gender? Gender { get; set; }

        public ICollection<Result> AllResults { get; set; } = new List<Result>();

        public override string ToString()
        {
            return $"{Name}, {((char?)Gender) ?? '-'}, {Year}";
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Core_name, Gender, Year);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as Swimmer);
        }
        public bool Equals(Swimmer? other)
        {
            return !ReferenceEquals(other, null)
                && (ReferenceEquals(this, other) || AreSame(this, other));
        }
        public static bool operator ==(Swimmer obj1, Swimmer obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (ReferenceEquals(obj1, null))
                return false;
            if (ReferenceEquals(obj2, null))
                return false;
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Swimmer obj1, Swimmer obj2) => !(obj1 == obj2);
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
            bool different =
                (main.Year != default && extra.Year != default && main.Year != extra.Year)
                || (main.Gender != null && extra.Gender != null && main.Gender != extra.Gender)
                || (!main.Name.Contains(extra.Name) && !extra.Name.Contains(main.Name));
            return !different;
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
                if (main.Year == default)
                    main.Year = extra.Year;
                if (main.Gender == null)
                    main.Gender = extra.Gender;

                List<Result> all = new List<Result>(main.AllResults);
                all.AddRange(extra.AllResults);
                HashSet<Result> result = new HashSet<Result>(all);
                main.AllResults = result.ToList();

                return true;
            }
            return false;
        }
        public static void MergeSwimmers(List<Swimmer> main, IEnumerable<Swimmer> extra)
        {
            foreach (var i in extra)
            {
                var origin = main.FirstOrDefault(it => Swimmer.AreSame(it, i));
                if (origin == null)
                    main.Add(i);
                else
                    Swimmer.MergeSwimmers(origin, i);
            }
        }

    }
}
