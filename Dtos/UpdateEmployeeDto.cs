using System.ComponentModel.DataAnnotations;

namespace MVCProject.Dtos;
public class UpdateEmployeeDto
{
    //public Guid id { get; set; }
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string PersonnelCode { get; set; }
    public bool? isDeleted { get; set; }
    [Required]
    [Phone(ErrorMessage = "فرمت نامعتبر تلفن همراه")]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "فرمت نامعتبر آدرس ایمیل")]
    [MaxLength(50)]
    public string Email { get; set; }
}