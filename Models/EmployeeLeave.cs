using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProject.Models;

public class EmployeeLeave : BaseEntity
{

    public Guid EmployeeId { get; set; }


    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; }

    
    [Required]
    public Guid LeaveTypeId { get; set; }

    [ForeignKey("LeaveTypeId")]
    public LeaveType LeaveTypeName { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public double Duration { get; set; }


    public string? Description { get; set; }

    
}