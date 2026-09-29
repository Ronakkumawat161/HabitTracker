using System;

namespace HabitTracker.Model.ViewModels
{
    public class StreakDto
    {
        public int Length { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}