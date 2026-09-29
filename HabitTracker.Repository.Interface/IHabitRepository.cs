using System.Linq.Expressions;
using HabitTracker.Entity;

namespace HabitTracker.Repository.Interface
{
    public interface IHabitRepository : IGenericRepository<Habit>
    {
        Task<IEnumerable<Habit>> GetHabitsByUser(string userId);
        Task<Habit> GetHabitWithLogs(Guid habitId);
        Task<Habit> GetHabit(Guid id);
        Task Update(Habit habit);
        Task<Habit> GetHabitById(Guid id);

    }
}
