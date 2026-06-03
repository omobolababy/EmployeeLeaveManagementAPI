using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Microsoft.AspNetCore.Mvc;


namespace Employee_Leave_Management_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
    {
        var employess = await _employeeRepository.GetAllEmployees();
        return Ok(employess);
    }
    
    [HttpGet("id")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        var employee = await _employeeRepository.GetEmployeeById(id);
        if (employee == null)
            return NotFound();
        
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeDto dto)
    {
        var employee = await _employeeRepository.CreateEmployee(dto);
        return CreatedAtAction(nameof(GetEmployeeById), 
            new {id = employee.Id},
            employee);
    }
    
    [HttpPut("id")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto dto)
    {
        var employee = await _employeeRepository.UpdateEmployee(id, dto);
        if (employee == null)
            return NotFound();
        
        return Ok(employee);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _employeeRepository.DeleteEmployee(id);
        if (!employee)
            return NotFound();
        
        return Ok();
    }
    
    [HttpGet("id/leaves")]
    public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetEmployeeLeaveHistory(int employeeId)
    {
        var leaves = await _employeeRepository.GetEmployeeLeaveHistory(employeeId);
        return Ok(leaves);
    }
}