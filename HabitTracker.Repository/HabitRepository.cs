using HabitTracker.Entity;
using HabitTracker.Infrastructure;
using HabitTracker.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Repository
{
    public class HabitRepository : GenericRepository<Habit>, IHabitRepository
    {
        private readonly HabitTrackerDbContext _context;

        public HabitRepository(HabitTrackerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Habit> GetHabitById(Guid id)
        {
            return await _context.Habits
                .Include(h => h.Logs)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task Update(Habit habit)
        {
            _context.Habits.Update(habit);
            await _context.SaveChangesAsync();
        }


        public async Task<Habit> GetHabit(Guid id)
        {
            return await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == id);
        }


        public async Task<IEnumerable<Habit>> GetHabitsByUser(string userId)
        {
            return await _context.Habits
            .Where(h => h.UserId == userId && !h.IsDeleted)
            .OrderBy(h => h.DisplayOrder)
            .Include(h => h.Logs)
            .ToListAsync();

        }

        public async Task<Habit> GetHabitWithLogs(Guid habitId)
        {
            return await _context.Habits
                .Include(h => h.Logs)
                .FirstOrDefaultAsync(h => h.Id == habitId);
        }
    }
}
