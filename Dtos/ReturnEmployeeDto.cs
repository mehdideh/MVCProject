using System.ComponentModel.DataAnnotations;

namespace MVCProject.Dtos;
public class ReturnEmployeeDto
{
    
    public Guid? EmployeeId { get; set; }
    
    [Required]
    public string PersonnelCode { get; set; }
    
    [Required]
    public string Name { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string Email { get; set; }
}