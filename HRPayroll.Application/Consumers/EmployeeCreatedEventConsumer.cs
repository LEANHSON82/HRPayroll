using HRCore.Domain.Events;
using HRPayroll.Application.Interfaces;
using HRPayroll.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRPayroll.Application.Consumers;

public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly IHRPayrollDbContext _context;
    private readonly ILogger<EmployeeCreatedEventConsumer> _logger;

    public EmployeeCreatedEventConsumer(IHRPayrollDbContext context, ILogger<EmployeeCreatedEventConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        _logger.LogInformation("N3 received EmployeeCreatedEvent for EmployeeId: {EmployeeId}", context.Message.EmployeeId);

        var existing = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == context.Message.EmployeeId);

        if (existing == null)
        {
            var newRef = new EmployeeReference
            {
                EmployeeId = context.Message.EmployeeId,
                EmployeeCode = context.Message.EmployeeCode,
                FullName = context.Message.FullName,
                DepartmentId = context.Message.DepartmentId
            };

            _context.Employees.Add(newRef);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            _logger.LogInformation("Successfully synced new EmployeeReference to PayrollDB.");
        }
        else
        {
            _logger.LogInformation("EmployeeReference already exists in PayrollDB.");
        }
    }
}
