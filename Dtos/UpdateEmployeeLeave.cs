using System.ComponentModel.DataAnnotations;

namespace MVCProject.Dtos;
public class UpdateEmployeeLeaveDto
{

    [Required]
    public Guid LeaveTypeId { get; set; }

    
    public string LeaveTypeName { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public double Duration { get; set; }
}