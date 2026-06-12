using Employee_Leave_Management_Api.Data;
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


    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        var employee = await _dbcontext.Employees.OrderByDescending(e => e.DateJoined).ToListAsync();
        return employee;
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        var employee = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        return employee;
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        _dbcontext.Employees.Add(employee);
        await _dbcontext.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(int id, Employee updatedemployee)
    {
        var existing = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (existing is null) return null;

        existing.FullName = updatedemployee.FullName;
        existing.Email = updatedemployee.Email;
        existing.Department = updatedemployee.Department;

        await _dbcontext.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (existing is null)
        {
            return false;
        }

        _dbcontext.Employees.Remove(existing);
        await _dbcontext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var employee = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        return employee != null;
    }
}