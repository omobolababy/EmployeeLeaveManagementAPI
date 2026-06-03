using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Model;

namespace Employee_Leave_Management_Api.Interface;

public interface ILeaveRepository
{
    Task<IEnumerable<LeaveRequest>> GetAllLeaves();
    
    Task<LeaveRequest> GetLeaveById(int id);
    
    Task<LeaveRequest> CreateLeave(CreateLeaveRequestDto dto);
    
    Task<LeaveRequest> UpdateLeave(int id, UpdateLeaveRequestDto dto);
    
    Task<bool> DeleteLeave(int id);
    
    Task<bool> ApproveLeave(int id);
    
    Task<bool> RejectLeave(int id);

    Task<IEnumerable<LeaveRequest>> GetLeaveByStatus(String status);
    
    Task<IEnumerable<Employee>> GetEmployeeCurrentlyOnLeave();

    Task<object> GetDepartmentLeaveStatistics();
}