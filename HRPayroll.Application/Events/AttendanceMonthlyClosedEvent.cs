using System;

namespace HRPayroll.Application.Events
{
    public class AttendanceMonthlyClosedEvent
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
