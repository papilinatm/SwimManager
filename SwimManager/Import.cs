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
        #region xlsx
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
                var g = name.Split(" ").Select(s => Data.GetGenderByName(s)).Where(i => i != null).ToList();

                res.Add(new Swimmer()
                {
                    Name = name,
                    Year = date.Year,
                    Gender = g.Count > 0 ? g[0] : null
                });

                sb.AppendLine(string.Join(",", row));
            }

            return res;
        }
        public static List<Participant> ImportParticipantsFromXLSX(string file)
        {
            if (!Path.Exists(file))
                return [];

            List<Participant> res = new List<Participant>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(file));

            var myWorksheet = xlPackage.Workbook.Worksheets.First();
            var totalColumns = myWorksheet.Dimension.End.Column;
            if (totalColumns < 3)
                return [];
            int time_column = (totalColumns == 4) ? 4 : 5;

            for (int rowNum = 2; rowNum <= myWorksheet.Dimension.End.Row; rowNum++)
            {
                try
                {
                    var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns];
                    TimeSpan time = default;
                    if (myWorksheet.Cells[rowNum, time_column].Value != null)
                        Utils.TryParseTimeSpan(myWorksheet.Cells[rowNum, time_column].Value.ToString(), out time);
                    res.Add(new Participant()
                    {
                        Name = myWorksheet.Cells[rowNum, 1].Value.ToString(),
                        Gender = (Gender)myWorksheet.Cells[rowNum, 2].Value.ToString().ToUpper()[0],
                        Year = (int)(double)(myWorksheet.Cells[rowNum, 3].Value),
                        Time = time == default ? null : time
                    });
                }
                catch (Exception e)
                {

                }
            }
            return res;
        }
        public static List<Participant> ImportRaceResultsFromXLSX(string file)
        {
            return ImportParticipantsFromXLSX(file);
        }
        public static List<Swimmer> ImportSwimmersAndResultsFromXLSX(string file)
        {
            try
            {

                if (!Path.Exists(file))
                    return [];

                HashSet<Swimmer> res = new HashSet<Swimmer>();
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

                        var s = new Swimmer()
                        {
                            Name = myWorksheet.Cells[rowNum, 1].Value.ToString(),
                            Gender = (Gender)myWorksheet.Cells[rowNum, 2].Value.ToString().ToUpper()[0],
                            Year = (int)(double.Parse(myWorksheet.Cells[rowNum, 3].Value.ToString()))
                        };
                        if (!res.Contains(s))
                            res.Add(s);

                        if (totalColumns < 8)//without results
                            continue;

                        TimeSpan time = default;
                        if (myWorksheet.Cells[rowNum, 6].Value != null
                            && Utils.TryParseTimeSpan(myWorksheet.Cells[rowNum, 6].Value.ToString(), out time))
                        {
                            res.First(i => i == s).AllResults.Add(new Result(
                                Data.KeyToStyle[myWorksheet.Cells[rowNum, 4].Value.ToString()],
                                int.Parse(myWorksheet.Cells[rowNum, 5].Value.ToString()),
                                time,
                                DateTime.Parse(myWorksheet.Cells[rowNum, 7].Value.ToString()),
                                myWorksheet.Cells[rowNum, 8].Value.ToString() == "Да"));
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                return res.ToList();
            }
            catch
            {
                return [];
            }
        }

        #endregion
        #region csv
        public static List<Swimmer> ImportSwimmersAndResults(string file, char delimeter = ';')
        {
            List<Swimmer> swimmers = new List<Swimmer>();
            try
            {
                using StreamReader sr = new(file);
                var header = sr.ReadLine().Split(delimeter);
                //parse dates
                List<DateTime> dateTimes = new List<DateTime>();
                for (int i = 3; i < header.Length; i += 3)
                {
                    if (DateTime.TryParse(header[i], out DateTime date))
                        dateTimes.Add(date);
                }

                var str = sr.ReadLine();
                while (str != null)
                {
                    var data = str.Split(',');
                    Swimmer swimmer = new(
                        data[0],
                        data[1].ToLower() == "ж" ? Gender.Female : Gender.Male,
                        int.Parse(data[2])
                        );

                    //ParsePersonalResults
                    for (int i = 3; i < data.Length; i += 3)
                    {
                        if (data[i] == "" || data[i + 1] == "" || data[i + 2] == ""
                            || !Data.KeyToStyle.ContainsKey(data[i])
                            || !int.TryParse(data[i + 1], out int dist)
                            )
                            continue;
                        TimeSpan time;
                        if (Utils.TryParseTimeSpan(data[i + 2], out time))
                            swimmer.AllResults.Add(new(Data.KeyToStyle[data[i]], dist, time, dateTimes[i / 3 - 1], true));
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
                List<DateTime> dateTimes = new List<DateTime>();
                for (int i = 3; i < header.Length; i += 3)
                {
                    if (DateTime.TryParse(header[i], out DateTime date))
                        dateTimes.Add(date);
                }

                var str = sr.ReadLine();
                while (str != null)
                {
                    var data = str.Split(',');
                    Swimmer swimmer = new(
                        data[0],
                        data[1] == "ж" ? Gender.Female : Gender.Male,
                        int.Parse(data[2])
                        );

                    //ParsePersonalResults
                    for (int i = 3; i < data.Length; i += 3)
                    {
                        if (data[i] == "" || data[i + 1] == "" || data[i + 2] == ""
                            || !Data.KeyToStyle.ContainsKey(data[i])
                            || !int.TryParse(data[i + 1], out int dist)
                            )
                            continue;
                        TimeSpan time;
                        if (Utils.TryParseTimeSpan(data[i + 2], out time))
                            swimmer.AllResults.Add(new(Data.KeyToStyle[data[i]], dist, time, dateTimes[i / 3 - 1], true));
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
        public static List<Participant> ImportParticipants(string file, char delimeter = ';')
        {
            List<Participant> participants = new List<Participant>();
            FileInfo fi = new FileInfo(file);

            using StreamReader sr = new(file);
            var header = sr.ReadLine()?.Split(delimeter);
            if (header == null || header?.Length != 4)
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
                    Time = Utils.TryParseTimeSpan(data[3], out TimeSpan time) ? time : null
                });

                str = sr.ReadLine();
            }
            return participants;
        }
        #endregion

    }
}
