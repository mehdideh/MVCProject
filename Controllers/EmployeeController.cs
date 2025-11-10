using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVCProject.Data;
using MVCProject.Models;
using MVCProject.Dtos;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Caching.Memory;

namespace MVCProject.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeController> _logger;
    private readonly IMemoryCache _cache;
    private readonly Services.Services _empservice;
    private const string CacheKey = "EmployeesAll";
    public EmployeeController(AppDbContext context, ILogger<EmployeeController> logger,IMemoryCache cache,Services.Services empservice)
    {
        _context = context?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
        _cache = cache;
        _empservice = empservice;
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
    public async Task<ActionResult<List<ReturnEmployeeDto>>> GetAll()
    {
        _logger.LogInformation("ورود به متد گرفتن لیست کارمندان");
        if (_cache.TryGetValue(CacheKey, out List<ReturnEmployeeDto> list))
            return Ok(list);
        
        var Emps = await _context.Employees.Where(e => e.isDeleted == false).Select(e=>new ReturnEmployeeDto
        {
            PersonnelCode = e.PersonnelCode,
            Name = e.Name,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber
        }).ToListAsync();

        if (Emps.Any())
        {
            _logger.LogInformation("با کد وضعیت  ");
          var opt = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
        .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(CacheKey, Emps, opt);

            return Ok(Emps);

        }
        else
        {
            _logger.LogInformation($"خروج از متد گرفتن کارمندان  ");

            return NotFound();
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Employee By Id", Description = "Return Employee By Id")]
    [SwaggerResponse(200, "if Employee Found")]
    [SwaggerResponse(404, "if Employee was not Exist")]

    public async Task<ActionResult<ReturnEmployeeDto>> GetById(Guid id)
    {

        _logger.LogInformation("Entered the GetById method");
        var _employee = await _context.Employees.Where(e => e.isDeleted == false && e.Id == id).Select(e => new ReturnEmployeeDto
        {
            PersonnelCode = e.PersonnelCode,
            Name = e.Name,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber
        }).FirstOrDefaultAsync();
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

    // [ApiExplorerSettings(IgnoreApi =true)]

    [HttpGet("GetByPCode/{_personnelcode}")]
    public async Task<ActionResult<ReturnEmployeeDto>> GetByPersonnelCode(string _personnelcode)
    {
        var Emp = await _context.Employees.Where(e => e.PersonnelCode == _personnelcode && e.isDeleted == false).Select(e => new ReturnEmployeeDto
        {
            EmployeeId = e.Id,
            Name = e.Name,
            PersonnelCode = e.PersonnelCode,
            PhoneNumber = e.PhoneNumber,
            Email = e.Email
        }).FirstOrDefaultAsync();
        if (Emp != null)
        {
            return Ok(Emp);
        }
        else
        {
            return NotFound();
        }
    }
    [HttpGet("GetByName/{_Name}")]
    public async Task<ActionResult<ReturnEmployeeDto>> GetByName(string _Name)
    {
        var Emp = await _context.Employees.Where(e => e.isDeleted == false && e.Name == _Name).Select(e => new ReturnEmployeeDto
        {
            PersonnelCode = e.PersonnelCode,
            Name = e.Name,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber
        }).FirstOrDefaultAsync();
        if (Emp != null) 
        {
            return Ok(Emp);
        }
        else
        {
            return NotFound();
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
    public async Task<ActionResult<ReturnEmployeeDto>> Create(CreateEmployeeDto dto)
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
            isDeleted = false,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email
        };
        _logger.LogInformation($"Employee {EmpNew.Name} Created");
        await _context.Employees.AddAsync(EmpNew);
        await _context.SaveChangesAsync();
        var Result = new ReturnEmployeeDto
        {
            Name = EmpNew.Name,
            PersonnelCode = EmpNew.PersonnelCode,
            Email = EmpNew.Email,
            PhoneNumber = EmpNew.PhoneNumber

        };
        _cache.Remove("EmployeesAll");
        return CreatedAtAction("GetById", new { id =EmpNew.Id },Result);
            
        
    }

    [HttpPut("Update/{id}")]
    [SwaggerOperation(Summary = "Update Employee", Description = "Update the Employee with the specified ID")]
    [SwaggerResponse(404, "if Employee was not Exist")]
    [SwaggerResponse(200, "if Employee Update Successfully ")]
    public async Task<IActionResult> UpdateById(UpdateEmployeeDto dto)
    {
        // if (await _context.Employees.AnyAsync(e => e.PersonnelCode == dto.PersonnelCode && e.Id != id))
        // {
        //     return Conflict("PersonnelCode already exists.");
        // }
        var Employee = await _context.Employees.FirstOrDefaultAsync(e => e.PersonnelCode == dto.PersonnelCode && e.isDeleted == false);
        if (Employee == null)
        {
            _logger.LogWarning("Employee Not Found");
            return NotFound($"Employee {dto.PersonnelCode} Not Found");
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
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            Employee.Email = dto.Email;
        }
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            Employee.PhoneNumber = dto.PhoneNumber;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Employee {dto.PersonnelCode} Updated");
        _cache.Remove("EmployeesAll");
        return Ok();


    }

    [HttpPut("Update/by_personnelcode/{_personnelcode}")]
    [SwaggerOperation(Summary = "Update Employee by PersonnelCode")]
    [SwaggerResponse(404, "if Employee was not Exist")]
    [SwaggerResponse(200, "if Employee Update Successfully ")]
    public async Task<IActionResult> Update(string _personnelcode , UpdateEmployeeDto dto)
    {
        var Employee = await _context.Employees.FirstOrDefaultAsync(e => e.PersonnelCode == _personnelcode && e.isDeleted == false);
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
         if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            Employee.Email = dto.Email;
        }
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            Employee.PhoneNumber = dto.PhoneNumber;
        }
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Employee {_personnelcode} Updated");
        _cache.Remove("EmployeesAll");
        return Ok();
    }

    [HttpDelete("Delete/{id}")]
    [SwaggerOperation(Summary = "Update Employee", Description = "Update the Employee with the specified ID")]
    [SwaggerResponse(404, "if Employee was not Exist")]
    [SwaggerResponse(204, "if Employee Delete Successfully with NoContent Method")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var Employee = await _context.Employees.FirstOrDefaultAsync(e=> e.Id == id && e.isDeleted == false);
        if (Employee == null)
        {
            return NotFound($"Employee {id} Not Found");
        }
        Employee.isDeleted = true;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Employee {id} Deleted");
        _cache.Remove("EmployeesAll");
        return NoContent();
    }
   
}