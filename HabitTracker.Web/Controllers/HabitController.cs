using System.Security.Claims;
using HabitTracker.Entity;
using HabitTracker.Model;
using HabitTracker.Model.Dto;
using HabitTracker.Model.ViewModels;
using HabitTracker.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Web.Controllers
{
    [Authorize]
    public class HabitController : Controller
    {
        private readonly IHabitService _habitService;
        private readonly IHabitAnalyticsService _analytics;

        public HabitController(
            IHabitService habitService,
            IHabitAnalyticsService analytics)
        {
            _habitService = habitService;
            _analytics = analytics;
        }

        public async Task<IActionResult> Index(string sort = "none")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var habits = (await _habitService.GetUserHabits(userId)).ToList();

            switch (sort)
            {
                case "name":
                    habits = habits.OrderBy(h => h.Title).ToList();
                    break;

                case "today":
                    habits = habits
                        .OrderByDescending(h => h.Logs.Any(l =>
                            l.Date.Date == DateTime.Today && l.IsCompleted))
                        .ThenBy(h => h.Title)
                        .ToList();
                    break;

                case "none":
                default:
                    break;
            }

            var vm = habits.Select(h => new HabitViewModel
            {
                Id = h.Id,
                Title = h.Title,
                TodayCompleted = h.Logs.Any(l => l.Date.Date == DateTime.Today && l.IsCompleted),
                YesterdayCompleted = h.Logs.Any(l => l.Date.Date == DateTime.Today.AddDays(-1) && l.IsCompleted),
                TwoDaysAgoCompleted = h.Logs.Any(l => l.Date.Date == DateTime.Today.AddDays(-2) && l.IsCompleted)
            }).ToList();

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Toggle(Guid id, DateTime date)
        {
            var result = await _habitService.ToggleHabitCompletion(id, date);
            return Json(new { success = result });
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateHabitViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHabitViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var habits = await _habitService.GetUserHabits(userId);
            int nextOrder = habits.Count() == 0 ? 1 : habits.Max(h => h.DisplayOrder) + 1;

            var habit = new Habit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = model.Title,
                Frequency = model.Frequency,
                Icon = model.Icon,
                IsDeleted = false,
                DisplayOrder = nextOrder,
                Logs = new List<HabitLog>()
            };

            await _habitService.CreateHabit(habit);
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Reorder([FromBody] List<Guid> orderedIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _habitService.ReorderHabits(userId, orderedIds);

            return Json(new { success = true });
        }




        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var habit = await _habitService.GetHabit(id);
            if (habit == null) return NotFound();

            habit.IsDeleted = true;
            await _habitService.UpdateHabit(habit);

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Details(Guid id)
        {
            var habit = await _habitService.GetHabit(id);
            if (habit == null)
                return NotFound();

            var logs = habit.Logs.OrderBy(l => l.Date).ToList();

            var now = DateTime.Now;
            int year = now.Year;
            int month = now.Month;

            int days = DateTime.DaysInMonth(year, month);

            var monthLogs = new Dictionary<DateTime, bool>();

            for (int d = 1; d <= days; d++)
            {
                var date = new DateTime(year, month, d);
                monthLogs[date] =
                    logs.Any(l => l.Date.Date == date && l.IsCompleted);
            }

            var last10 = _analytics.GetLast10Days(logs);
            var streakList = _analytics.GetStreaks(logs);

            var vm = new HabitDetailsViewModel
            {
                Habit = new HabitDto { Id = habit.Id, Title = habit.Title },

                CurrentYear = now.Year,
                CurrentMonth = now.Month,


                Score = _analytics.CalculateScore(logs),
                MonthChange = _analytics.CalculateMonthChange(logs),
                YearChange = _analytics.CalculateYearChange(logs),
                Total = logs.Count(l => l.IsCompleted),

                Last10DaysLabels = last10.Labels,
                Last10DaysValues = last10.Values,

                FirstDay = new DateTime(now.Year, now.Month, 1),
                DaysInMonth = DateTime.DaysInMonth(now.Year, now.Month),
                MonthLogs = monthLogs,

                Streaks = streakList.Take(10).ToList()
            };

            return View(vm);
        }




        [HttpPost]
        public async Task<IActionResult> ToggleFromCalendar(Guid habitId, DateTime date)
        {
            var result = await _habitService.ToggleHabitCompletion(habitId, date);
            return Json(new { success = result });
        }



        [HttpPost]
        public async Task<IActionResult> LoadMonth(Guid habitId, int year, int month)
        {
            var habit = await _habitService.GetHabit(habitId);
            if (habit == null) return NotFound();

            var logs = habit.Logs.ToList();

            var first = new DateTime(year, month, 1);
            int days = DateTime.DaysInMonth(year, month);

            var monthData = new Dictionary<string, bool>();

            for (int d = 1; d <= days; d++)
            {
                var date = new DateTime(year, month, d);

                bool completed = logs.Any(l =>
                    l.Date.Date == date &&
                    l.IsCompleted);

                monthData[date.ToString("yyyy-MM-dd")] = completed;
            }

            return Json(new
            {
                firstDay = ((int)first.DayOfWeek + 6) % 7, // Monday start
                days = monthData
            });
        }

    }
}
