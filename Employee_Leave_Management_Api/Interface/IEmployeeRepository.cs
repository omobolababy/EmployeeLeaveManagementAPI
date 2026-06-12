using Employee_Leave_Management_Api.Model;

namespace Employee_Leave_Management_Api.Interface;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> AddAsync(Employee employee);
    Task<Employee> UpdateAsync(int id, Employee updatedemployee);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}