using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Entity
{
    public class Streak
    {
        public Guid Id { get; set; }

        public Guid HabitId { get; set; }

        public int CurrentStreak { get; set; }

        public int LongestStreak { get; set; }

        public DateTime LastUpdated { get; set; }

        public Habit Habit { get; set; }
    }
}
