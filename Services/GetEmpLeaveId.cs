using Microsoft.EntityFrameworkCore;
using MVCProject.Data;
using MVCProject.Dtos;

namespace MVCProject.Services;

public class Services
{
    private readonly AppDbContext _context;

    public Services(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Guid?> GetEmpLeaveIdAsync(FindLeaveIdDto dto)
    {
        var leave = await _context.Employeeleaves
            .FirstOrDefaultAsync(l =>
                l.EmployeeId == dto.EmployeeId &&
                l.LeaveTypeId == dto.LeaveTypeId &&
                l.StartDate == dto.StartDate.ToUniversalTime() &&
                l.EndDate == dto.EndDate.ToUniversalTime() &&
                !l.isDeleted);

        return leave?.Id;
    }
    public async Task<Guid?> GetLeaveTypeIdAsync(string leaveTypeName)
    {
        var leaveId = await _context.LeaveTypes.FirstOrDefaultAsync(e => e.Type == leaveTypeName && !e.isDeleted);
        return leaveId?.Id;
    }
}