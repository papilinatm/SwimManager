using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SwimManager
{
    internal partial class Program
    {
        static bool YesNo()
        {
            Console.WriteLine("1. Да\n2. Нет");
            return Utils.InputInt(1, 2) == 1;
        }
        static int Menu(string[] actions, bool exit_by_zero = true)
        {
            int i = 1;
            foreach (var s in actions)
                Console.WriteLine($"{i++}. {s}");
            if (exit_by_zero)
                Console.WriteLine($"0. Выход");
            Console.WriteLine($"Выберите действие: ");
            return Utils.InputInt(exit_by_zero ? 0 : 1, actions.Length);
        }

        static void Main(string[] args)
        {
            while (true)
            {
                switch (Menu([
                    "Клуб",
                    "Соревнования"
                    ]))
                {
                    case 0:
                        return;
                    case 1:
                        {
                            ClubMode();
                            break;
                        }
                    case 2:
                        {
                            RaceMode();
                            break;
                        }
                }
            }
        }

   }
}