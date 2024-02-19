using System;
using System.Collections.Generic;
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
            bool again = true;
            while (again)
                switch (Menu([
                    "Загрузить данные об учениках",
                    "Выгрузить данные об учениках",
                    "Удалить клуб"
                ]))
                {

                    case 0:
                        {
                            again = false;
                            break;
                        }
                    case 1:
                        {
                            break;
                        }
                    case 2:
                        {
                            List <Swimmer> swimmers = new List <Swimmer>();
                            switch(Menu([
                                "Экспортировать всех",
                                "Выбрать"
                                ]))
                            {
                                case 0:
                                    {
                                        break;
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
                                        swimmers = db.Swimmers.Where(sw=> sw.Year>=min && sw.Year<=max).ToList();
                                        break;
                                    }
                            }
                            if (swimmers.Count == 0)
                                Console.WriteLine("Нет пловцов для выгрузки");
                            else
                            {
                                Console.WriteLine("Название файла для сохранения (если файл существует, он будет перезаписан)");
                                var path = export_folder + Console.ReadLine() + ".csv";
                                if (ExportSwimmersToCSV(swimmers, path))
                                {
                                    Console.WriteLine($"Данные сохранены в {path}");
                                }
                                else
                                { 
                                    Console.WriteLine($"Что-то пошло не так :(");
                                }


                            }
                            break;
                        }
                    case 3:
                        {
                            db.Database.EnsureDeleted();
                            again = false;
                            break;
                        }
                }
        }

        private static bool ExportSwimmersToCSV(List<Swimmer> swimmers, string path)
        {
            var folder = Path.GetDirectoryName(path);
            if (folder == null)
                return false;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);


            using StreamWriter streamWriter = new StreamWriter(path, false, new UTF8Encoding(true));
            streamWriter.WriteLine(string.Join(",", ["ФИО", "Пол", "Год"]));
            foreach (Swimmer s in swimmers)
            {
                streamWriter.WriteLine($"{s.Name},{(char)s.Gender},{s.Year},");
            }

            return true;
        }
    }
}