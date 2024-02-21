using OfficeOpenXml;
using OfficeOpenXml.Style;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static OfficeOpenXml.ExcelErrorValue;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SwimManager
{
    static public class ExportImport
    {
        /// <summary>
        /// экспорт данных из регистра заявков (формат выгрузки с госуслуг)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public List<Swimmer> ImportSwimmersFromApplicationList(string fileName)
        {
            List<Swimmer> res = new List<Swimmer>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(fileName));
            
            var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
            var totalRows = myWorksheet.Dimension.End.Row;
            var totalColumns = myWorksheet.Dimension.End.Column;

            var sb = new StringBuilder(); 
            for (int rowNum = 6; rowNum <= totalRows; rowNum++) //select starting row here
            {
                var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns];
                var data = row.Select(c => c.Value == null ? string.Empty : (string)c.Value.ToString()).ToList();
                if (!data[9].Contains("Зачислен"))
                    continue;
                var date = DateTime.FromOADate(double.Parse(data[5]));
                var name = data[2];
                var g = name.Split(" ").Select(s => Data.GetGenderByName(s)).Where(i=>i!=null).ToList();
                
                res.Add(new Swimmer()
                {
                    Name = name,
                    Year = date.Year,
                    Gender = g.Count > 0 ? g[0]: null
                });

                sb.AppendLine(string.Join(",", row));
            }

            return res;
        }

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
                streamWriter.WriteLine(string.Join(delimeter, [s.Name, (char)s.Gender,s.Year]));

            return true;
        }
        public static bool ExportSwimmersToXLSX(List<Swimmer> swimmers, string path)
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
            using (var range = worksheet.Cells[1, 1, 1, 3])
                range.Style.Font.Bold = true;

            
            int i = 1;
            foreach (Swimmer s in swimmers)
            {
                i++;
                worksheet.Cells[i, 1].Value = s.Name;
                worksheet.Cells[i, 2].Value = (char)s.Gender;
                worksheet.Cells[i, 3].Value = s.Year;
            }
            
            worksheet.Cells[ $"A1:C{i}" ].AutoFitColumns();
            worksheet.Cells[ $"A1:C{i}" ].AutoFilter = true;


            xlPackage.Save();

            return true;
        }
        public static List<Swimmer> ImportSwimmersAndResults(string file, char delimeter=';')
        {
            List<Swimmer> swimmers = new List<Swimmer>();
            try
            {
                using StreamReader sr = new(file);
                var header = sr.ReadLine().Split(delimeter);
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
                            || !Utils.keyToStyle.ContainsKey(data[i])
                            || !int.TryParse(data[i + 1], out int dist)
                            )
                            continue;
                        TimeSpan time;
                        if (Utils.TryParseTimeSpan(data[i+2], out time))
                            swimmer.AllResults.Add(new(Utils.keyToStyle[data[i]], dist, time, dateTimes[i / 3 - 1], true));
                    }
                    swimmers.Add(swimmer);  
                    str = sr.ReadLine();
                }
                return swimmers;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public static List<Swimmer> ImportRaceResults(string file)
        {
            List<Swimmer> swimmers = new List<Swimmer>();
            try
            {
                using StreamReader sr = new(file);
                var header = sr.ReadLine().Split(';');
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
                            || !Utils.keyToStyle.ContainsKey(data[i])
                            || !int.TryParse(data[i + 1], out int dist)
                            )
                            continue;
                        TimeSpan time;
                        if (Utils.TryParseTimeSpan(data[i+2], out time))
                            swimmer.AllResults.Add(new(Utils.keyToStyle[data[i]], dist, time, dateTimes[i / 3 - 1], true));
                    }
                    swimmers.Add(swimmer);  
                    str = sr.ReadLine();
                }
                return swimmers;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public static List<Participant> ImportParticipantsFromXLSX(string file)
        {
            if(!Path.Exists(file))
                return [];

            List<Participant> res = new List<Participant>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(file));

            var myWorksheet = xlPackage.Workbook.Worksheets.First(); 
            var totalColumns = myWorksheet.Dimension.End.Column;
            if (totalColumns < 3)
                return [];

            for (int rowNum = 2; rowNum <= myWorksheet.Dimension.End.Row; rowNum++) 
            {
                try
                {
                    var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns];
                    TimeSpan time = default;
                    if (myWorksheet.Cells[rowNum, 4].Value!=null)
                        Utils.TryParseTimeSpan(myWorksheet.Cells[rowNum, 4].Value.ToString(), out time);
                    res.Add(new Participant()
                    {
                        Name = myWorksheet.Cells[rowNum, 1].Value.ToString(),
                        Gender = (Gender)myWorksheet.Cells[rowNum, 2].Value.ToString().ToUpper()[0],
                        Year = (int)(double)(myWorksheet.Cells[rowNum, 3].Value),
                        PlannedTime = time==default?null:time
                    });
                }
                catch (Exception e)
                {

                }
            }
            return res;
        }       
        public static List<Participant> ImportParticipants(string file, char delimeter = ';')
        {
            List<Participant> participants = new List<Participant>();
            FileInfo fi = new FileInfo(file);
            
            using StreamReader sr = new(file);
            var header = sr.ReadLine().Split(delimeter);
            if (header.Length != 4)
                throw new FormatException("Количество столбцов должно быть 4");

            var str = sr.ReadLine();
            while (str != null)
            {
                var data = str.Split(delimeter);
                participants.Add(new()
                {
                    Name = data[0],
                    Gender = data[1].ToLower() == "ж" ? Gender.Female : Gender.Male,
                    Year = int.Parse(data[2]),
                    PlannedTime = Utils.TryParseTimeSpan(data[3], out TimeSpan time) ? time : null
                });

                str = sr.ReadLine();
            }
            return participants;
        }

    }
}
