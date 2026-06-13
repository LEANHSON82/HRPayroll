using System.Threading.Tasks;
using MassTransit;
using MediatR;
using HRPayroll.Application.Events;
using HRPayroll.Application.Features.Payrolls;

namespace HRPayroll.Application.Consumers
{
    public class AttendanceMonthlyClosedEventConsumer : IConsumer<AttendanceMonthlyClosedEvent>
    {
        private readonly IMediator _mediator;

        public AttendanceMonthlyClosedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<AttendanceMonthlyClosedEvent> context)
        {
            var msg = context.Message;
            var command = new CalculatePayrollCommand 
            { 
                Month = msg.Month, 
                Year = msg.Year, 
                EmployeeId = msg.EmployeeId 
            };

            await _mediator.Send(command);
        }
    }
}
