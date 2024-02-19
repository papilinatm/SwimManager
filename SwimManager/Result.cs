using System;

namespace SwimManager
{
    public class Result
    {
        public Result()
        {
        }

        public Result(Style style, int dist, TimeSpan time, DateTime dateTime, bool isShort=true)
        {
            Style = style;
            Time = time;
            Distance = dist;
            Date = dateTime;
            ShortWater = isShort;
        }
        public long ID { get; set; }
        public Style Style { get; private set; }
        public int Distance { get; private set; }
        public bool ShortWater { get; private set; } = true;
        public TimeSpan Time { get; private set; }
        public DateTime Date { get; private set; } = DateTime.Now;

        public override bool Equals(object? obj)
        {
            return obj is Result && AreSame(this, (Result)obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Date,Style,Distance,Time,ShortWater);
        }
        public static bool AreSame(Result r1, Result r2)
        {
            return r1.Date==r2.Date
                && r1.Style==r2.Style
                && r1.Time==r2.Time
                && r1.Distance==r2.Distance
                && r1.ShortWater==r2.ShortWater;                
        }

        public static Result GeneratePersonalBest(Style style = Style.Freestyle, int distance = 50, bool isShort = true)
        {
            Random rnd = new Random();
            return new Result() { Style= style, Distance = distance, ShortWater = isShort, Time = TimeSpan.FromSeconds(distance/2 + rnd.Next(0, distance)) };
        }
    }
}
