// Dtos/EmployeeLeaveSummaryDto.cs
public class LeaveSummaryItemDto
{
    public string LeaveType { get; set; } = null!;
    public int Count { get; set; }
    public double TotalDuration { get; set; }    
    public string Unit { get; set; } = "روز"; 
}

public class EmployeeLeaveSummaryDto
{
    public string PersonnelCode { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public List<LeaveSummaryItemDto> Summary { get; set; } = new();
}