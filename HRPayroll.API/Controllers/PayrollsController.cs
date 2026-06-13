using HRPayroll.Application.DTOs;
using HRPayroll.Application.Features.Payrolls;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRPayroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayrollsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PayrollDto>>> GetPayrolls([FromQuery] int? month, [FromQuery] int? year, [FromQuery] Guid? employeeId)
    {
        var result = await _mediator.Send(new GetPayrollsQuery
        {
            Month = month,
            Year = year,
            EmployeeId = employeeId
        });
        
        return Ok(result);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculatePayroll([FromBody] CalculatePayrollCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("simulate-event")]
    public async Task<IActionResult> SimulateEvent([FromBody] CalculatePayrollCommand command, [FromServices] MassTransit.IPublishEndpoint publishEndpoint)
    {
        await publishEndpoint.Publish(new HRPayroll.Application.Events.AttendanceMonthlyClosedEvent
        {
            Month = command.Month,
            Year = command.Year,
            EmployeeId = command.EmployeeId
        });
        return Accepted(new { message = "Event published to RabbitMQ. Payroll calculation will run in background." });
    }
}
