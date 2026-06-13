using HRPayroll.Application.DTOs;
using HRPayroll.Application.Interfaces;
using HRPayroll.Domain.Entities;
using HRPayroll.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRPayroll.Application.Features.Payrolls;

public class CalculatePayrollCommand : IRequest<List<PayrollDto>>
{
    public int Month { get; set; }
    public int Year { get; set; }
    public Guid? EmployeeId { get; set; } // If null, calculate for all
}

public class CalculatePayrollCommandHandler : IRequestHandler<CalculatePayrollCommand, List<PayrollDto>>
{
    private readonly IHRPayrollDbContext _context;

    public CalculatePayrollCommandHandler(IHRPayrollDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayrollDto>> Handle(CalculatePayrollCommand request, CancellationToken cancellationToken)
    {
        var configsQuery = _context.SalaryConfigurations.AsQueryable();

        if (request.EmployeeId.HasValue)
        {
            configsQuery = configsQuery.Where(x => x.EmployeeId == request.EmployeeId.Value);
        }

        var configs = await configsQuery.ToListAsync(cancellationToken);
        var results = new List<PayrollDto>();

        foreach (var config in configs)
        {
            // Simple payroll calculation logic
            decimal totalAllowances = config.MealAllowance + config.TransportAllowance;
            decimal totalDeductions = config.InsuranceDeduction + config.OtherDeductions;
            decimal netSalary = config.BaseSalary + totalAllowances - totalDeductions;

            // Check if record already exists
            var existingRecord = await _context.PayrollRecords
                .FirstOrDefaultAsync(x => x.EmployeeId == config.EmployeeId && x.Month == request.Month && x.Year == request.Year, cancellationToken);

            if (existingRecord != null)
            {
                existingRecord.BaseSalary = config.BaseSalary;
                existingRecord.TotalAllowances = totalAllowances;
                existingRecord.TotalDeductions = totalDeductions;
                existingRecord.NetSalary = netSalary;
                existingRecord.CalculatedAt = DateTime.UtcNow;
                
                results.Add(MapToDto(existingRecord));
            }
            else
            {
                var newRecord = new PayrollRecord
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = config.EmployeeId,
                    Month = request.Month,
                    Year = request.Year,
                    BaseSalary = config.BaseSalary,
                    TotalAllowances = totalAllowances,
                    TotalDeductions = totalDeductions,
                    NetSalary = netSalary,
                    Status = PayrollStatus.Pending,
                    CalculatedAt = DateTime.UtcNow
                };

                _context.PayrollRecords.Add(newRecord);
                results.Add(MapToDto(newRecord));
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return results;
    }

    private PayrollDto MapToDto(PayrollRecord record)
    {
        return new PayrollDto
        {
            Id = record.Id,
            EmployeeId = record.EmployeeId,
            Month = record.Month,
            Year = record.Year,
            BaseSalary = record.BaseSalary,
            TotalAllowances = record.TotalAllowances,
            TotalDeductions = record.TotalDeductions,
            NetSalary = record.NetSalary,
            Status = record.Status,
            CalculatedAt = record.CalculatedAt
        };
    }
}
