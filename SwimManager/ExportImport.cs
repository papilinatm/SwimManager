using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
    }
}
