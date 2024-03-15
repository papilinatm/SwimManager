using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SwimManager
{
    public enum Gender : ushort
    {
        Male = 'М',
        Female = 'Ж'
    };
    public enum Style : short
    {
        Butterfly = 1,
        Backstroke,
        Breaststroke,
        Freestyle,
        Medley
    }
    public static class Data
    {
        static Dictionary<string, Gender> names = null;

        public static Dictionary<string, Style> KeyToStyle { get; } = new ()
        {
            { "кр", Style.Freestyle },
            { "вс", Style.Freestyle },
            { "бат", Style.Butterfly },
            { "сп", Style.Backstroke},
            { "бр", Style.Breaststroke},
            { "кмп", Style.Medley }
        };
        public static Dictionary<Style, string> StyleToShortString { get; } = new ()
        {
            { Style.Freestyle, "кр"},
            { Style.Butterfly, "бат"},
            { Style.Backstroke, "сп"},
            { Style.Breaststroke, "бр"},
            { Style.Medley, "кмп"}
        };
        public static Dictionary<Style, string> StyleToString { get; } = new ()
        {
            { Style.Butterfly, "Баттерфляй"},
            { Style.Backstroke, "Кроль на спине"},
            { Style.Breaststroke, "Брасс"},
            { Style.Freestyle, "Вольный стиль"},
            { Style.Medley, "Комплексное плавание"}
        };

        public static Gender? GetGenderByName(string name)
        {
            if (names == null)
                names = LoadNames();
            return names.ContainsKey(name)?names[name]:null;
        }
        private static Dictionary<string, Gender> LoadNames()
        {
            Dictionary<string, Gender> res = new Dictionary<string, Gender>();

            using StreamReader rdr = new StreamReader(@"data\names.json");
            var str = rdr.ReadLine();
            while (str != null)
            {
                try
                {
                    var pair = str.Split(",").Select(s => s.Split(":")).ToList();
                    var name = pair.FirstOrDefault(p => p[0].Contains("text"))?[1];
                    var gender = pair.FirstOrDefault(p => p[0].Contains("gender"))?[1];
                    if (gender != null && name != null)
                        res.Add(name.Trim('\"'), gender.Contains('m') ? Gender.Male : Gender.Female);
                }
                catch { }
                str = rdr.ReadLine();
            }

            return res;
        }
    }
}
