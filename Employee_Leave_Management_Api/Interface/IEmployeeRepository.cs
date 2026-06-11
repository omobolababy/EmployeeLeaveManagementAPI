using Employee_Leave_Management_Api.Data;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Model;
using Employee_Leave_Management_Api.Responses;
using Employee_Leave_Management_Api.Services;

namespace Employee_Leave_Management_Api.Interface;

public interface IEmployeeRepository
{
 Task<List<EmployeeResponseDto>> GetAllAsync();

 Task<EmployeeResponseDto?> GetByIdAsync(int id);

 Task<EmployeeResponseDto> CreateAsync(
  CreateEmployeeDto dto);

 Task<EmployeeResponseDto?> UpdateAsync(
  int id,
  UpdateEmployeeDto dto);

 Task<bool> DeleteAsync(int id);

 Task<IEnumerable<LeaveRequestResponseDto>>
  GetEmployeeLeaveHistoryAsync(int employeeId);
 
 Task<IEnumerable<EmployeeResponseDto>> GetEmployeesCurrentlyOnLeaveAsync();

}