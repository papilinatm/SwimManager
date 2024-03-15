using System;

namespace SwimManager
{
    public class Participant : Swimmer
    {
        public TimeSpan? Time { get; set; }
        public override string ToString()
        {
            return $"{base.ToString()} - {Time?.ToString(@"mm\:ss\.ff") ?? "N/A"}";
        }

        public bool Equals(Swimmer? other)
        {
            return AreSame(this, other);
        }
        public static bool operator ==(Participant obj1, Swimmer obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;
            if (obj1 is null || obj2 is null)
                return false;
            return obj1.Equals(obj2);
        }

        public static bool operator !=(Participant obj1, Swimmer obj2)
        {
            return !(obj1 == obj2);
        }
        public static bool operator ==(Swimmer obj1, Participant obj2)
        {
            return obj2 == obj1;
        }
        public static bool operator !=(Swimmer obj1, Participant obj2)
        {
            return obj2 != obj1;
        }
    }
}
