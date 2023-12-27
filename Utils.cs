﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwimManager
{
    enum Gender : ushort
    {
        Male = 'М',
        Female = 'Ж'
    };

    enum Style: short
    {
        Butterfly=1,
        Backstroke,
        Breaststroke,
        Freestyle,
        Medley
    }

    public class Data
    {

        public static List<string> Boys = new List<string>() { "Егоров Матвей Семёнович", "Максимов Егор Михайлович", "Титов Ярослав Глебович", "Голиков Роман Сергеевич", "Кириллов Артём Артёмович", "Чернов Александр Кириллович", "Ершов Илья Юрьевич", "Покровский Добрыня Артёмович", "Петров Александр Богданович", "Герасимов Лука Анатольевич", "Максимов Александр Матвеевич", "Дьяков Демид Алексеевич", "Федоров Михаил Максимович", "Сергеев Егор Никитич", "Анисимов Даниил Владимирович", "Коновалов Михаил Ярославович", "Дмитриев Елисей Дмитриевич", "Меркулов Иван Фёдорович", "Ширяев Даниил Андреевич", "Злобин Даниил Иванович", "Долгов Кирилл Саввич", "Ковалев Егор Фёдорович", "Шульгин Даниил Николаевич", "Чернов Пётр Александрович", "Денисов Савелий Артёмович", "Зайцев Глеб Арсентьевич", "Попов Артём Маркович", "Кольцов Роман Артемьевич", "Александров Даниил Артурович", "Поляков Дмитрий Тимофеевич", "Уткин Марк Леонидович", "Климов Серафим Романович", "Малинин Вячеслав Богданович", "Беляев Михаил Андреевич", "Иванов Платон Максимович", "Смирнов Михаил Михайлович", "Воробьев Давид Ильич", "Николаев Георгий Кириллович", "Чистяков Владимир Александрович", "Новиков Дмитрий Иванович", "Иванов Артём Артемьевич", "Серов Роман Андреевич", "Майоров Евгений Артёмович", "Балашов Андрей Павлович", "Курочкин Илья Романович", "Шувалов Александр Михайлович", "Кузин Артём Андреевич", "Зубов Василий Вячеславович", "Некрасов Никита Тимурович", "Кузьмин Мирон Лукич ", "Плотников Константин Даниэльевич", "Богданов Георгий Александрович", "Щербаков Артемий Кириллович", "Фомин Роман Фёдорович", "Балашов Александр Саввич", "Филимонов Макар Арсенович", "Бычков Максим Львович", "Сергеев Леонид Алиевич", "Терентьев Кирилл Владиславович", "Царев Михаил Камильевич", "Фролов Максим Степанович", "Соколов Максим Ярославович", "Морозов Лев Дмитриевич", "Тарасов Артём Антонович", "Ульянов Иван Тимурович", "Семин Константин Алексеевич", "Завьялов Лев Олегович", "Поздняков Андрей Денисович", "Кириллов Фёдор Иванович", "Степанов Степан Ильич", "Минаев Глеб Романович", "Старостин Даниил Георгиевич", "Корнеев Павел Тимурович", "Киселев Савва Фёдорович", "Зуев Святослав Григорьевич", "Ларин Иван Даниилович", "Цветков Арсений Матвеевич", "Горелов Алексей Артёмович", "Морозов Мартин Данилович", "Соколов Александр Тимофеевич", "Исаев Артём Тимофеевич", "Кулаков Демид Фёдорович", "Волков Александр Матвеевич", "Софронов Александр Всеволодович", "Макаров Артём Максимович", "Морозов Арсений Артёмович", "Денисов Михаил Владимирович", "Козлов Кирилл Дмитриевич", "Волков Артём Иванович", "Александров Кирилл Ярославович", "Зиновьев Владислав Матвеевич", "Ананьев Марк Андреевич", "Михайлов Алексей Захарович", "Борисов Егор Никитич", "Новиков Артемий Святославович", "Губанов Пётр Михайлович", "Назаров Степан Артемьевич", "Григорьев Никита Максимович", "Яковлев Никита Робертович", "Кузнецов Артемий Сергеевич" };

        public static List<string> Girls = new List<string> {"Корчагина София Егоровна","Андреева Николь Арсентьевна","Воробьева София Михайловна","Кравцова Вероника Руслановна","Петрова Полина Павловна","Сорокина Анастасия Марковна","Богданова Маргарита Ильинична","Гришина Ирина Николаевна","Воробьева Софья Арсентьевна","Филиппова Марианна Ивановна","Анохина Амина Арсентьевна","Волкова Екатерина Владимировна","Иванова Ангелина Давидовна","Ильина Кристина Данииловна","Баранова Анна Марковна","Симонова Таисия Артёмовна","Медведева Алёна Владимировна","Фирсова Кира Романовна","Дмитриева Ольга Максимовна","Волкова Мирослава Максимовна","Киселева Мелания Максимовна","Соколова Алиса Тимофеевна","Кожевникова Алиса Александровна","Иванова Ульяна Михайловна","Осипова Анна Мироновна","Андреева Василиса Георгиевна","Романова Александра Андреевна","Левина Софья Тихоновна","Чистякова Дарина Артемьевна","Федорова Валерия Мироновна","Савельева Виктория Владимировна","Сахарова Татьяна Степановна","Петухова Милана Мироновна","Моргунова Ева Захаровна","Румянцева Анна Георгиевна","Филимонова Зоя Арсентьевна","Иванова София Георгиевна","Лапшина Варвара Михайловна","Троицкая София Олеговна","Калинина Александра Марковна","Макарова Маргарита Михайловна","Козловская Алиса Мироновна","Куприянова Елизавета Максимовна","Жукова Дарья Вячеславовна","Вишневская Софья Егоровна","Титова Ярослава Артёмовна","Ершова Алиса Андреевна","Гусева Мария Матвеевна","Фомичева Анна Григорьевна","Климова Анна Матвеевна","Смирнова Анна Артёмовна","Петрова Анастасия Максимовна","Иванова Полина Фёдоровна","Белова Алина Сергеевна","Назарова Анна Данииловна","Федотова Василиса Андреевна","Максимова Ксения Львовна","Ильинская Анастасия Петровна","Попова Амелия Георгиевна","Титова Дарья Михайловна","Глебова Амалия Лукинична","Петухова Айлин Савельевна","Панова Анастасия Максимовна","Белкина Ирина Артёмовна","Кондратьева Ника Васильевна","Безрукова Мария Тимофеевна","Васильева Дарья Витальевна","Маркелова Майя Максимовна","Михайлова Мария Даниловна","Кочеткова София Марковна","Козлова Ирина Максимовна","Лаврова Алия Дмитриевна","Новикова Светлана Егоровна","Петрова Виктория Фёдоровна","Клюева Алиса Матвеевна","Калинина Майя Романовна","Иванова Варвара Артуровна","Трофимова Алёна Марковна","Фомина София Леонидовна","Моисеева Варвара Даниловна","Лебедева Василиса Константиновна","Глебова Стефания Ильинична","Пономарева Алина Львовна","Исакова Мария Ильинична","Степанова Кристина Марковна","Старикова Виктория Марсельевна","Горелова Дарья Данииловна","Алешина Анастасия Марковна","Ковалева Василиса Михайловна","Лаптева Арина Владимировна","Спиридонова Алёна Мироновна","Андреева Мария Елисеевна","Анисимова Дарья Тимофеевна","Орлова Марьям Платоновна","Лукьянова Милана Марковна","Яковлева Елена Артёмовна","Крылова Александра Александровна","Денисова Таисия Ивановна","Ильина Ульяна Марковна","Кулешова Анастасия Матвеевна" };

    }

}