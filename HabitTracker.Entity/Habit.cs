using Microsoft.AspNetCore.Identity;

namespace HabitTracker.Entity
{
    public class Habit
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } // string, not Guid

        public string Title { get; set; }

        public string? Icon { get; set; }

        public string Frequency { get; set; }

        public IdentityUser User { get; set; }

        public int DisplayOrder { get; set; }

        public ICollection<HabitLog> Logs { get; set; }
        public bool IsDeleted { get; set; }

    }
}
