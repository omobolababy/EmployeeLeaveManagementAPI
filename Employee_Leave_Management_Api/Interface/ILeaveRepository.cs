using System.Collections;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Model;
using Employee_Leave_Management_Api.Responses;

namespace Employee_Leave_Management_Api.Interface;

public interface ILeaveRepository
{
    Task<IEnumerable<LeaveRequestResponseDto>> GetAllAsync();

    Task<LeaveRequestResponseDto?> GetByIdAsync(int id);

    Task<LeaveRequestResponseDto> CreateAsync(SubmitLeaveRequestDto dto);

    Task<LeaveRequestResponseDto?> UpdateAsync(int id, SubmitLeaveRequestDto dto);

    Task<bool> DeleteAsync(int id);

    Task<LeaveRequestResponseDto> ApproveAsync(int leaveId, LeaveActionRequestDto dto);

    Task<LeaveRequestResponseDto> RejectAsync(int leaveId, LeaveActionRequestDto dto);

    Task<IEnumerable<LeaveRequestResponseDto>> GetLeaveByStatusAsync(string status);

    Task<IEnumerable> GetLeaveStatisticsAsync();
}