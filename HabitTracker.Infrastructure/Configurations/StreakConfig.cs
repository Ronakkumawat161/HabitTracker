using HabitTracker.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.Infrastructure.Configuration
{
    public class StreakConfig : IEntityTypeConfiguration<Streak>
    {
        public void Configure(EntityTypeBuilder<Streak> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CurrentStreak)
                .HasDefaultValue(0);

            builder.Property(x => x.LongestStreak)
                .HasDefaultValue(0);

            builder.Property(x => x.LastUpdated)
                .IsRequired();
        }
    }
}
