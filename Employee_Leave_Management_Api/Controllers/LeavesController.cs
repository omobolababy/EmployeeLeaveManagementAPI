using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.ResponseDto;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Leave_Management_Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class LeavesController : ControllerBase
{
    private readonly ILeaveRepository _leaveRepository;

    public LeavesController(ILeaveRepository leaveRepository)
    {
        _leaveRepository = leaveRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllLeaves()
    {
        var leaves = await _leaveRepository.GetAllAsync();

        return Ok(leaves.Select(MapLeaveResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLeaveById(int id)
    {
        var leave = await _leaveRepository.GetByIdAsync(id);

        if (leave is null)
            return NotFound("Leave request not found");

        return Ok(MapLeaveResponse(leave));
    }
    
    [HttpPost]
    public async Task<IActionResult> SubmitLeaveRequest(
        SubmitLeaveRequestDto dto)
    {
        try
        {
            var leave =
                await _leaveRepository.SubmitAsync(dto);

            return CreatedAtAction(
                nameof(GetLeaveById),
                new { id = leave.Id },
                MapLeaveResponse(leave));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeaveRequest(
        int id,
        SubmitLeaveRequestDto dto)
    {
        try
        {
            var leave =
                await _leaveRepository.UpdateAsync(id, dto);

            if (leave is null)
                return NotFound();

            return Ok(MapLeaveResponse(leave));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeaveRequest(int id)
    {
        var deleted =
            await _leaveRepository.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
    
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveLeave(
        int id,
        LeaveActionRequestDto dto)
    {
        try
        {
            var leave =
                await _leaveRepository.ApproveAsync(id, dto);

            if (leave is null)
                return NotFound();

            return Ok(MapLeaveResponse(leave));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectLeave(
        int id,
        LeaveActionRequestDto dto)
    {
        try
        {
            var leave =
                await _leaveRepository.RejectAsync(id, dto);

            if (leave is null)
                return NotFound();

            return Ok(MapLeaveResponse(leave));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var leaves =
            await _leaveRepository.GetByStatusAsync(status);

        return Ok(leaves.Select(MapLeaveResponse));
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var statistics =
            await _leaveRepository.GetLeaveStatisticsByDepartmentAsync();

        return Ok(statistics);
    }

    private static LeaveRequestResponseDto MapLeaveResponse(
        Model.LeaveRequest leave)
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