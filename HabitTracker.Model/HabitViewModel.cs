using HabitTracker.Entity;

namespace HabitTracker.Model
{
    public class HabitViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public bool TodayCompleted { get; set; }
        public bool YesterdayCompleted { get; set; }
        public bool TwoDaysAgoCompleted { get; set; }
        public Dictionary<DateTime, bool> CalendarMap { get; set; }
        public Dictionary<string, int> HeatmapData { get; set; }


    }
}
