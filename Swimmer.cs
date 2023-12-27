using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SwimManager
{
    internal class PersonalBest
    {
        public Style Style { get; private set; }
        public int Distance { get; private set; }
        public bool ShortWater { get; private set; } = true;
        public TimeSpan Time { get; private set; }
        public DateTime Date { get; private set; } = DateTime.Now;

        public static PersonalBest GeneratePersonalBest(Style style = Style.Freestyle, int distance = 50, bool isShort = true)
        {
            Random rnd = new Random();
            return new PersonalBest() { Style= style, Distance = distance, ShortWater = isShort, Time = TimeSpan.FromSeconds(distance/2 + rnd.Next(0, distance)) };
        }
    }


    internal class Swimmer
    {
        public string Name { get; private set; }
        public int Year { get; private set; }
        public Gender Gender { get; private set; }
        public List<PersonalBest> PersonalBests { get; private set; } = new List<PersonalBest>();

        public override string ToString()
        {
            return $"{Name}, {(char)Gender}, {Year}";
        }
        public string PrintWithPersonalBest(Style style, int distance)
        {
            return ToString() + $", {GetPersonalBest(style, distance)?.Time.ToString() ?? "N/A"}";
        }
        public PersonalBest? GetPersonalBest(Style style, int distance)
        {
            return PersonalBests.FirstOrDefault(pb => pb.Style == style && pb.Distance == distance);
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
                    Gender = isBoy ? Gender.Male : Gender.Female,
                    Year = rnd.Next(2000, 2018),
                    PersonalBests = new List<PersonalBest>() { PersonalBest.GeneratePersonalBest((Style)rnd.Next(2, 5)), PersonalBest.GeneratePersonalBest((Style)rnd.Next(2, 5), 25) }
                }); ;
            }
            return swimmers;
        }
    }
}
