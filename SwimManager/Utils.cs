using System;
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
        public static DateTime InputDate()
        {
            DateTime res = default;
            while (!DateTime.TryParse(Console.ReadLine(), out res))
            {
                Console.WriteLine($"Введите дату в формате ДД.ММ.ГГГГ:");
            }
            return res;
        }
        public static Style ChooseStyle()
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
            return style;
        }
        public static void ChooseDiscipline(out Style style, out int dist, out bool short_water)
        {
            style = Utils.ChooseStyle();
            Console.WriteLine("Дистанция, м: ");
            dist = Utils.InputInt(0, 100000);
            Console.WriteLine("Короткая вода: ");
            short_water = Utils.YesNo();
        }
        public static string[] ChooseFilesInDirectory(string dir, string pattern, bool choose_many)
        {
            var files = Directory.GetFiles(dir, pattern);
            if (files.Length==0)
            {
                return [];
            }
            int i = 1;
            foreach ( var file in files )
                Console.WriteLine($"{i++}. {Path.GetFileName(file)}");

            if (choose_many)
            {
                Console.WriteLine("Выбрать все?");
                if (YesNo())
                    return files;

                HashSet<string> res = [];
                Console.WriteLine("Введите номера файлов по одному (номер Enter номер Enter). В конце введите 0");
                while (true)
                {
                    int ind = InputInt(0, files.Length);
                    if (ind == 0)
                        return res.ToArray();
                    res.Add(files[ind]);
                }
            }
            else
            {
                Console.WriteLine("Введите номер файла");
                int ind = InputInt(0, files.Length);
                return ind == 0 ? [] : [files[ind-1]];
            }
        }

        public static void OpenFile(string path)
        {
            new Process { StartInfo = new ProcessStartInfo(Path.GetFullPath(path)) { UseShellExecute = true } }.Start();
        }
        public static bool TryParseTimeSpan(string date, out TimeSpan time)
        {
            return  (double.TryParse(date, out double d) && TryParseTimeSpan(d, out time)) ||
                    TimeSpan.TryParseExact(date, @"s\.ff", CultureInfo.InvariantCulture, out time) ||
                    TimeSpan.TryParseExact(date, @"s", CultureInfo.InvariantCulture, out time) ||
                    TimeSpan.TryParseExact(date, @"m\:s\.ff", CultureInfo.InvariantCulture, out time) ||
                    TimeSpan.TryParseExact(date, @"m\:s", CultureInfo.InvariantCulture, out time) ||
                    TimeSpan.TryParseExact(date, @"h\:m\:s\.ff", CultureInfo.InvariantCulture, out time) ||
                    TimeSpan.TryParseExact(date, @"h\:m\:s", CultureInfo.InvariantCulture, out time);
        }
        public static bool TryParseTimeSpan(double date, out TimeSpan time)
        {
            time = default;
            try
            {
                var dt = DateTime.FromOADate(date);
                time = dt.TimeOfDay;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
