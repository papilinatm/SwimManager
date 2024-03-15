using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SwimManager
{
    internal partial class Program
    {        
        private static void RaceMode()
        {
            while (true)
                switch (Utils.Menu([
                "Сгенерировать протоколы",
                "Загрузить результаты",
                ]))
                {

                    case 0:
                        {
                            return;
                        }
                    case 1:
                        {
                            var swimmers = ImportParticipants();
                            if (swimmers.Count == 0)
                            {
                                Console.WriteLine("Нет участников");
                                return;
                            }
                            Console.WriteLine("Загружены данные о пловцах:");
                            foreach (var s in swimmers)
                                Console.WriteLine(s);
                            Console.WriteLine();

                            GenerateRuns(swimmers);
                            //GenerateRuns(ExportImport.ImportSwimmersFromApplicationList());
                            break;
                        }
                    case 2:
                        {
                            ImportRaceResults();
                            break;
                        }
                }
        }

        private static void ImportRaceResults()
        {
            var path = Utils.ChooseFilesInDirectory(race_results_folder, "*.xlsx", false);
            if (path.Length == 0)
            {
                Console.WriteLine("Нет файла для загрузки");
                return;
            }

            Console.WriteLine("Выберите клуб: ");
            string db_name = ChooseDB();
            SwimDB db = new SwimDB(db_folder + db_name);

            Utils.ChooseDiscipline(out Style style, out int dist, out bool short_water);

            Console.WriteLine("Дата (ДД.ММ.ГГГГ): ");
            var dt = Utils.InputDate();

            var participants = ExportImport.ImportRaceResultsFromXLSX(path[0]);
            var swimmers = db.Swimmers.Include(s => s.AllResults).AsEnumerable();
            foreach (var p in participants)
            {
                if (p.Time == null)
                    continue;
                var r = new Result(style, dist, (TimeSpan)p.Time, dt, short_water);
                var s = swimmers.FirstOrDefault(s => s==p);
                if (s is null)
                    db.Swimmers.Add(new Swimmer
                    {
                        Name = p.Name,
                        Year = p.Year,
                        Gender = p.Gender,
                        AllResults = [r]
                    });
                else
                    s.AllResults.Add(r);
            }
            db.SaveChanges();
        }

        private static List<Participant> ImportParticipants()
        {
            try
            {
                //Console.WriteLine($"Путь к файлу с расширением (например, data.xlsx):");
                //var path = Console.ReadLine();
                var path = Utils.ChooseFilesInDirectory(files_folder, "*.xlsx", false);
                if (path.Length==0)
                {
                    Console.WriteLine("Нет файла для загрузки");
                    return [];
                }
                return ExportImport.ImportParticipantsFromXLSX(path[0]);
                //switch (Path.GetExtension(path))
                //{
                //    //case ".csv":
                //    //    {
                //    //        return ExportImport.ImportParticipants(path);
                //    //    }
                //    case ".xlsx":
                //        {
                //            return ExportImport.ImportParticipantsFromXLSX(path);
                //        }
                //    default:
                //        {
                //            Console.WriteLine("Неподдерживаемое расширение файла");
                //            return [];
                //        }
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return [];
            }
        }
        private static List<(int, int)> EnterCategories()
        {
            var categories = new List<(int, int)>();
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
            return categories;
        }


        private static void GenerateRuns(List<Participant> swimmers)
        {
            Style style = Utils.ChooseStyle();

            Console.WriteLine("Дистанция, м: ");
            int dist = Utils.InputInt(0, 100000);

            Console.WriteLine("Введите количество дорожек (2-10): ");
            int pathCount = Utils.InputInt(2, 10);

            StringBuilder buffer = new StringBuilder();
            Console.WriteLine("Разбить заплывы: ");
            Console.WriteLine("1. По абсолютному времени");
            Console.WriteLine("2. По категориям");
            switch (Utils.InputInt(1, 2))
            {
                case 1:
                    {
                        buffer.AppendLine(ContestManager.ProceedCategory(swimmers, null, Gender.Male, style, dist, pathCount));
                        buffer.AppendLine(ContestManager.ProceedCategory(swimmers, null, Gender.Female, style, dist, pathCount));
                        break;
                    }
                case 2:
                    {
                        var categories = EnterCategories();

                        foreach (var category in categories)
                        {
                            buffer.AppendLine(ContestManager.ProceedCategory(swimmers, category, Gender.Male, style, dist, pathCount));
                            buffer.AppendLine(ContestManager.ProceedCategory(swimmers, category, Gender.Female, style, dist, pathCount));
                        }
                        break;
                    }
            }
            Console.WriteLine(buffer.ToString());
            Console.WriteLine("Сохранить?");
            if (Utils.YesNo())
            {
                Console.WriteLine("Название файла: ");
                string path = Path.GetFullPath($"{race_folder}{Console.ReadLine()}.txt");
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, buffer.ToString());
                Utils.OpenFile(path);
            }

        }
    }
}