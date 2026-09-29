using HabitTracker.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.Infrastructure.Configuration
{
    public class HabitConfig : IEntityTypeConfiguration<Habit>
    {
        public void Configure(EntityTypeBuilder<Habit> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Icon)
                .HasMaxLength(50);

            builder.Property(x => x.Frequency)
                .HasMaxLength(20)
                .HasDefaultValue("Daily");

            builder.HasMany(x => x.Logs)
                .WithOne()
                .HasForeignKey(x => x.HabitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
