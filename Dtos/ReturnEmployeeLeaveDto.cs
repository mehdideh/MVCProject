using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MVCProject.Models;

namespace MVCProject.Dtos;
public class ReturnEmployeeLeaveDto
{
    [Required]   
    public Guid Id { get; set; }
    [Required]
    public string PersonnelCode { get; set; }

    public Guid EmployeeId { get; set; }


    [Required]
    public string Name { get; set; }
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