using HRPayroll.Application.DTOs;
using HRPayroll.Application.Features.SalaryConfigs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRPayroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalaryConfigsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalaryConfigsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{employeeId:guid}")]
    public async Task<ActionResult<SalaryConfigDto>> GetConfig(Guid employeeId)
    {
        var result = await _mediator.Send(new GetSalaryConfigQuery { EmployeeId = employeeId });
        
        if (result == null)
            return NotFound(new { Message = "Salary configuration not found for this employee." });
            
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SalaryConfigDto>> UpdateConfig([FromBody] UpdateSalaryConfigCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
