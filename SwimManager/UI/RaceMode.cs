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
                            break;
                        }
                }
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
            Console.WriteLine("Стиль: ");
            Style style = (Style)Utils.Menu(
                [
Data.StyleToString[Style.Butterfly],
Data.StyleToString[Style.Backstroke],
Data.StyleToString[Style.Breaststroke],
Data.StyleToString[Style.Freestyle],
Data.StyleToString[Style.Medley]
                ], false);

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
            if(Utils.YesNo())
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