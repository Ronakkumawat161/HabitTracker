using HabitTracker.Entity;
using HabitTracker.Repository.Interface;
using HabitTracker.Service.Interface;

namespace HabitTracker.Service
{
    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IGenericRepository<HabitLog> _logRepository;
        private readonly IGenericRepository<Streak> _streakRepository;

        public HabitService(
            IHabitRepository habitRepository,
            IGenericRepository<HabitLog> logRepository,
            IGenericRepository<Streak> streakRepository)
        {
            _habitRepository = habitRepository;
            _logRepository = logRepository;
            _streakRepository = streakRepository;
        }


        public async Task<IEnumerable<Habit>> GetUserHabits(string userId)
        {
            return (await _habitRepository.GetHabitsByUser(userId))
                   .Where(h => !h.IsDeleted);
        }

        public async Task<Habit> GetHabit(Guid id)
        {
            return await _habitRepository.GetHabitWithLogs(id);
        }

        public async Task CreateHabit(Habit habit)
        {
            await _habitRepository.AddAsync(habit);
            await _habitRepository.SaveChangesAsync();
        }

        public async Task UpdateHabit(Habit habit)
        {
            await _habitRepository.UpdateAsync(habit);
            await _habitRepository.SaveChangesAsync();
        }

        public async Task DeleteHabit(Guid id)
        {
            await _habitRepository.DeleteAsync(id);
            await _habitRepository.SaveChangesAsync();
        }



        public async Task<bool> ToggleHabitCompletion(Guid habitId, DateTime date)
        {
            // Try finding a log for the given date
            var logs = await _logRepository.FindAsync(l =>
                l.HabitId == habitId &&
                l.Date.Date == date.Date);

            var existingLog = logs.FirstOrDefault();

            if (existingLog == null)
            {
                // Add new log
                var newLog = new HabitLog
                {
                    Id = Guid.NewGuid(),
                    HabitId = habitId,
                    Date = date,
                    IsCompleted = true
                };

                await _logRepository.AddAsync(newLog);
            }
            else
            {
                // Toggle completion
                existingLog.IsCompleted = !existingLog.IsCompleted;
                await _logRepository.UpdateAsync(existingLog);
            }

            await _logRepository.SaveChangesAsync();

            // Recalculate streak
            await UpdateStreak(habitId);

            return true;
        }


        private async Task UpdateStreak(Guid habitId)
        {
            var logs = (await _logRepository
                .FindAsync(l => l.HabitId == habitId && l.IsCompleted))
                .OrderByDescending(l => l.Date)
                .ToList();

            int currentStreak = 0;
            DateTime today = DateTime.Today;

            foreach (var log in logs)
            {
                if (log.Date.Date == today.AddDays(-currentStreak))
                {
                    currentStreak++;
                }
                else
                {
                    break;
                }
            }

            var streaks = await _streakRepository.FindAsync(s => s.HabitId == habitId);
            var streak = streaks.FirstOrDefault();

            if (streak == null)
            {
                streak = new Streak
                {
                    Id = Guid.NewGuid(),
                    HabitId = habitId,
                    CurrentStreak = currentStreak,
                    LongestStreak = currentStreak,
                    LastUpdated = DateTime.Now
                };

                await _streakRepository.AddAsync(streak);
            }
            else
            {
                streak.LongestStreak = Math.Max(streak.LongestStreak, currentStreak);
                streak.CurrentStreak = currentStreak;
                streak.LastUpdated = DateTime.Now;

                await _streakRepository.UpdateAsync(streak);
            }

            await _streakRepository.SaveChangesAsync();
        }

        public async Task<Streak> GetStreak(Guid habitId)
        {
            var streaks = await _streakRepository.FindAsync(s => s.HabitId == habitId);
            return streaks.FirstOrDefault();
        }

        public async Task ReorderHabits(string userId, List<Guid> orderedIds)
        {
            var habits = (await _habitRepository.GetHabitsByUser(userId)).ToList();

            for (int i = 0; i < orderedIds.Count; i++)
            {
                var habit = habits.First(h => h.Id == orderedIds[i]);
                habit.DisplayOrder = i + 1;

                await _habitRepository.Update(habit);
            }
        }




    }
}
