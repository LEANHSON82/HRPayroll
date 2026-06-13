using HRPayroll.Application.DTOs;
using HRPayroll.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRPayroll.Application.Features.Reports;

public class GetDashboardReportQuery : IRequest<DashboardReportDto>
{
    public int Month { get; set; }
    public int Year { get; set; }
}

public class GetDashboardReportQueryHandler : IRequestHandler<GetDashboardReportQuery, DashboardReportDto>
{
    private readonly IHRPayrollDbContext _context;

    public GetDashboardReportQueryHandler(IHRPayrollDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardReportDto> Handle(GetDashboardReportQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.PayrollRecords
            .AsNoTracking()
            .Where(x => x.Month == request.Month && x.Year == request.Year)
            .ToListAsync(cancellationToken);

        return new DashboardReportDto
        {
            Month = request.Month,
            Year = request.Year,
            TotalEmployeesProcessed = records.Count,
            TotalSalaryFund = records.Sum(x => x.NetSalary),
            TotalAllowances = records.Sum(x => x.TotalAllowances),
            TotalDeductions = records.Sum(x => x.TotalDeductions)
        };
    }
}
