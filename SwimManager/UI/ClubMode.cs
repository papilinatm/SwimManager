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
        static string db_folder = @"data\clubs\";
        static string export_folder = @"data\export\";
        private static string ChooseDB()
        {
            if (!Directory.Exists(db_folder))
                Directory.CreateDirectory(db_folder);

            var db_names = Directory.GetFiles(db_folder, "*.db")
                .Select(f => (Path.GetFileNameWithoutExtension(f)))
                .ToList();
            if (db_names.Count > 0)
            {
                int i = 1;
                foreach (var f in db_names)
                    Console.WriteLine($"{i++}. {f}");
                Console.WriteLine($"0. Новый");

                Console.WriteLine($"Выберите действие: ");
                int choose = Utils.InputInt(0, db_names.Count);
                if (choose != 0)
                    return db_names[choose - 1];
            }
            Console.WriteLine($"Название: ");
            return Console.ReadLine();
        }

        private static void ClubMode()
        {
            string db_name = ChooseDB();
            SwimDB db = new SwimDB(db_folder + db_name);
            while (true)
                switch (Menu([
                    "Загрузить данные об учениках",
                    "Выгрузить данные об учениках",
                    "Удалить клуб"
                ]))
                {

                    case 0:
                        {
                            return;
                        }
                    case 1:
                        {
                            ImportSwimmers(db);
                            break;
                        }
                    case 2:
                        {
                            ExportSwimmers(db);
                            break;
                        }
                    case 3:
                        {
                            db.Database.EnsureDeleted();
                            return;
                        }
                }
        }

        private static void ImportSwimmers(SwimDB db)
        {
            string default_folder = "";
            string ext = "";
            int mode = Menu([
                "Загрузить список заявок (выгрузка от учебного отдела)",
                "Загрузить список пловцов с результатами",
                ]);//1 - xsls, 2 - csv
            switch (mode)
            {
                case 0:
                    {
                        return;
                    }
                case 1:
                    {
                        default_folder = @"data\xlsx\";
                        ext = "xlsx";
                        break;
                    }
                case 2:
                    {
                        default_folder = @"data\csv\";
                        ext = "csv";
                        break;
                    }
            }
            string[] paths = { };
            switch (Menu([
                "Загрузить все файлы из папки",
                "Загрузить файл",
                ]))
            {
                case 0:
                    {
                        return;
                    }
                case 1:
                    {
                        Console.WriteLine($"Путь к папке (Enter, если папка по умолчанию - {default_folder}):");
                        var path = Console.ReadLine();
                        if (string.IsNullOrEmpty(path))
                            path = default_folder;
                        if (!Directory.Exists(path))
                        {
                            Console.WriteLine("Папки не существует");
                            return;
                        }
                        paths = Directory.GetFiles(path, $"*.{ext}");
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine($"Путь к файлу с расширением (например, data.{ext}):");
                        var path = Console.ReadLine();
                        if (!File.Exists(path))
                        {
                            Console.WriteLine("Файла не существует");
                            return;
                        }
                        paths = [Path.GetFullPath(path)];
                        break;
                    }
            }

            List<Swimmer> imported_swimmers = new List<Swimmer>();
            switch (mode)
            {
                case 1:
                    {
                        foreach (var path in paths)
                            Swimmer.MergeSwimmers(imported_swimmers, ExportImport.ImportSwimmersFromApplicationList(path));
                        break;
                    }
                case 2:
                    {
                        List<Swimmer> swimmers = new List<Swimmer>();
                        foreach (var path in paths)
                        {
                            try
                            {
                                swimmers = ExportImport.ImportSwimmersAndResults(path, ',');
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    swimmers = ExportImport.ImportSwimmersAndResults(path, ';');
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Неверный формат файла");
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            Swimmer.MergeSwimmers(imported_swimmers, swimmers);
                        }
                        break;
                    }
            }
            if (imported_swimmers.Count() == 0)
            {
                Console.WriteLine("Нет пловцов для импорта");
                return;
            }

            Console.WriteLine("Загружены данные о пловцах:");
            foreach (var s in imported_swimmers)
                Console.WriteLine(s);
            Console.WriteLine();

            db.MergeSwimmers(imported_swimmers);
        }

        private static void ExportSwimmers(SwimDB db)
        {
            List<Swimmer> swimmers = new List<Swimmer>();
            switch (Menu([
                "Экспортировать всех",
                "Выбрать"
                ]))
            {
                case 0:
                    {
                        return;
                    }
                case 1:
                    {
                        swimmers = db.Swimmers.ToList();
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Введите год рождения (от Enter до Enter)");
                        var y1 = Utils.InputInt(1900, DateTime.Now.Year);
                        var y2 = Utils.InputInt(1900, DateTime.Now.Year);
                        int min = y1 > y2 ? y2 : y1;
                        int max = y1 > y2 ? y1 : y2;
                        swimmers = db.Swimmers.Where(sw => sw.Year >= min && sw.Year <= max).ToList();
                        break;
                    }
            }
            if (swimmers.Count == 0)
                Console.WriteLine("Нет пловцов для выгрузки");
            else
            {
                Console.WriteLine("Название файла для сохранения (если файл существует, он будет перезаписан)");
                /*
                var path = Path.GetFullPath(export_folder + Console.ReadLine() + ".csv");
                Console.WriteLine("Разделитель (обычно , или ; в зависимости от настроек программы для просмотра)");

                bool success = ExportImport.ExportSwimmersToCSV(swimmers, path, Console.ReadLine());
                */

                var path = Path.GetFullPath(export_folder + Console.ReadLine() + ".xlsx");
                bool success = ExportImport.ExportSwimmersToXLSX(swimmers, path);
                if (success)
                {
                    Console.WriteLine($"Данные сохранены в {path}. Открыть?");
                    if (YesNo())
                        Utils.OpenFile(path);
                }
                else
                    Console.WriteLine($"Что-то пошло не так :(");
            }
        }
    }
}