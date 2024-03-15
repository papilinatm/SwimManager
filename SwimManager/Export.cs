using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SwimManager
{
    static public partial class ExportImport
    {
        public static bool ExportSwimmersToCSV(List<Swimmer> swimmers, string path, string delimeter = ";")
        {
            var folder = Path.GetDirectoryName(path);
            if (folder == null)
                return false;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);


            using StreamWriter streamWriter = new StreamWriter(path, false, new UTF8Encoding(true));
            streamWriter.WriteLine(string.Join(delimeter, ["ФИО", "Пол", "Год"]));
            foreach (Swimmer s in swimmers)
                streamWriter.WriteLine(string.Join(delimeter, [s.Name, (char)s.Gender, s.Year]));

            return true;
        }
        public static bool ExportParticipantsToXLSX(List<Swimmer> swimmers, string path)
        {
            var folder = Path.GetDirectoryName(path);
            if (folder == null)
                return false;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (File.Exists(path))
                File.Delete(path);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(path));

            xlPackage.Workbook.Properties.Title = Path.GetFileNameWithoutExtension(path);
            var worksheet = xlPackage.Workbook.Worksheets.Add("1");

            worksheet.Cells[1, 1].Value = "ФИО";
            worksheet.Cells[1, 2].Value = "Пол";
            worksheet.Cells[1, 3].Value = "Год";
            worksheet.Cells[1, 4].Value = "Заявочное время";
            worksheet.Cells[1, 5].Value = "Результат";

            worksheet.Columns[4, 5].Style.Numberformat.Format = "[h]:mm:ss";
            using (var range = worksheet.Cells[1, 1, 1, 5])
                range.Style.Font.Bold = true;

            int i = 1;
            foreach (Swimmer s in swimmers)
            {
                i++;
                worksheet.Cells[i, 1].Value = s.Name;
                worksheet.Cells[i, 2].Value = (char)s.Gender;
                worksheet.Cells[i, 3].Value = s.Year;
            }

            worksheet.Cells[$"A1:E{i}"].AutoFitColumns();
            worksheet.Cells[$"A1:E{i}"].AutoFilter = true;


            xlPackage.Save();

            return true;
        }

        internal static bool ExportPersonalBestsToXLSX(List<Swimmer> swimmers, string path)
        {
            var folder = Path.GetDirectoryName(path);
            if (folder == null)
                return false;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (File.Exists(path))
                File.Delete(path);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(path));

            xlPackage.Workbook.Properties.Title = Path.GetFileNameWithoutExtension(path);
            var worksheet = xlPackage.Workbook.Worksheets.Add("1");
            string[] headers = ["ФИО", "Пол", "Год", "Стиль", "Дистанция", "Короткая вода", "Лучший", "Дата", "Последний", "Дата"];

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cells[1, i + 1].Value = headers[i];

            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                range.Style.Font.Bold = true;

            int ind = 1;
            foreach (Swimmer s in swimmers)
            {
                if (s.AllResults.Count == 0)
                {
                    ind++;
                    worksheet.Cells[ind, 1].Value = s.Name;
                    worksheet.Cells[ind, 2].Value = (char)s.Gender;
                    worksheet.Cells[ind, 3].Value = s.Year;
                    continue;
                }
                foreach (var r in s.PersonalBests)
                {
                    ind++;
                    var last_r = s.GetLastResult(r.Style, r.Distance, r.ShortWater);
                    worksheet.Cells[ind, 1].Value = s.Name;
                    worksheet.Cells[ind, 2].Value = (char)s.Gender;
                    worksheet.Cells[ind, 3].Value = s.Year;
                    worksheet.Cells[ind, 4].Value = Data.StyleToShortString[r.Style];
                    worksheet.Cells[ind, 5].Value = r.Distance;
                    worksheet.Cells[ind, 6].Value = r.ShortWater ? "Да" : "Нет";
                    worksheet.Cells[ind, 7].Value = r.Time.ToString(@"h\:mm\:ss\.ff");
                    worksheet.Cells[ind, 8].Value = r.Date.ToString("dd.MM.yyyy");
                    worksheet.Cells[ind, 9].Value = last_r.Time.ToString(@"h\:mm\:ss\.ff");
                    worksheet.Cells[ind, 10].Value = last_r.Date.ToString("dd.MM.yyyy");
                }
            }

            worksheet.Cells[1, 1, ind, headers.Length].AutoFitColumns();
            worksheet.Cells[1, 1, ind, headers.Length].AutoFilter = true;

            xlPackage.Save();
            return true;
        }
        internal static bool ExportPersonalBestsToXLSX(List<Swimmer> swimmers, string path, Style style, int dist, bool is_short)
        {
            var folder = Path.GetDirectoryName(path);
            if (folder == null)
                return false;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (File.Exists(path))
                File.Delete(path);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(path));

            xlPackage.Workbook.Properties.Title = Path.GetFileNameWithoutExtension(path);
            var worksheet = xlPackage.Workbook.Worksheets.Add($"{Data.StyleToString[style]}, {dist} ({(is_short ? "к" : "д")})");
            string[] headers = ["ФИО", "Пол", "Год", "Лучший", "Дата", "Последний", "Дата"];

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cells[1, i + 1].Value = headers[i];

            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                range.Style.Font.Bold = true;

            int ind = 1;
            foreach (Swimmer s in swimmers)
            {
                var r = s.GetPersonalBest(style, dist, is_short);
                ind++;
                worksheet.Cells[ind, 1].Value = s.Name;
                worksheet.Cells[ind, 2].Value = (char)s.Gender;
                worksheet.Cells[ind, 3].Value = s.Year;
                if (r is not null)
                {
                    var last_r = s.GetLastResult(r.Style, r.Distance, r.ShortWater);
                    worksheet.Cells[ind, 4].Value = r.Time.ToString(@"h\:mm\:ss\.ff");
                    worksheet.Cells[ind, 5].Value = r.Date.ToString("dd.MM.yyyy");
                    worksheet.Cells[ind, 6].Value = last_r.Time.ToString(@"h\:mm\:ss\.ff");
                    worksheet.Cells[ind, 7].Value = last_r.Date.ToString("dd.MM.yyyy");
                }
            }
            worksheet.Cells[1, 1, ind, headers.Length].AutoFitColumns();
            worksheet.Cells[1, 1, ind, headers.Length].AutoFilter = true;

            xlPackage.Save();
            return true;
        }

        internal static bool ExportAllResultsToXLSX(List<Swimmer> swimmers, string path)
        {
            var folder = Path.GetDirectoryName(path);
            if (folder == null)
                return false;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (File.Exists(path))
                File.Delete(path);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(path));

            xlPackage.Workbook.Properties.Title = Path.GetFileNameWithoutExtension(path);
            var worksheet = xlPackage.Workbook.Worksheets.Add("1");
            string[] headers = ["ФИО", "Пол", "Год", "Стиль", "Дистанция", "Результат", "Дата", "Короткая вода"];

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cells[1, i + 1].Value = headers[i];

            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                range.Style.Font.Bold = true;

            int ind = 1;
            foreach (Swimmer s in swimmers)
            {
                if (s.AllResults.Count==0)
                {
                    ind++;
                    worksheet.Cells[ind, 1].Value = s.Name;
                    worksheet.Cells[ind, 2].Value = (char)s.Gender;
                    worksheet.Cells[ind, 3].Value = s.Year;
                    continue;
                }
                foreach (var r in s.AllResults)
                {
                    ind++;
                    worksheet.Cells[ind, 1].Value = s.Name;
                    worksheet.Cells[ind, 2].Value = (char)s.Gender;
                    worksheet.Cells[ind, 3].Value = s.Year;
                    worksheet.Cells[ind, 4].Value = Data.StyleToShortString[r.Style];
                    worksheet.Cells[ind, 5].Value = r.Distance;
                    worksheet.Cells[ind, 6].Value = r.Time.ToString(@"h\:mm\:ss\.ff");
                    worksheet.Cells[ind, 7].Value = r.Date.ToString("dd.MM.yyyy");
                    worksheet.Cells[ind, 8].Value = r.ShortWater?"Да":"Нет";
                }
            }

            worksheet.Cells[1, 1, ind, headers.Length].AutoFitColumns();
            worksheet.Cells[1, 1, ind, headers.Length].AutoFilter = true;

            xlPackage.Save();
            return true;
        }
    }
}
