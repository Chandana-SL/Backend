using System.Collections.Generic;

namespace TimeTrack.API.DTOs.Productivity
{
    public class TaskDistributionDto
    {
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
    }

    public class ProductivityResponseDto
    {
        public decimal TotalHoursLogged { get; set; }
        public int TaskCompletionRate { get; set; }
        public int EfficiencyScore { get; set; }
        public int CompletedTasks { get; set; }
        public int TotalTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public decimal WeeklyAverage { get; set; }
        public decimal[] DailyHours { get; set; } = new decimal[7];
        public TaskDistributionDto TaskDistribution { get; set; } = new TaskDistributionDto();
    }
}