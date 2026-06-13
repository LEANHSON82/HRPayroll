namespace HRPayroll.Application.DTOs;

public class DashboardReportDto
{
    public int TotalEmployeesProcessed { get; set; }
    public decimal TotalSalaryFund { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
