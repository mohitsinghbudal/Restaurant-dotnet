namespace HotelManagementSystem.Models.Report
{
    public class DashboardReport
    {
        public IEnumerable<TopItemReport> MostOrderedItems { get; set; } = [];
        public IEnumerable<TopCustomerReport> RegularCustomers { get; set; } = [];
        public FinancialSummary Summary { get; set; } = new();
    }

    public class TopItemReport
    {
        public string ItemName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
    }

    public class TopCustomerReport
    {
        public int SessionId { get; set; } 
        public int VisitCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class FinancialSummary
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
    }
}
