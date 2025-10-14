using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MVCProject.Models;

public class LeaveType :BaseEntity
{
    [Required]
    public string Type { get; set; }
}