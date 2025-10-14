namespace MVCProject.Dtos;
public class UpdateEmployeeLeaveDto
{
    public string? PersonnelCode { get; set; }

    public Guid? LeaveTypeId { get; set; }

    public double? Duration { get; set; }
}