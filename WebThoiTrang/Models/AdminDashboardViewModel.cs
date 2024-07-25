namespace WebThoiTrang.Models
{
    public class AdminDashboardViewModel
    {
        public RevenueSummaryViewModel RevenueSummary { get; set; }
        public IEnumerable<ProductDto> TopSellingProducts { get; set; }
    }
}
