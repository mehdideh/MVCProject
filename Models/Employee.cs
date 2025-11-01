using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using MVCProject.Models;
namespace MVCProject.Models;

public class Employee : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 8, MinimumLength = 8)]
    public string PersonnelCode { get; set; }

    [Required]
    [Phone(ErrorMessage = "فرمت نامعتبر تلفن همراه")]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "فرمت نامعتبر آدرس ایمیل")]
    [MaxLength(50)]
    public string Email { get; set; }

    // [Required]
    // [MaxLength(255, ErrorMessage = "حداکثر طول مسیر 255")]
    // public string PhotoPath { get; set; }

    
}