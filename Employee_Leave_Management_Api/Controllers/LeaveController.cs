using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Leave_Management_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveController : ControllerBase
{
    private readonly ILeaveRepository _leaveRepository;
    public LeaveController(ILeaveRepository leaveRepository)
    {
        _leaveRepository = leaveRepository;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAllLeaves()
    {
        var leaves = await _leaveRepository.GetAllLeaves();
        return Ok(leaves);
    }
    
    [HttpGet("id")]
    public async Task<ActionResult> GetLeaveById(int id)
    {
        var leave = await _leaveRepository.GetLeaveById(id);
        if (leave == null)
            return NotFound();
        
        return Ok(leave);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLeave(CreateLeaveRequestDto dto)
    {
        var leave = await _leaveRepository.CreateLeave(dto);
        return CreatedAtAction(nameof(GetLeaveById), 
            new {id = leave.Id},
            leave);
    }

    [HttpPut("id")]
    public async Task<IActionResult> UpdateLeave(int id, UpdateLeaveRequestDto dto)
    {
        var leave = await _leaveRepository.UpdateLeave(id, dto);
        if (leave == null)
            return NotFound();
        
        return Ok(leave);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        var leave = await _leaveRepository.DeleteLeave(id);
        if (!leave)
            return NotFound();
        
        return Ok("Deleted Successfully");
    }
    
    [HttpPut("id/approve")]
    public async Task<IActionResult> ApproveLeave(int id)
    {
        var leave = await _leaveRepository.ApproveLeave(id);
        if (!leave)
            return NotFound();
        
        return Ok("Leave Approved");
    }

    [HttpPut("id/reject")]
    public async Task<IActionResult> RejectLeave(int id)
    {
        var leave = await _leaveRepository.RejectLeave(id);
        if (!leave)
            return NotFound();
        
        return Ok("Leave Rejected");
    }
    
    [HttpGet("status/{status}")]
    public async Task<ActionResult> GetLeaveByStatus(string status)
    {
        var leaves = await _leaveRepository.GetLeaveByStatus(status);
        return Ok(leaves);
    }
    
    [HttpGet("employess-on-leave")]
    public async Task<ActionResult> GetEmployeeCurrentlyOnLeave()
    {
        var leaves = await _leaveRepository.GetEmployeeCurrentlyOnLeave();
        return Ok(leaves);
    }

    [HttpGet("deapartment-leave-statistics")]
    public async Task<IActionResult> GetDeapartmentStatistics()
    {
        var statistics = await _leaveRepository.GetDepartmentLeaveStatistics();
        return Ok(statistics);
    }
}