using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Model;

namespace Employee_Leave_Management_Api.Interface;

public interface ILeaveRepository
{
    Task<IEnumerable<LeaveRequest>> GetAllAsync();
    Task<LeaveRequest?> GetByIdAsync(int id);

    Task<IEnumerable<LeaveRequest>> GetEmployeeLeaveHistoryAsync(int employeeId);

    Task<LeaveRequest> SubmitAsync(SubmitLeaveRequestDto dto);
    Task<LeaveRequest?> UpdateAsync(int id, SubmitLeaveRequestDto dto);
    Task<bool> DeleteAsync(int id);

    Task<LeaveRequest?> ApproveAsync(int leaveId, LeaveActionRequestDto dto);
    Task<LeaveRequest?> RejectAsync(int leaveId, LeaveActionRequestDto dto);

    Task<IEnumerable<LeaveRequest>> GetByStatusAsync(string status);
    Task<IEnumerable<Employee>> GetEmployeesCurrentlyOnLeaveAsync();
    Task<IEnumerable<DepartmentLeaveStatistics>> GetLeaveStatisticsByDepartmentAsync();
}