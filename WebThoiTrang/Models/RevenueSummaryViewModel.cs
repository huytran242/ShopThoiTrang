namespace WebThoiTrang.Models
{
    public class RevenueSummaryViewModel
    {
        public decimal TotalRevenue { get; set; }
        public List<MonthlyRevenue> MonthlyRevenue { get; set; }
        public decimal DailyRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
   
    }
}
