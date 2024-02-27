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
        static string data_folder = @"data\";
        static string db_folder = data_folder + @"clubs\";
        static string files_folder = data_folder + @"files\";
        static string register_folder = files_folder + @"export\";

        static void Main(string[] args)
        {
            if (!Directory.Exists(data_folder))
                Directory.CreateDirectory(data_folder);
            if (!Directory.Exists(db_folder))
                Directory.CreateDirectory(db_folder);
            if (!Directory.Exists(files_folder))
                Directory.CreateDirectory(files_folder);
            if (!Directory.Exists(register_folder))
                Directory.CreateDirectory(register_folder);


            
            while (true)
            {
                switch (Utils.Menu([
                    "Клуб",
                    "Соревнования",
                    "Инструкция"
                    ]))
                {
                    case 0:
                        return;
                    case 1:
                        {
                            ClubMode();
                            break;
                        }
                    case 2:
                        {
                            RaceMode();
                            break;
                        }
                    case 3:
                        {
                            Help();
                            break;
                        }
                }
            }
        }

        private static void Help()
        {
            Console.WriteLine($"Для генерации стартовых протоколов необходимо загрузить список участников в виде файла .xlsx из 4 столбцов: ФИО, пол (М/Ж), год рождения, заявочное время. Формат столбцов - текст. Время задается в формате минуты:секунды.доли. Если время не задано, пловец попадает в заплывы после тех, у кого указано заявочное время.");
            Console.WriteLine($"Для загрузки файлов используется папка {Path.GetFullPath(files_folder)}.");
            Console.WriteLine();
            Console.WriteLine($"Далее выбирается режим разбиения по заплывам: с разбивкой на категории или без.");
            Console.WriteLine($"Сгенерированные заплывы выводятся на экран, если всё верно, их можно сохранить в файл.");
            Console.WriteLine(); 
            Console.WriteLine($"Результаты соревнований можно сохранить в базу данных. Для этого необходимо загрузить таблицу того же формата, что и список участников, но вместо заявочного времени указать фактическое.");
            Console.WriteLine();
            Console.WriteLine($"Данные о пловцах хранятся в файлах баз данных в папке {Path.GetFullPath(db_folder)}");
            Console.WriteLine();
            Console.WriteLine($"Для добавления пловцов в клуб необходимо создать клуб или выбрать один из уже существующих. Загрузить пловцов можно, сложив в папку {Path.GetFullPath(register_folder)} файлы с выгрузкой списка заявок (или вручную указать путь до другой папки). Будут загружены все люди со статусом 'Зачислен'. Если в БД уже есть пловец с таким ФИО и годом, то новый не создается.");
            Console.WriteLine();
            Console.WriteLine($"Пловцов клуба можно сохранить в таблицу .xlsx, которую потом использовать для генерации протоколов.");
            Console.WriteLine($"Выгрузка пловцов со всеми результатами планируется, но пока не придумался формат. Если есть мысли - пишите разработчику :)");
            Console.WriteLine();
        }
    }
}