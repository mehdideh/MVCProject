using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVCProject.Data;
using MVCProject.Models;
using MVCProject.Dtos;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace MVCProject.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }
    private readonly AppDbContext _context;


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {

        var Emps = await _context.Employees.Where(e => e.isDeleted == false).ToListAsync();
    
        if (Emps != null)
        {
            return Ok(Emps);
        }
        else
        {
            return Content("There is No Employee Signed up");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {


        var _employee = await _context.Employees.FindAsync(id);
        if (_employee != null)
        {
            return Ok(_employee);
        }
        else
        {
            return NotFound($"Id : {id} not Found");
        }
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add()
    {
        var Emp = new Models.Employee
        {
            Name = "Mehdi",
            PersonnelCode = "12345678"

        };

        await _context.Employees.AddAsync(Emp);
        await _context.SaveChangesAsync();
        return Ok(Emp);
        //return Content("User Created");

    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] Models.Employee _employee)
    {

        if (!ModelState.IsValid)
            return BadRequest("Model is Not Valid!");
        else
        {
            await _context.Employees.AddAsync(_employee);
            await _context.SaveChangesAsync();
            return Ok(_employee);
        }
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, UpdateEmployeeDto dto)
    {
        if (!ModelState.IsValid)
        {

            return BadRequest("Model is Not Valid ");
        }

        var Employee = await _context.Employees.FirstOrDefaultAsync(e => e.PersonnelCode == id);
        if (Employee == null)
        {
            return NotFound("Employee No Found");
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
        return Ok(Employee);


    }
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var Employee = await _context.Employees.FindAsync(id);
        if (Employee == null)
        {
            return NotFound($"Employee {id} Not Found");
        }
        Employee.isDeleted = true;
        await _context.SaveChangesAsync();
        return Ok($"Employee {id} Marked as Deleted.");
    }
}