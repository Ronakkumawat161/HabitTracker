using HabitTracker.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.Infrastructure.Configuration
{
    public class HabitLogConfig : IEntityTypeConfiguration<HabitLog>
    {
        public void Configure(EntityTypeBuilder<HabitLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.IsCompleted)
                .HasDefaultValue(false);
        }
    }
}
