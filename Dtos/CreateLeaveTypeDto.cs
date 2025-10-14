using System.ComponentModel.DataAnnotations;
namespace MVCProject.Dtos;
public class CreateLeaveTypeDto
{
    [Required]
    public string Type { get; set; }
}