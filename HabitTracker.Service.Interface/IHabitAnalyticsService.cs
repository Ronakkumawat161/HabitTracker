using HabitTracker.Entity;
using HabitTracker.Model.ViewModels;
namespace HabitTracker.Service.Interface
{
    public interface IHabitAnalyticsService
    {
        int CalculateScore(IEnumerable<HabitLog> logs);
        int CalculateMonthChange(IEnumerable<HabitLog> logs);
        int CalculateYearChange(IEnumerable<HabitLog> logs);

        (List<string> Labels, List<int> Values) GetLast10Days(IEnumerable<HabitLog> logs);

        Dictionary<string, int> GetHeatmapData(List<HabitLog> logs);

        List<StreakDto> GetStreaks(IEnumerable<HabitLog> logs);
    }
}
