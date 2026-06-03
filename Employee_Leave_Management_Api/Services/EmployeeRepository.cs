using Employee_Leave_Management_Api.Data;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_Management_Api.Services;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbcontext;
    
    public EmployeeRepository(ApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {
        var employee = await _dbcontext.Employees.ToListAsync();
        return employee;
    }

    public async Task<Employee> GetEmployeeById(int id)
    {
        var employee = await _dbcontext.Employees.Include(x => x.LeavesRequests).FirstOrDefaultAsync(x => x.Id == id);
        return employee;
    }

    public async Task<Employee> CreateEmployee(CreateEmployeeDto dto)
    {
        var employeeExists = await _dbcontext.Employees.AnyAsync(x => x.Email == dto.Email);
        if (employeeExists)
        {
            throw new Exception("Employee already exists");
        }
        
        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Department = dto.Department,
            DateJoined = dto.DateJoined
        };
        _dbcontext.Employees.AddAsync(employee);
        await _dbcontext.SaveChangesAsync();
        
        return employee;
    }

    public async Task<Employee> UpdateEmployee(int id, UpdateEmployeeDto dto)
    {
        var employee = await _dbcontext.Employees.FindAsync(id);
        if (employee == null) 
            return null;
        
        employee.FullName = dto.FullName;
        employee.Email = dto.Email;
        employee.Department = dto.Department;
        
        _dbcontext.Employees.Update(employee);
        await _dbcontext.SaveChangesAsync();
        
        return employee;
    }

    public async Task<bool> DeleteEmployee(int id)
    {
        var employee = await _dbcontext.Employees.FindAsync(id);
        if (employee == null) 
            return false;
        
        _dbcontext.Employees.Remove(employee);
        await _dbcontext.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<LeaveRequest>> GetEmployeeLeaveHistory(int employeeId)
    {
        var employee = await _dbcontext.LeaveRequests.Where(x => x.EmployeeId == employeeId).ToListAsync();
        return employee;
    }
}