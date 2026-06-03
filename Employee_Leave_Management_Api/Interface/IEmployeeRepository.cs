using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Model;

namespace Employee_Leave_Management_Api.Interface;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployees();
    
    Task<Employee> GetEmployeeById(int id);
    
    Task<Employee> CreateEmployee(CreateEmployeeDto dto);
    
    Task<Employee> UpdateEmployee(int id, UpdateEmployeeDto dto);
    
    Task<bool> DeleteEmployee(int id);
    
    Task<IEnumerable<LeaveRequest>> GetEmployeeLeaveHistory(int employeeId);
}