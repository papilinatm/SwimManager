using System;

namespace SwimManager
{
    public class Participant : Swimmer
    {
        public TimeSpan? PlannedTime { get; set; }
        public override string ToString()
        {
            return $"{base.ToString()} - {PlannedTime?.ToString(@"mm\:ss\.ff") ?? "N/A"}";
        }
    }
}
