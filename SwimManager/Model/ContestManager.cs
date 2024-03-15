using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwimManager
{
    internal class ContestManager
    {
        public static List<List<Participant>> MakeRuns(List<Participant> swimmers, Gender gender, int minYear, int maxYear, Style style, int distance, int pathCount = 8)
        {
            List<List<Participant>> res = new List<List<Participant>>();
            var participants = swimmers.Where(s => s.Gender == gender && s.Year >= minYear && s.Year <= maxYear).ToList();
            var qualified = participants.Where(s => s.Time != null).OrderBy(s => s.Time?.TotalSeconds).ToList();
            var unqualified = participants.Where(s => s.Time == null).ToList();
            double runsCount = participants.Count / pathCount + 0.99;
            int middlePath = (int)pathCount / 2;
            List<int> path = new List<int>(pathCount);
            path.AddRange(Enumerable.Range(0, pathCount));
            for (int p = 0; p <= middlePath; p++)
            {
                path[p] = pathCount - 2 * (p + 1);
                path[path.Count - p - 1] = pathCount - 2 * p - 1;
            }
            List<int> pathOrder = new List<int>(pathCount);
            foreach (var p in path.OrderBy(p => p))
                pathOrder.Add(path.IndexOf(p));

            int i = 0;
            for (int n = 0; n < runsCount; n++)
            {
                var run = new List<Participant>(pathCount);
                for (int tmp = 0; tmp < pathCount; tmp++)
                    run.Add(null);
                for (int j = 0; j < pathCount && i < participants.Count; j++, i++)
                {
                    run[pathOrder[j]] = (i < qualified.Count()) ? qualified[i] : unqualified[i - qualified.Count()];
                }
                if (!run.All(s => s is null))
                    res.Add(run);
            }
            if (res.Count > 1)
            {
                if (res.Last().Count(s => s is not null) == 1)
                {
                    res[res.Count - 1][pathOrder[1]] = res[res.Count - 2][pathCount - 1];
                    res[res.Count - 2][pathCount - 1] = null;
                }
            }
            return res;
        }

        public static string ProceedCategory(List<Participant> swimmers, (int, int)? category, Gender gender, Style style, int dist, int pathCount)
        {
            int currentYear = DateTime.Now.Year;
            var runs = MakeRuns(swimmers,
                                               gender,
                                               currentYear - (category?.Item2 ?? currentYear),
                                               currentYear - (category?.Item1 ?? 0),
                                               style,
                                               dist,
                                               pathCount);
            return runs.Count > 0 ? RunsToString(runs, category, gender, style, dist, pathCount) : "";
        }

        private static string RunsToString(List<List<Participant>> runs, (int, int)? category, Gender gender, Style style, int dist, int pathCount)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int runNumber = 1;

            var str = category != null ? ($"{category?.Item1}-{category?.Item2}") : "";
            sb.AppendLine($"{Data.StyleToString[style]}, {dist} м, {(char)gender}{str}");
            foreach (var run in runs)
            {
                if (run.All(s => s is null))
                {
                    //sb.AppendLine("Нет заплывов\n");
                    continue;
                }
                sb.AppendLine($"----------Заплыв {runNumber++}--------------");
                foreach (var s in run)
                    sb.AppendLine($"{(i++ % pathCount + 1)}.  {s?.ToString() ?? " - "}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
