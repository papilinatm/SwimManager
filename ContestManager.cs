using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SwimManager
{
    internal class ContestManager
    {
        public static List<List<Swimmer>> MakeRuns(List<Swimmer> swimmers, Gender gender, int minYear, int maxYear, Style style, int distance, int pathCount = 8)
        {
            List<List<Swimmer>> res = new List<List<Swimmer>>();
            var participants = swimmers.Where(s => s.Gender == gender && s.Year >= minYear && s.Year <= maxYear).ToList();
            var qualified = participants.Where(s => s.GetPersonalBest(style, distance) != null).OrderBy(s => s.GetPersonalBest(style, distance).Time.TotalSeconds).ToList();
            var unqualified = participants.Where(s => s.GetPersonalBest(style, distance) == null).ToList();
            double runsCount = participants.Count / pathCount + 0.99;
            int middlePath = (int) pathCount / 2 ;
            List <int> path = new List<int>(pathCount);
            path.AddRange(Enumerable.Range(0, pathCount));
            for (int p = 0; p<= middlePath; p++ )
            {
                path[p] = pathCount - 2*(p+1);
                path[path.Count - p - 1] = pathCount - 2*p - 1;
            }
            List<int> pathOrder = new List<int>(pathCount);
            foreach (var p in path.Order())
                pathOrder.Add(path.IndexOf(p));
            


            int i = 0;
            for (int n = 0; n < runsCount; n++)
            {
                var run = new List<Swimmer>(pathCount);
                for (int tmp = 0; tmp < pathCount; tmp++)
                    run.Add(null);
                for (int j = 0; j < middlePath && i < participants.Count; j++, i++)
                {
                    run[pathOrder[j]] = (i < qualified.Count()) ? qualified[i] : unqualified[i- qualified.Count()];
                }

                res.Add(run);
            }
            return res;
        }
    }
}
