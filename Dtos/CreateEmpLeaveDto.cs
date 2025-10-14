using System.ComponentModel.DataAnnotations;

namespace MVCProject.Dtos;
public class CreateEmpLeaveDto
{
    [Required]
    public string PersonnelCode { get; set; }

    [Required]
    public Guid LeaveTypeId { get; set; }

    [Required]
    public double Duration { get; set; }
}