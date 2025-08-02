using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MVCProject.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 8, MinimumLength = 8)]
    public string PersonnelCode { get; set; }
    
    public bool isDeleted { get; set; } = false;

}