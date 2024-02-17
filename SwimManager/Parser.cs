using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SwimManager
{
    static public class Parser
    {
        static public List<Swimmer> ParseApplicationsFromXLSX()
        {
            List<Swimmer> res = new List<Swimmer>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage xlPackage = new(new FileInfo(@"data\xlsx\test.xlsx"));
            
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
    }
}
