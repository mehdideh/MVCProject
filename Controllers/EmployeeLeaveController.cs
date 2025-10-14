using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MVCProject.Data;
using MVCProject.Dtos;
using MVCProject.Models;
using Swashbuckle.AspNetCore.Annotations;
namespace MVCProject.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class EmployeeLeave : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeLeave> _logger;
    public EmployeeLeave(AppDbContext context , ILogger<EmployeeLeave> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Return List of Employees", Description = "return List of Employees")]
    [SwaggerResponse(200, "Return List of Employee leave Records")]
    [SwaggerResponse(404,"if EmployeesLeave List is Empty")]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Entered GetAll Method");

        var EmpLeaveList = await _context.Employeeleaves.ToListAsync();
        if (string.IsNullOrWhiteSpace(EmpLeaveList.ToString()))
        {
            return NotFound("No Record Created");
        }
        return Ok(EmpLeaveList);
    }
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Return Employee Leave with Specified Id")]
    [SwaggerResponse(200, "Return List of Employee Leave")]
    [SwaggerResponse(404, "if Employee Leave List is Empty")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var EmpLeave = await _context.Employeeleaves.FirstOrDefaultAsync(e => e.Id == id);
        if (EmpLeave != null)
        {
            return Ok(EmpLeave);
        }
        return NotFound("Record Not Found");
    }

    [HttpGet("GetByPCode/{_personnelcode}")]
    [SwaggerOperation(Summary = "Return All Employee Leave with PersonnelCode")]
    [SwaggerResponse(200, "Return List of Employee Leave")]
    [SwaggerResponse(404, "if Employee Leave List is Empty")]
    public async Task<IActionResult> GetByPCode(string _personnelcode)
    {
        var EmpLeave = await _context.Employeeleaves.Where(e => e.Employee.PersonnelCode == _personnelcode).ToListAsync();
        if (EmpLeave != null)
        {
            return Ok(EmpLeave);
        }
        return NotFound("Record Not Found");
    }

    [HttpPost("Create")]
    [SwaggerOperation(Summary = "Create Employee Leave Record")]
    [SwaggerResponse(201, "if Employee Leave Record Create Successfully")]
    
    public async Task<IActionResult> Create(CreateEmpLeaveDto dto)
    {
        _logger.LogInformation("Entered The Create Method");
        var Emp = await _context.Employees.FirstOrDefaultAsync(e => e.PersonnelCode == dto.PersonnelCode);
        var leavetype = await _context.LeaveTypes.FirstOrDefaultAsync(e => e.Id == dto.LeaveTypeId);

        var EmpLeaveObject = new Models.EmployeeLeave
        {
            EmployeeId = Emp.Id,
            LeaveTypeId = dto.LeaveTypeId,
            LeaveTypeName = leavetype,
            Duration = dto.Duration,
            StartDate = DateTime.UtcNow
        };
        _logger.LogInformation($"Object {dto.PersonnelCode} Created");
        await _context.Employeeleaves.AddAsync(EmpLeaveObject);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetById",new { id = EmpLeaveObject.Id });

    }

    [HttpPatch("Update/{id}")]
    [SwaggerOperation(Summary = "Update Employee Leave Record With Specified Id")]
    [SwaggerResponse(200, "if Employee Leave Record Update Successfully")]
    [SwaggerResponse(404,"if Employee Leave with specified Id Was Not Exists")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeLeaveDto dto)
    {
        
        _logger.LogInformation("Entered The Update Method");
        var EmpLeave = await _context.Employeeleaves.FirstOrDefaultAsync(e => e.Id == id);
        if (EmpLeave == null)
        {
            return NotFound("Not Found");
        }
        
        if (dto.LeaveTypeId != null)
        {
            EmpLeave.LeaveTypeId = dto.LeaveTypeId.Value;
        }
        if (dto.Duration != null)
        {
            EmpLeave.Duration = dto.Duration.Value;
        }
        _logger.LogInformation($"Record {id} Updated");
        await _context.SaveChangesAsync();
        return Ok(EmpLeave);


    }

    [HttpDelete("Delete/{id}")]
    [SwaggerOperation(Summary = "Return List of Employees", Description = "return List of Employees")]
    [SwaggerResponse(204, "if Employee Leave Record Delete Successfully")]
    [SwaggerResponse(404,"if Employee Leave Record With Specified Id Was Not Exists")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var Empleave = await _context.Employeeleaves.FirstOrDefaultAsync(e => e.Id == id);
        if (Empleave == null)
        {
            return NotFound("Not Found!");
        }
        _context.Employeeleaves.Remove(Empleave);
        await _context.SaveChangesAsync();
        return NoContent();
    }


   
}