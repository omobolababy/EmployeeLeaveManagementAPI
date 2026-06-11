using Employee_Leave_Management_Api.Data;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Employee_Leave_Management_Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_Management_Api.Services;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbcontext;
    
    public EmployeeRepository(ApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }


    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _dbcontext.Employees.Select(e => new EmployeeResponseDto
        {
            Id = e.Id,
            FullName = e.FullName,
            Email = e.Email,
            Department = e.Department,
            DateJoined = e.DateJoined
        }).ToListAsync();

        return employees;
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
      var employee = await _dbcontext.Employees.Where(e => e.Id == id).Select(e => new EmployeeResponseDto
      {
          Id = e.Id,
          FullName = e.FullName,
          Email = e.Email,
          Department = e.Department,
          DateJoined = e.DateJoined
      }).FirstOrDefaultAsync();

      return employee;
    }

    public async Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Department = dto.Department,
            DateJoined = DateTime.UtcNow
        };

        _dbcontext.Employees.Add(employee);
        
        await _dbcontext.SaveChangesAsync();

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Department = employee.Department,
            DateJoined = employee.DateJoined
        };

    }

    public async Task<EmployeeResponseDto?> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _dbcontext.Employees
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return null;

        employee.FullName = dto.FullName;
        employee.Email = dto.Email;
        employee.Department = dto.Department;

        await _dbcontext.SaveChangesAsync();

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Department = employee.Department,
            DateJoined = employee.DateJoined
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
       var employee = await _dbcontext.Employees.FirstOrDefaultAsync(x => x.Id == id);
       
       if (employee == null)
           return false;
       
       _dbcontext.Employees.Remove(employee);
       await _dbcontext.SaveChangesAsync();
       return true;
    }

    public async Task<IEnumerable<LeaveRequestResponseDto>> GetEmployeeLeaveHistoryAsync(int employeeId)
    {
        return await _dbcontext.LeaveRequests
            .Include(x => x.Approvals)
            .Where(x => x.EmployeeId == employeeId)
            .Select(x => new LeaveRequestResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                LeaveType = x.LeaveType.ToString(),
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Reason = x.Reason,
                Status = x.Status,
                DateCreated = x.DateCreated,
                Approvals = x.Approvals
                    .Select(a => new LeaveApprovalResponseDto
                    {
                        ApproverId = a.ApproverId,
                        Action = a.Action,
                        Reason = a.Reason,
                        DateActed = a.DateActed
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesCurrentlyOnLeaveAsync()
    {
        var today = DateTime.Today;

        return await _dbcontext.Employees
            .Where(e => e.LeavesRequests.Any(l =>
                l.Status == "Approved" &&
                l.StartDate <= today &&
                l.EndDate >= today))
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Department = e.Department,
                DateJoined = e.DateJoined
            })
            .ToListAsync();
    }
}