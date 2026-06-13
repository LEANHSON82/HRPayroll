using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRPayroll.Domain.Entities;

public class EmployeeReference
{
    [Key]
    public Guid EmployeeId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public Guid? DepartmentId { get; set; }
    
    [MaxLength(200)]
    public string? DepartmentName { get; set; }

    // Navigation properties for EF Core
    public SalaryConfiguration? SalaryConfiguration { get; set; }
    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
}
