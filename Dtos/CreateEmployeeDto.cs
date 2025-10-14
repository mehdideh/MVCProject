using System.ComponentModel.DataAnnotations;

public class CreateEmployeeDto
{
    [Required]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "PersonnelCode Must Be 8 Characters")]
    public string PersonnelCode { get; set; }
    
    [Required]
    public string Name { get; set; }
}