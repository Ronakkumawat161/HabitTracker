using System.Reflection;
using HabitTracker.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Infrastructure
{
    public class HabitTrackerDbContext : IdentityDbContext<IdentityUser>
    {
        public HabitTrackerDbContext(DbContextOptions<HabitTrackerDbContext> options)
            : base(options) { }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitLog> HabitLogs { get; set; }
        public DbSet<Streak> Streaks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
