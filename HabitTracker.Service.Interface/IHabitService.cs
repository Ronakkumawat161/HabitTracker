using HabitTracker.Entity;

namespace HabitTracker.Service.Interface
{
    public interface IHabitService
    {
        Task<IEnumerable<Habit>> GetUserHabits(string userId);
        Task<Habit> GetHabit(Guid id);

        Task CreateHabit(Habit habit);
        Task UpdateHabit(Habit habit);
        Task DeleteHabit(Guid id);

        Task<bool> ToggleHabitCompletion(Guid habitId, DateTime date);

        Task<Streak> GetStreak(Guid habitId);
        Task ReorderHabits(string userId, List<Guid> orderedIds);


    }
}
