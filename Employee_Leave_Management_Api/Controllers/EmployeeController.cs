using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Employee_Leave_Management_Api.Responses;
using Microsoft.AspNetCore.Mvc;


namespace Employee_Leave_Management_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveRepository _leaveRepository;

    public EmployeeController(IEmployeeRepository employeeRepository, ILeaveRepository leaveRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
        {
            return NotFound("Employee not found");
        }

        var Response = new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Department = employee.Department,
            DateJoined = employee.DateJoined
        };

        return Ok(Response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Department = dto.Department,
            DateJoined = DateTime.UtcNow
        };

        var createdEmployee = await _employeeRepository.CreateAsync(employee);

        {
            Id = createdEmployee.Id,
            FullName = createdEmployee.FullName,
            Email = createdEmployee.Email,
            Department = createdEmployee.Department,
            DateJoined = createdEmployee.DateJoined
        }
        ;

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = response.Id },
            response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        UpdateEmployeeDto dto)
    {
        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Department = dto.Department
        };

        var updated =
            await _employeeRepository.UpdateAsync(id, employee);

        if (updated is null)
            return NotFound("Employee not found");

        var response = new EmployeeResponseDto
        {
            Id = updated.Id,
            FullName = updated.FullName,
            Email = updated.Email,
            Department = updated.Department,
            DateJoined = updated.DateJoined
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await _employeeRepository.DeleteAsync(id);

        if (!deleted)
            return NotFound("Employee not found");

        return NoContent();
    }

    [HttpGet("{id}/leaves")]
    public async Task<IActionResult> GetEmployeeLeaveHistory(int id)
    {
        var leaves =
            await _leaveRepository.GetLeaveHistoryAsync(id);

        var response = leaves.Select(MapLeaveResponse);

        return Ok(response);
    }

    [HttpGet("on-leave")]
    public async Task<IActionResult> GetEmployeesCurrentlyOnLeave()
    {
        var employees =
            await _leaveRepository.GetEmployeesCurrentlyOnLeaveAsync();

        var response = employees.Select(e => new EmployeeResponseDto
        {
            Id = e.Id,
            FullName = e.FullName,
            Email = e.Email,
            Department = e.Department,
            DateJoined = e.DateJoined
        });

        return Ok(response);
    }

    private static LeaveRequestResponseDto MapLeaveResponse(
        LeaveRequest leave)
    {
        return new LeaveRequestResponseDto
        {
            Id = leave.Id,
            EmployeeId = leave.EmployeeId,
            LeaveType = leave.LeaveType,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            Status = leave.Status,
            DateCreated = leave.DateCreated,

            Approvals = leave.Approvals.Select(a =>
                new LeaveApprovalResponseDto
                {
                    ApproverId = a.ApproverId,
                    Action = a.Action,
                    Reason = a.Reason,
                    DateActed = a.DateActed
                }).ToList()
        };
    }
}
