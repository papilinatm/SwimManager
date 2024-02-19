using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SwimManager
{
    internal partial class Program
    {
        static string[] contest_actions = {
                "Загрузить участников",
                "Сгенерировать протоколы",
                "Загрузить результаты",
            };
        static int Menu(string[] actions)
        {
            int i = 1;
            foreach (var s in actions)
                Console.WriteLine($"{i++}. {s}");
            Console.WriteLine($"0. Выход");
            Console.WriteLine($"Выберите действие: ");
            return Utils.InputInt(0, actions.Length);
        }

        static void Main(string[] args)
        {
            List<(int, int)> categories = new()
            {
                (5, 6),
                (7, 8),
                (9, 10),
                (11, 12),
                (13, 14),
                (15, 16),
                (17, 18),
                (18, 22),
                (20, 22),
            };

            while (true)
            {
                switch (Menu([
                    "Клуб",
                    "Соревнования"
                    ]))
                {
                    case 0:
                        return;
                    case 1:
                        {
                            ClubMode();

                            //var swimmers = db.Swimmers.Include(s => s.AllResults).ToList();
                            //GenerateRuns(swimmers, new List<(int, int)> { (20,29), (30, 39)});
                            //GenerateRuns(Parser.ParseApplicationsFromXLSX());
                            break;
                        }
                    case 2:
                        {
                            var swimmers = ParseResults();
                            //db.Swimmers.AddRange(swimmers);
                            //db.SaveChanges();
                            break;
                        }
                }
            }
        }

        static Dictionary<string, Style> keyToStyle = new Dictionary<string, Style>()
        {
            { "кр", Style.Freestyle },
            { "вс", Style.Freestyle },
            { "бат", Style.Butterfly },
            { "сп", Style.Backstroke},
            { "бр", Style.Breaststroke},
            { "кмп", Style.Medley }
        };
        private static List<Swimmer> ParseResults()
        {
            Console.WriteLine("Введите название файла: ");
            string file = Console.ReadLine();
            if (!File.Exists(file))
            {
                Console.WriteLine("Файл не найден");
                return new List<Swimmer>();
            }
            List<Swimmer> swimmers = new List<Swimmer>();
            try
            {
                using StreamReader sr = new(file);
                var header = sr.ReadLine().Split(',');
                //parse dates
                List <DateTime> dateTimes = new List<DateTime>();
                for (int i = 3; i < header.Length; i += 3)
                {
                    if (DateTime.TryParse(header[i], out DateTime date))
                        dateTimes.Add(date);
                }

                var str = sr.ReadLine();
                while (str != null)
                {
                    var data = str.Split(',');
                    Swimmer swimmer = new (
                        data[0],
                        data[1] == "ж" ? Gender.Female : Gender.Male,
                        int.Parse(data[2])
                        );

                    //ParsePersonalResults
                    for (int i = 3; i < data.Length; i += 3)
                    {
                        if (data[i] == "" || data[i + 1] == "" || data[i + 2] == ""
                            || !keyToStyle.ContainsKey(data[i])
                            || !int.TryParse(data[i + 1], out int dist)
                            )
                            continue;

                        if (TimeSpan.TryParseExact(data[i + 2], @"s\.ff", CultureInfo.InvariantCulture, out TimeSpan time) ||
                            TimeSpan.TryParseExact(data[i + 2], @"s", CultureInfo.InvariantCulture, out time) ||
                            TimeSpan.TryParseExact(data[i + 2], @"m\:s\.ff", CultureInfo.InvariantCulture, out time) ||
                            TimeSpan.TryParseExact(data[i + 2], @"m\:s", CultureInfo.InvariantCulture, out time) ||
                            TimeSpan.TryParseExact(data[i + 2], @"h\:m\:s\.ff", CultureInfo.InvariantCulture, out time) ||
                            TimeSpan.TryParseExact(data[i + 2], @"h\:m\:s", CultureInfo.InvariantCulture, out time))
                            swimmer.AllResults.Add(new(keyToStyle[data[i]], dist, time, dateTimes[i / 3 - 1], true));
                    }
                    swimmers.Add(swimmer);  
                    str = sr.ReadLine();
                }
                return swimmers;
            }
            catch (Exception e)
            {
                Console.WriteLine("Неверный формат файла");
                Console.WriteLine(e.Message);
                return new List<Swimmer>();
            }
        }

        private static void GenerateRuns(List<Swimmer>? swimmers=null, List<(int, int)>? categories=null)
        {
            if (swimmers==null)
            {
                Console.WriteLine("Введите количество спортсменов: ");
                int n = Utils.InputInt(1, 1000);

                swimmers = Swimmer.GenerateSwimmers(n);

                Console.WriteLine("Вывести спортсменов? (0 - нет, любая клавиша - да): ");
                if (Console.ReadLine() != "0")
                    foreach (var s in swimmers)
                        Console.WriteLine(s.PrintWithPersonalBest(Style.Freestyle, 50));

                Console.WriteLine("--------------------------");
            }
            if (categories==null)
            {
                categories = new List<(int, int)>();
                Console.WriteLine("Настройка категорий");
                while (true)
                {
                    Console.WriteLine("Введите возраст (границы включаются в категорию), например 14 Enter 18 Enter");
                    int min = Utils.InputInt(1, 100);
                    int max = Utils.InputInt(min, 100);
                    categories.Add((min, max));
                    Console.WriteLine("Добавить еще? Введите 0 для выхода");
                    if (Console.ReadKey(true).KeyChar == '0')
                        break;
                }

            }
            

            List<(int, int)> Categories = categories?? new List<(int, int)>()
            {
                (5,6),
                (7,8),
                (9,10),
                (11,12),
                (13,14),
                (15,16),
                (17,18),
                (18,22),
                (23,29),
                (30,39),
                (40,49),
                (50,59),
                (60,69),
                (70,79),
                (80,89),
                (90,99)
            };

            Console.WriteLine("Введите количество дорожек (2-10): ");
            int pathCount = Utils.InputInt(2, 10);

            Console.WriteLine("Разбить заплывы: ");
            Console.WriteLine("1. По абсолютному времени");
            Console.WriteLine("2. По категориям");
            switch(Utils.InputInt(1, 2))
            {
                case 1:
                    {
                        ProceedCategory(swimmers, null, Gender.Male, Style.Freestyle, 50, pathCount);
                        ProceedCategory(swimmers, null, Gender.Female, Style.Freestyle, 50, pathCount);
                        break;
                    }
                case 2:
                    {
                        foreach (var category in Categories)
                        {
                            ProceedCategory(swimmers, category, Gender.Male, Style.Freestyle, 50, pathCount);
                            ProceedCategory(swimmers, category, Gender.Female, Style.Freestyle, 50, pathCount);
                        }
                        break;
                    }
            }

        }

        private static void ProceedCategory(List<Swimmer> swimmers, (int, int)? category, Gender gender, Style style, int dist, int pathCount)
        {
            int currentYear = DateTime.Now.Year;
            var runs = ContestManager.MakeRuns(swimmers,
                                               gender,
                                               currentYear - (category?.Item2??currentYear),
                                               currentYear - (category?.Item1??0),
                                               style,
                                               dist,
                                               pathCount);
            if (runs.Count == 0)
                return;
            
            int i = 0;
            int runNumber = 1;

            var str = category != null ? ($"{category?.Item1}-{category?.Item2}") : "";
            Console.WriteLine($"{Utils.StyleToString[style]}, {dist} м, {(char)gender}{str}");
            foreach (var run in runs)
            {
                if (run.All(s => s == null))
                {
                    //Console.WriteLine("Нет заплывов\n");
                    continue;
                }
                Console.WriteLine($"----------Заплыв {runNumber++}--------------");
                foreach (var s in run)
                    Console.WriteLine($"{(i++ % pathCount + 1)}.  {s?.PrintWithPersonalBest(Style.Freestyle, dist) ?? " - "}");
                Console.WriteLine();
            }
        }
    }
}