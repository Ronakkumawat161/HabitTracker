using HabitTracker.Model.Dto;
using HabitTracker.Model.ViewModels;

namespace HabitTracker.Model
{
    public class HabitDetailsViewModel
    {
        public HabitDto Habit { get; set; }

        // Overview
        public int Score { get; set; }
        public int MonthChange { get; set; }
        public int YearChange { get; set; }
        public int Total { get; set; }

        // Trend charts
        public List<string> Last10DaysLabels { get; set; }
        public List<int> Last10DaysValues { get; set; }

        public List<string> MonthlyLabels { get; set; }  // NEW
        public List<int> MonthlyValues { get; set; }     // NEW

        // Weekly history bars
        public List<string> WeeklyLabels { get; set; }  // NEW
        public List<int> WeeklyValues { get; set; }     // NEW

        // Heatmap
        public Dictionary<string, int> HeatmapData { get; set; }

        // Streaks
        public List<StreakDto> Streaks { get; set; }
        public int DaysInMonth { get; set; }

        public DateTime FirstDay { get; set; }
        public Dictionary<DateTime, bool> MonthLogs { get; set; }

        public int CurrentYear { get; set; }
        public int CurrentMonth { get; set; }



    }

}
