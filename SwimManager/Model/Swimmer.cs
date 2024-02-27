using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SwimManager
{
    public class Swimmer : IEquatable<Swimmer>
    {
        private string full_name;
        private string core_name;
        private string Core_name
        {
            get
            {
                if (core_name == null)
                {
                    var items = full_name.Split(' ');
                    core_name = (items.Length > 2) ? string.Join(' ', items.Take(2)) : full_name;
                }
                return core_name;
            }
        }

        public Swimmer(string name, Gender gender, int year)
        {
            Name = name;
            Gender = gender;
            Year = year;
        }

        public Swimmer()
        {
        }
        public int ID { get; internal set; }
        public string Name
        {
            get
            {
                return full_name;
            }
            set
            {
                full_name = Regex.Replace(value.Trim(), @"(\s)\1+", "$1");
            }
        }
        public int Year { get; set; }
        public Gender? Gender { get; set; }

        public ICollection<Result> AllResults { get; set; } = [];

        public override string ToString()
        {
            return $"{Name}, {((char?)Gender) ?? '-'}, {Year}";
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Core_name, Gender, Year);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as Swimmer);
        }
        public bool Equals(Swimmer? other)
        {
            return !ReferenceEquals(other, null)
                && (ReferenceEquals(this, other) || AreSame(this, other));
        }
        public static bool operator ==(Swimmer obj1, Swimmer obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (ReferenceEquals(obj1, null))
                return false;
            if (ReferenceEquals(obj2, null))
                return false;
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Swimmer obj1, Swimmer obj2) => !(obj1 == obj2);
        public string PrintWithPersonalBest(Style style, int distance)
        {
            return ToString() + $", {GetPersonalBest(style, distance)?.Time.ToString(@"mm\:ss\.ff") ?? "N/A"}";
        }
        public Result? GetPersonalBest(Style style, int distance, bool isShort = true)
        {
            return AllResults.Where(r => r.Style == style && r.Distance == distance && r.ShortWater == isShort)
                .ToList()
                .MinBy(r => r.Time);
        }

        public static List<Swimmer> GenerateSwimmers(int count)
        {
            List<string> Boys = ["Егоров Матвей Семёнович", "Максимов Егор Михайлович", "Титов Ярослав Глебович", "Голиков Роман Сергеевич", "Кириллов Артём Артёмович", "Чернов Александр Кириллович", "Ершов Илья Юрьевич", "Покровский Добрыня Артёмович", "Петров Александр Богданович", "Герасимов Лука Анатольевич", "Максимов Александр Матвеевич", "Дьяков Демид Алексеевич", "Федоров Михаил Максимович", "Сергеев Егор Никитич", "Анисимов Даниил Владимирович", "Коновалов Михаил Ярославович", "Дмитриев Елисей Дмитриевич", "Меркулов Иван Фёдорович", "Ширяев Даниил Андреевич", "Злобин Даниил Иванович", "Долгов Кирилл Саввич", "Ковалев Егор Фёдорович", "Шульгин Даниил Николаевич", "Чернов Пётр Александрович", "Денисов Савелий Артёмович", "Зайцев Глеб Арсентьевич", "Попов Артём Маркович", "Кольцов Роман Артемьевич", "Александров Даниил Артурович", "Поляков Дмитрий Тимофеевич", "Уткин Марк Леонидович", "Климов Серафим Романович", "Малинин Вячеслав Богданович", "Беляев Михаил Андреевич", "Иванов Платон Максимович", "Смирнов Михаил Михайлович", "Воробьев Давид Ильич", "Николаев Георгий Кириллович", "Чистяков Владимир Александрович", "Новиков Дмитрий Иванович", "Иванов Артём Артемьевич", "Серов Роман Андреевич", "Майоров Евгений Артёмович", "Балашов Андрей Павлович", "Курочкин Илья Романович", "Шувалов Александр Михайлович", "Кузин Артём Андреевич", "Зубов Василий Вячеславович", "Некрасов Никита Тимурович", "Кузьмин Мирон Лукич ", "Плотников Константин Даниэльевич", "Богданов Георгий Александрович", "Щербаков Артемий Кириллович", "Фомин Роман Фёдорович", "Балашов Александр Саввич", "Филимонов Макар Арсенович", "Бычков Максим Львович", "Сергеев Леонид Алиевич", "Терентьев Кирилл Владиславович", "Царев Михаил Камильевич", "Фролов Максим Степанович", "Соколов Максим Ярославович", "Морозов Лев Дмитриевич", "Тарасов Артём Антонович", "Ульянов Иван Тимурович", "Семин Константин Алексеевич", "Завьялов Лев Олегович", "Поздняков Андрей Денисович", "Кириллов Фёдор Иванович", "Степанов Степан Ильич", "Минаев Глеб Романович", "Старостин Даниил Георгиевич", "Корнеев Павел Тимурович", "Киселев Савва Фёдорович", "Зуев Святослав Григорьевич", "Ларин Иван Даниилович", "Цветков Арсений Матвеевич", "Горелов Алексей Артёмович", "Морозов Мартин Данилович", "Соколов Александр Тимофеевич", "Исаев Артём Тимофеевич", "Кулаков Демид Фёдорович", "Волков Александр Матвеевич", "Софронов Александр Всеволодович", "Макаров Артём Максимович", "Морозов Арсений Артёмович", "Денисов Михаил Владимирович", "Козлов Кирилл Дмитриевич", "Волков Артём Иванович", "Александров Кирилл Ярославович", "Зиновьев Владислав Матвеевич", "Ананьев Марк Андреевич", "Михайлов Алексей Захарович", "Борисов Егор Никитич", "Новиков Артемий Святославович", "Губанов Пётр Михайлович", "Назаров Степан Артемьевич", "Григорьев Никита Максимович", "Яковлев Никита Робертович", "Кузнецов Артемий Сергеевич"];
            List<string> Girls = ["Корчагина София Егоровна", "Андреева Николь Арсентьевна", "Воробьева София Михайловна", "Кравцова Вероника Руслановна", "Петрова Полина Павловна", "Сорокина Анастасия Марковна", "Богданова Маргарита Ильинична", "Гришина Ирина Николаевна", "Воробьева Софья Арсентьевна", "Филиппова Марианна Ивановна", "Анохина Амина Арсентьевна", "Волкова Екатерина Владимировна", "Иванова Ангелина Давидовна", "Ильина Кристина Данииловна", "Баранова Анна Марковна", "Симонова Таисия Артёмовна", "Медведева Алёна Владимировна", "Фирсова Кира Романовна", "Дмитриева Ольга Максимовна", "Волкова Мирослава Максимовна", "Киселева Мелания Максимовна", "Соколова Алиса Тимофеевна", "Кожевникова Алиса Александровна", "Иванова Ульяна Михайловна", "Осипова Анна Мироновна", "Андреева Василиса Георгиевна", "Романова Александра Андреевна", "Левина Софья Тихоновна", "Чистякова Дарина Артемьевна", "Федорова Валерия Мироновна", "Савельева Виктория Владимировна", "Сахарова Татьяна Степановна", "Петухова Милана Мироновна", "Моргунова Ева Захаровна", "Румянцева Анна Георгиевна", "Филимонова Зоя Арсентьевна", "Иванова София Георгиевна", "Лапшина Варвара Михайловна", "Троицкая София Олеговна", "Калинина Александра Марковна", "Макарова Маргарита Михайловна", "Козловская Алиса Мироновна", "Куприянова Елизавета Максимовна", "Жукова Дарья Вячеславовна", "Вишневская Софья Егоровна", "Титова Ярослава Артёмовна", "Ершова Алиса Андреевна", "Гусева Мария Матвеевна", "Фомичева Анна Григорьевна", "Климова Анна Матвеевна", "Смирнова Анна Артёмовна", "Петрова Анастасия Максимовна", "Иванова Полина Фёдоровна", "Белова Алина Сергеевна", "Назарова Анна Данииловна", "Федотова Василиса Андреевна", "Максимова Ксения Львовна", "Ильинская Анастасия Петровна", "Попова Амелия Георгиевна", "Титова Дарья Михайловна", "Глебова Амалия Лукинична", "Петухова Айлин Савельевна", "Панова Анастасия Максимовна", "Белкина Ирина Артёмовна", "Кондратьева Ника Васильевна", "Безрукова Мария Тимофеевна", "Васильева Дарья Витальевна", "Маркелова Майя Максимовна", "Михайлова Мария Даниловна", "Кочеткова София Марковна", "Козлова Ирина Максимовна", "Лаврова Алия Дмитриевна", "Новикова Светлана Егоровна", "Петрова Виктория Фёдоровна", "Клюева Алиса Матвеевна", "Калинина Майя Романовна", "Иванова Варвара Артуровна", "Трофимова Алёна Марковна", "Фомина София Леонидовна", "Моисеева Варвара Даниловна", "Лебедева Василиса Константиновна", "Глебова Стефания Ильинична", "Пономарева Алина Львовна", "Исакова Мария Ильинична", "Степанова Кристина Марковна", "Старикова Виктория Марсельевна", "Горелова Дарья Данииловна", "Алешина Анастасия Марковна", "Ковалева Василиса Михайловна", "Лаптева Арина Владимировна", "Спиридонова Алёна Мироновна", "Андреева Мария Елисеевна", "Анисимова Дарья Тимофеевна", "Орлова Марьям Платоновна", "Лукьянова Милана Марковна", "Яковлева Елена Артёмовна", "Крылова Александра Александровна", "Денисова Таисия Ивановна", "Ильина Ульяна Марковна", "Кулешова Анастасия Матвеевна"];

            List<Swimmer> swimmers = new List<Swimmer>(count);
            Random rnd = new Random();
            int boys = 0;
            int girls = 0;
            for (int i = 0; i < count; i++)
            {
                bool isBoy = rnd.Next(0, 2) == 0;
                swimmers.Add(new Swimmer()
                {
                    Name = isBoy ? Boys[boys++ % 100] : Girls[girls++ % 100],
                    Gender = isBoy ? SwimManager.Gender.Male : SwimManager.Gender.Female,
                    Year = rnd.Next(2000, 2018),
                    AllResults = [Result.GeneratePersonalBest((Style)rnd.Next(2, 5)), Result.GeneratePersonalBest((Style)rnd.Next(2, 5), 25)]
                }); ;
            }
            return swimmers;
        }

        public static bool AreSame(Swimmer main, Swimmer extra)
        {
            bool different =
                (main.Year != default && extra.Year != default && main.Year != extra.Year)
                || (main.Gender != null && extra.Gender != null && main.Gender != extra.Gender)
                || (!main.Name.Contains(extra.Name) && !extra.Name.Contains(main.Name));
            return !different;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="extra"></param>
        /// <returns>true if merged, false if main and extra are different</returns>
        public static bool MergeSwimmers(Swimmer main, Swimmer extra)
        {
            if (AreSame(main, extra))
            {
                if (main.Name.Length < extra.Name.Length)
                    main.Name = extra.Name;
                if (main.Year == default)
                    main.Year = extra.Year;
                if (main.Gender == null)
                    main.Gender = extra.Gender;

                List<Result> all = new List<Result>(main.AllResults);
                all.AddRange(extra.AllResults);
                HashSet<Result> result = new HashSet<Result>(all);
                main.AllResults = result.ToList();

                return true;
            }
            return false;
        }
        public static void MergeSwimmers(List<Swimmer> main, IEnumerable<Swimmer> extra)
        {
            foreach (var i in extra)
            {
                var origin = main.FirstOrDefault(it => Swimmer.AreSame(it, i));
                if (origin == null)
                    main.Add(i);
                else
                    Swimmer.MergeSwimmers(origin, i);
            }
        }

    }
}
