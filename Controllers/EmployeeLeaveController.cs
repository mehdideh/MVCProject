using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Caching.Memory;
using MVCProject.Data;
using MVCProject.Dtos;
using MVCProject.Models;
using Swashbuckle.AspNetCore.Annotations;
using MVCProject.Services;
using SQLitePCL;
namespace MVCProject.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class EmployeeLeave : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeLeave> _logger;
    private readonly IMemoryCache _cache;
    private readonly Services.Services _leaveService;


    private const string CacheKey = "EmployeeLeaveAll";
    public EmployeeLeave(AppDbContext context, ILogger<EmployeeLeave> logger, IMemoryCache cache, Services.Services leaveService)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _leaveService = leaveService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Return List of Employees", Description = "return List of Employees")]
    [SwaggerResponse(200, "Return List of Employee leave Records")]
    [SwaggerResponse(404, "if EmployeesLeave List is Empty")]
    public async Task<ActionResult<List<ReturnEmployeeLeaveDto>>> GetAll()
    {
        _logger.LogInformation("Entered GetAll Method");
        if (_cache.TryGetValue(CacheKey, out List<ReturnEmployeeLeaveDto> list))
            return Ok(list);

        var EmpLeaveList = await _context.Employeeleaves.Where(e => e.isDeleted == false).Select(e => new ReturnEmployeeLeaveDto
        {
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            LeaveTypeName = e.LeaveTypeName,
            Duration = e.Duration
        }).ToListAsync();
        if (!EmpLeaveList.Any())
        {
            return NotFound("No Record Created");
        }
        var opt = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
        .SetSlidingExpiration(TimeSpan.FromMinutes(1));

        _cache.Set(CacheKey, EmpLeaveList, opt);
        return Ok(EmpLeaveList);
    }
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Return Employee Leave with Specified Id")]
    [SwaggerResponse(200, "Return List of Employee Leave")]
    [SwaggerResponse(404, "if Employee Leave List is Empty")]
    public async Task<ActionResult<ReturnEmployeeLeaveDto>> GetById(Guid id)
    {
        var EmpLeaveList = await _context.Employeeleaves.Where(e => e.isDeleted == false && e.Id == id).Select(e => new ReturnEmployeeLeaveDto
        {
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            LeaveTypeName = e.LeaveTypeName,
            Duration = e.Duration
        }).FirstOrDefaultAsync();
        if (EmpLeaveList != null)
        {
            return Ok(EmpLeaveList);
        }
        return NotFound("Record Not Found");
    }

    [HttpGet("GetByPCode/{_personnelcode}")]
    [SwaggerOperation(Summary = "Return All Employee Leave with PersonnelCode")]
    [SwaggerResponse(200, "Return List of Employee Leave")]
    [SwaggerResponse(404, "if Employee Leave List is Empty")]
    public async Task<ActionResult<List<ReturnEmployeeLeaveDto>>> GetByPCode(string _personnelcode)
    {
        var EmpLeaveList = await _context.Employeeleaves.Where(e => e.isDeleted == false && e.Employee.PersonnelCode == _personnelcode).Select(e => new ReturnEmployeeLeaveDto
        {
            Id = e.Id,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            LeaveTypeName = e.LeaveTypeName,
            Duration = e.Duration
        }).ToListAsync();
        if (EmpLeaveList.Any())
        {
            return Ok(EmpLeaveList);
        }
        return NotFound("Record Not Found");
    }

    [HttpPost("Create")]
    [SwaggerOperation(Summary = "Create Employee Leave Record")]
    [SwaggerResponse(201, "if Employee Leave Record Create Successfully")]

    public async Task<ActionResult<ReturnEmployeeLeaveDto>> Create(CreateEmpLeaveDto dto)
    {
        _logger.LogInformation("Entered The Create Method");
        var Emp = await _context.Employees.FirstOrDefaultAsync(e => e.PersonnelCode == dto.PersonnelCode);
        var leavetype = await _context.LeaveTypes.FirstOrDefaultAsync(e => e.Id == dto.LeaveTypeId);
        if (Emp == null || leavetype == null)
        {
            return NotFound("Employee or Leave Type Not Found");
        }
        if (dto.EndDate < dto.StartDate)
        {
            return BadRequest("زمان پایان مرخصی نمیتواند قبل از شروع مرخصی باشد");
        }
        var EmpLeaveObject = new Models.EmployeeLeave
        {
            Id = Guid.NewGuid(),
            isDeleted = false,
            EmployeeId = Emp.Id,
            LeaveTypeId = dto.LeaveTypeId,
            LeaveTypeName = leavetype.Type,
            Duration = dto.Duration,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate

        };
        var Result = new ReturnEmployeeLeaveDto
        {
            EmployeeId = Emp.Id,
            LeaveTypeId = dto.LeaveTypeId,
            LeaveTypeName = leavetype.Type,
            Duration = dto.Duration,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        _logger.LogInformation($"Object {EmpLeaveObject.Id} Created");
        await _context.Employeeleaves.AddAsync(EmpLeaveObject);
        await _context.SaveChangesAsync();
        _cache.Remove("EmployeeLeaveAll");
        return CreatedAtAction("GetById", new { id = EmpLeaveObject.Id }, Result);

    }

    [HttpPatch("Update/{id}")]
    [SwaggerOperation(Summary = "Update Employee Leave Record With Specified Id")]
    [SwaggerResponse(200, "if Employee Leave Record Update Successfully")]
    [SwaggerResponse(404, "if Employee Leave with specified Id Was Not Exists")]
    public async Task<IActionResult> Update(Guid id, UpdateEmployeeLeaveDto dto)
    {

        _logger.LogInformation("Entered The Update Method");
        var EmpLeave = await _context.Employeeleaves.FirstOrDefaultAsync(e => e.Id == id);

        if (EmpLeave == null)
        {
            return NotFound("Not Found");
        }

        if (dto.LeaveTypeId != null && dto.Duration != null && dto.StartDate != null && dto.EndDate != null && string.IsNullOrWhiteSpace(dto.LeaveTypeName))
        {
            EmpLeave.LeaveTypeId = dto.LeaveTypeId;
            EmpLeave.Duration = dto.Duration;
            EmpLeave.StartDate = dto.StartDate;
            EmpLeave.EndDate = dto.EndDate;
            EmpLeave.LeaveTypeName = dto.LeaveTypeName;

            _logger.LogInformation($"Record {id} Updated");
            await _context.SaveChangesAsync();
            _cache.Remove("EmployeeLeaveAll");
            var Result = new ReturnEmployeeLeaveDto
            {
                EmployeeId = EmpLeave.Id,
                LeaveTypeId = EmpLeave.LeaveTypeId,
                LeaveTypeName = EmpLeave.LeaveTypeName,
                Duration = EmpLeave.Duration,
                StartDate = EmpLeave.StartDate,
                EndDate = EmpLeave.EndDate
            };
            return Ok(Result);

        }
        return BadRequest();



    }

    [HttpDelete("Delete/{id}")]
    [SwaggerOperation(Summary = "Return List of Employees", Description = "return List of Employees")]
    [SwaggerResponse(204, "if Employee Leave Record Delete Successfully")]
    [SwaggerResponse(404, "if Employee Leave Record With Specified Id Was Not Exists")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var Empleave = await _context.Employeeleaves.FirstOrDefaultAsync(e => e.Id == id && e.isDeleted == false);
        if (Empleave == null)
        {
            return NotFound("Not Found!");
        }
        Empleave.isDeleted = true;
        await _context.SaveChangesAsync();
        _cache.Remove("EmployeeLeaveAll");
        return NoContent();
    }



    [HttpPost("find-leave-id")]
    public async Task<ActionResult<Guid>> FindEmployeeLeaveId(FindLeaveIdDto dto)
    {
        var id = await _leaveService.GetEmpLeaveIdAsync(dto);
        return id.HasValue ? Ok(id.Value) : NotFound("رکورد مرخصی پیدا نشد");
    }


    [HttpGet("summary/{personnelCode}")]
    [SwaggerOperation(Summary = "خلاصه مرخصی کارمند بر اساس کد پرسنلی")]
    [SwaggerResponse(200, "خلاصه مرخصی")]
    [SwaggerResponse(404, "کارمند یا مرخصی پیدا نشد")]
    public async Task<ActionResult<EmployeeLeaveSummaryDto>> GetLeaveSummary(string personnelCode)
    {
        var employee = await _context.Employees
            .Where(e => e.PersonnelCode == personnelCode && !e.isDeleted)
            .FirstOrDefaultAsync();

        if (employee == null)
            return NotFound("کارمند پیدا نشد.");

        var leaves = await _context.Employeeleaves
            .Where(l => l.Employee.PersonnelCode == personnelCode && !l.isDeleted)
            .Select(l => new
            {
                l.LeaveTypeName,
                l.Duration
            })
            .ToListAsync();

        if (!leaves.Any())
            return NotFound("هیچ مرخصی ثبت نشده.");

        var summary = leaves
            .GroupBy(l => l.LeaveTypeName)
            .Select(g => new LeaveSummaryItemDto
            {
                LeaveType = g.Key,
                Count = g.Count(),
                TotalDuration = g.Sum(x => x.Duration),
                Unit = g.Key.Contains("ساعتی") ? "ساعت" : "روز"
            })
            .OrderBy(s => s.LeaveType)
            .ToList();

        var result = new EmployeeLeaveSummaryDto
        {
            PersonnelCode = personnelCode,
            EmployeeName = employee.Name,
            Summary = summary
        };

        return Ok(result);
    }



}