using System.Reflection.Emit;
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
public class LeaveType : ControllerBase
{
    private readonly ILogger<LeaveType> _logger;
    private readonly AppDbContext _context;
    public LeaveType(AppDbContext context,ILogger<LeaveType> logger)
    {
        _logger = logger;
        _context = context;
    }
    [HttpGet]
    [SwaggerOperation(Summary = "Return List of Employees", Description = "return List of Employees")]
    [SwaggerResponse(200, "Return List of Leave Types")]
    [SwaggerResponse(404, "if Leave Types List is Empty")]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Entered GetAll Method");
        var LeaveTypeList = await _context.LeaveTypes.ToListAsync();
        if (LeaveTypeList != null)
        {
            _logger.LogInformation("Returned LeaveType List");

            return Ok(LeaveTypeList);
        }
        _logger.LogInformation("Empty LeaveType List");
        return NotFound("Empty");
    }

    [HttpPost("Create")]
    [SwaggerOperation(Summary = "Create New Leave Type")]
    [SwaggerResponse(409, "if Leave Type Was Exists")]
    [SwaggerResponse(201,"if Leave Type Created Successfully")]
    public async Task<IActionResult> Create(CreateLeaveTypeDto dto)
    {
        if (await _context.LeaveTypes.AnyAsync(e => e.Type == dto.Type) || dto == null)
        {
            return Conflict("Leave Type Was Exists");
                          
        }
        else
        {
            var newLeaveType = new Models.LeaveType
            {
                Type = dto.Type
            };
            await _context.LeaveTypes.AddAsync(newLeaveType);
            return CreatedAtAction("GetAll", dto);
        }
    }


}