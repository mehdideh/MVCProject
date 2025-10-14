using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVCProject.Data;
using MVCProject.Models;
using MVCProject.Dtos;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace MVCProject.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeController> _logger;
    
    public EmployeeController(AppDbContext context, ILogger<EmployeeController> logger)
    {
        _context = context?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
        
    }

    // [HttpGet("Error")]
    // public IActionResult ErrorTest()
    // {
    //     throw new Exception("Testing Exception Handler");
    // }


    [HttpGet]
    [SwaggerOperation(Summary = "Return List of Employees")]
    [SwaggerResponse(200, "Return List of Employees")]
    [SwaggerResponse(404,"if Employees List is Empty")]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("ورود به متد گرفتن لیست کارمندان");

        var Emps = await _context.Employees.Where(e => e.isDeleted == false).ToListAsync();

        if (Emps.Any())
        {
            _logger.LogInformation($"خروج از متد گرفتن کارمندان با کد وضعیت : {HttpContext.Response.StatusCode}");
            

            return Ok(Emps);
        }
        else
        {
            _logger.LogInformation($"خروج از متد گرفتن کارمندان با کد وضعیت : {HttpContext.Response.StatusCode}");

            return NotFound();
        }
    }

    [HttpGet("id")]
    [SwaggerOperation(Summary = "Employee By Id", Description = "Return Employee By Id")]
    [SwaggerResponse(200, "if Employee Found")]
    [SwaggerResponse(404, "if Employee was not Exist")]
    
    public async Task<IActionResult> GetById(Guid id)
    {

        _logger.LogInformation("Entered the GetById method");
        var _employee = await _context.Employees.FindAsync(id);
        if (_employee != null)
        {
            _logger.LogInformation("Employee found");
            return Ok(_employee);
        }
        else
        {
            _logger.LogInformation("Employee Not Found");
            return NotFound($"Id : {id} not Found");
        }
    }

    // [HttpPost("Add")]
    // public async Task<IActionResult> Add()
    // {
    //     var Emp = new Models.Employee
    //     {
    //         Name = "Mehdi",
    //         PersonnelCode = "12345678"

    //     };

    //     await _context.Employees.AddAsync(Emp);
    //     await _context.SaveChangesAsync();
    //     return Ok(Emp);
    //     //return Content("User Created");

    // }


    [HttpPost("Create")]
    [SwaggerOperation(Summary = "Create Employee")]
    [SwaggerResponse(409, "if Employee Was Exist")]
    [SwaggerResponse(400, "if Employee Object is Not valid")]
    [SwaggerResponse(201,"if Employee Created Successfully")]
    public async Task<IActionResult> Create(CreateEmployeeDto dto)
    {

        if (await _context.Employees.AnyAsync(e => e.PersonnelCode == dto.PersonnelCode))
        {
            return Conflict("PersonnelCode already exists.");
        }
        var EmpNew = new Models.Employee
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            PersonnelCode = dto.PersonnelCode,
            isDeleted = false
        };
        _logger.LogInformation($"Employee {EmpNew.Name} Created");
        await _context.Employees.AddAsync(EmpNew);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetById", new { id =EmpNew.Id },EmpNew);
            
        
    }

    [HttpPut("Update/{id}")]
    [SwaggerOperation(Summary = "Update Employee", Description = "Update the Employee with the specified ID")]
    [SwaggerResponse(404, "if Employee was not Exist")]
    [SwaggerResponse(200, "if Employee Update Successfully ")]
    public async Task<IActionResult> UpdateById(Guid id, UpdateEmployeeDto dto)
    {
        // if (await _context.Employees.AnyAsync(e => e.PersonnelCode == dto.PersonnelCode && e.Id != id))
        // {
        //     return Conflict("PersonnelCode already exists.");
        // }
        var Employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (Employee == null)
        {
            _logger.LogWarning("Employee Not Found");
            return NotFound($"Employee {id} Not Found");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            Employee.Name = dto.Name;
        }
        if (!string.IsNullOrWhiteSpace(dto.PersonnelCode))
        {
            Employee.PersonnelCode = dto.PersonnelCode;
        }
        if (dto.isDeleted.HasValue)
        {
            Employee.isDeleted = dto.isDeleted.Value;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Employee {id} Updated");
        return Ok(dto);


    }

    [HttpPut("Update/by_personnelcode/{_personnelcode}")]
    [SwaggerOperation(Summary = "Update Employee by PersonnelCode")]
    [SwaggerResponse(404, "if Employee was not Exist")]
    [SwaggerResponse(200, "if Employee Update Successfully ")]
    public async Task<IActionResult> Update(string _personnelcode , UpdateEmployeeDto dto)
    {
        var Employee = await _context.Employees.FirstOrDefaultAsync(e => e.PersonnelCode == _personnelcode);
        if (Employee == null)
        {
            _logger.LogWarning("Employee Not Found");
            return NotFound($"Employee {_personnelcode} Not Found");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            Employee.Name = dto.Name;
        }
        if (!string.IsNullOrWhiteSpace(dto.PersonnelCode))
        {
            Employee.PersonnelCode = dto.PersonnelCode;
        }
        if (dto.isDeleted.HasValue)
        {
            Employee.isDeleted = dto.isDeleted.Value;
        }
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Employee {_personnelcode} Updated");
        return Ok(dto);
    }

    [HttpDelete("Delete/{id}")]
    [SwaggerOperation(Summary = "Update Employee", Description = "Update the Employee with the specified ID")]
    [SwaggerResponse(404, "if Employee was not Exist")]
    [SwaggerResponse(204, "if Employee Delete Successfully with NoContent Method")]
    public async Task<IActionResult> Delete(int id)
    {
        var Employee = await _context.Employees.FindAsync(id);
        if (Employee == null)
        {
            return NotFound($"Employee {id} Not Found");
        }
        Employee.isDeleted = true;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Employee {id} Deleted");
        return NoContent();
    }
    [ApiExplorerSettings(IgnoreApi =true)]
     public async Task<string> GetEmployeeName(string _personnelcode)
    {
        var Emp = await  _context.Employees.FirstOrDefaultAsync(e => e.PersonnelCode == _personnelcode);
        if (Emp != null && Emp.isDeleted == false)
        {
            return Emp.Name;
        }
        else
        {
            return "Employee Not Found";
        }
    }
   
}