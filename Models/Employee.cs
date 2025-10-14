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

    
}