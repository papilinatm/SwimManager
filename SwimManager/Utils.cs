﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SwimManager
{

    public static class Utils
    {
        public static int InputInt(int min, int max)// where T : struct, IComparable<T>
        {
            int res = min;
            while (!int.TryParse(Console.ReadLine(), out res) || res < min || res > max)
            {
                Console.WriteLine($"Введите число от {min} до {max}:");
            }
            return res;
        }
        public static string[] ChooseFilesInDirectory(string dir, string pattern)
        {
            var files = Directory.GetFiles(dir, pattern);
            int i = 1;
            foreach ( var file in files )
                Console.WriteLine($"{i++}. {Path.GetFileName(file)}");

            Console.WriteLine("Выбрать все?");
            if (YesNo())
                return files;

            HashSet <string> res = [];
            Console.WriteLine("Введите номера файлов по одному (номер Enter номер Enter). В конце введите 0");
            while (true)
            {
                int ind = InputInt(0, files.Length);
                if (ind == 0)
                    return res.ToArray();
                res.Add(files[ind]);
            }
        }

        public static void OpenFile(string path)
        {
            new Process { StartInfo = new ProcessStartInfo(Path.GetFullPath(path)) { UseShellExecute = true } }.Start();
        }
        public static bool TryParseTimeSpan(string date, out TimeSpan time)
        {
            return TimeSpan.TryParseExact(date, @"s\.ff", CultureInfo.InvariantCulture, out time) ||
                                        TimeSpan.TryParseExact(date, @"s", CultureInfo.InvariantCulture, out time) ||
                                        TimeSpan.TryParseExact(date, @"m\:s\.ff", CultureInfo.InvariantCulture, out time) ||
                                        TimeSpan.TryParseExact(date, @"m\:s", CultureInfo.InvariantCulture, out time) ||
                                        TimeSpan.TryParseExact(date, @"h\:m\:s\.ff", CultureInfo.InvariantCulture, out time) ||
                                        TimeSpan.TryParseExact(date, @"h\:m\:s", CultureInfo.InvariantCulture, out time);
        }
        public static bool YesNo()
        {
            Console.WriteLine("1. Да\n2. Нет");
            return Utils.InputInt(1, 2) == 1;
        }
        public static int Menu(string[] actions, bool exit_by_zero = true)
        {
            int i = 1;
            foreach (var s in actions)
                Console.WriteLine($"{i++}. {s}");
            if (exit_by_zero)
                Console.WriteLine($"0. Выход");
            Console.WriteLine($"Выберите действие: ");
            return Utils.InputInt(exit_by_zero ? 0 : 1, actions.Length);
        }

    }

}
