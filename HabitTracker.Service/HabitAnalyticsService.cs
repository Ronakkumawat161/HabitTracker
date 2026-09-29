using HabitTracker.Entity;
using HabitTracker.Model;
using HabitTracker.Model.ViewModels;
using HabitTracker.Service.Interface;

namespace HabitTracker.Service
{
    public class HabitAnalyticsService : IHabitAnalyticsService
    {
        public int CalculateScore(IEnumerable<HabitLog> logs)
        {
            if (!logs.Any()) return 0;

            int completed = logs.Count(l => l.IsCompleted);
            int total = logs.Count();

            return (int)Math.Round((completed / (double)total) * 100);
        }

        public int CalculateMonthChange(IEnumerable<HabitLog> logs)
        {
            var now = DateTime.UtcNow;

            int thisMonth = logs.Count(l => l.Date.Month == now.Month &&
                                            l.Date.Year == now.Year &&
                                            l.IsCompleted);

            int lastMonth = logs.Count(l => l.Date.Month == now.AddMonths(-1).Month &&
                                            l.Date.Year == now.AddMonths(-1).Year &&
                                            l.IsCompleted);

            if (lastMonth == 0)
                return thisMonth == 0 ? 0 : 100;

            return (int)Math.Round(((thisMonth - lastMonth) / (double)lastMonth) * 100);
        }

        public int CalculateYearChange(IEnumerable<HabitLog> logs)
        {
            var now = DateTime.UtcNow;

            int thisYear = logs.Count(l => l.Date.Year == now.Year && l.IsCompleted);
            int lastYear = logs.Count(l => l.Date.Year == now.AddYears(-1).Year && l.IsCompleted);

            if (lastYear == 0)
                return thisYear == 0 ? 0 : 100;

            return (int)Math.Round(((thisYear - lastYear) / (double)lastYear) * 100);
        }

        public (List<string> Labels, List<int> Values) GetLast10Days(IEnumerable<HabitLog> logs)
        {
            var labels = new List<string>();
            var values = new List<int>();

            for (int i = 9; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);
                labels.Add(day.ToString("dd MMM"));

                var log = logs.FirstOrDefault(l => l.Date.Date == day.Date);
                values.Add(log != null && log.IsCompleted ? 1 : 0);
            }

            return (labels, values);
        }

        public Dictionary<string, int> GetHeatmapData(List<HabitLog> logs)
        {
            return logs.ToDictionary(
                l => new DateTimeOffset(l.Date.Date).ToUnixTimeSeconds().ToString(),
                l => l.IsCompleted ? 1 : 0
            );
        }


        public List<StreakDto> GetStreaks(IEnumerable<HabitLog> logs)
        {
            var sorted = logs.OrderBy(l => l.Date).ToList();

            var streaks = new List<StreakDto>();
            int count = 0;
            DateTime? start = null;

            for (int i = 0; i < sorted.Count; i++)
            {
                var log = sorted[i];

                if (log.IsCompleted)
                {
                    count++;
                    start ??= log.Date;
                }
                else
                {
                    if (count > 0)
                    {
                        streaks.Add(new StreakDto
                        {
                            Length = count,
                            StartDate = start.Value,
                            EndDate = sorted[i - 1].Date
                        });
                    }
                    count = 0;
                    start = null;
                }
            }

            if (count > 0)
            {
                streaks.Add(new StreakDto
                {
                    Length = count,
                    StartDate = start.Value,
                    EndDate = sorted.Last().Date
                });
            }

            return streaks.OrderByDescending(s => s.Length).ToList();
        }
    }
}
