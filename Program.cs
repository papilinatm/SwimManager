using System.CodeDom.Compiler;

namespace SwimManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int n=0;
            do 
            {
              Console.WriteLine("Введите количество спортсменов: ");
            }
            while (!int.TryParse(Console.ReadLine(), out n) || n <= 0);
          

            var swimmers = Swimmer.GenerateSwimmers(n);

            Console.WriteLine("Вывести спортсменов? (0 - нет, любая клавиша - да): ");
            if (Console.ReadLine()!="0")
              foreach (var s in swimmers)
                    Console.WriteLine(s.PrintWithPersonalBest(Style.Freestyle, 50));

            Console.WriteLine("--------------------------");

            int pathCount = 7;
            do 
            {
              Console.WriteLine("Введите количество дорожек (2-10): ");
            }
            while (!int.TryParse(Console.ReadLine(), out pathCount) || pathCount < 2 || pathCount > 10);
          
            List<(int, int)> Categories = new List<(int, int)>()
            {
                (5,6),
                (7,8),
                (9,10),
                (11,12),
                (13,14),
                (15,16),
                (17,18),
                (18,99)
            };
            int currentYear = DateTime.Now.Year;
            foreach (var category in Categories)
            {
                Console.WriteLine($"Вольный стиль, 25 м, М{category.Item1}-{category.Item2}" );
                var runs = ContestManager.MakeRuns(swimmers, Gender.Male, currentYear-category.Item2, currentYear-category.Item1, Style.Freestyle, 25, pathCount);
                int i = 0;
                int runNumber = 1;
                foreach (var run in runs)
                {
                    if (run.All(s=> s==null))
                    {
                        Console.WriteLine("Нет заплывов\n");
                        continue;
                    }
                    Console.WriteLine($"----------Заплыв {runNumber++}--------------");
                    foreach (var s in run)
                        Console.WriteLine($"{(i++%pathCount+1)}.  {s?.PrintWithPersonalBest(Style.Freestyle, 25)??" - "}");
                    Console.WriteLine();
                }
            }


        }
    }
}