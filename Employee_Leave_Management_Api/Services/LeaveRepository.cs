using Employee_Leave_Management_Api.Data;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_Management_Api.Services;

public class LeaveRepository : ILeaveRepository 
{
    private readonly ApplicationDbContext _dbcontext;
    public LeaveRepository(ApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    
    public async Task<IEnumerable<LeaveRequest>> GetAllLeaves()
    {
        var leaves = await _dbcontext.LeaveRequests.Include(x => x.Employee).ToListAsync();
        return leaves;
    }

    public async Task<LeaveRequest> GetLeaveById(int id)
    {
        var leaves = await _dbcontext.LeaveRequests.Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == id);
        return leaves;
    }

    public async Task<LeaveRequest> CreateLeave(CreateLeaveRequestDto dto)
    {
       var employeeExists = await _dbcontext.Employees.AnyAsync(x => x.Id == dto.EmployeeId);

       if (!employeeExists)
       {
           throw new Exception("Employee does not exist");
       }
       
       if (dto.StartDate > dto.EndDate)
       {
           throw new Exception("Start date cannot be greater than end date");
       }

       var hasOverlappingLeave = await _dbcontext.LeaveRequests
           .AnyAsync(l =>
               l.EmployeeId == dto.EmployeeId &&
               l.Status != "Rejected" &&
               dto.StartDate <= l.EndDate &&
               dto.EndDate >= l.StartDate);

       if (hasOverlappingLeave)
       {
           throw new Exception("Employee already has overlapping leave");
       }

       var Leave = new LeaveRequest
       {
           EmployeeId = dto.EmployeeId,
           LeaveType = dto.LeaveType,
           StartDate = dto.StartDate,
           EndDate = dto.EndDate,
           Reason = dto.Reason,
           Status = "Pending",
           DateCreated = DateTime.Now
       };
       _dbcontext.LeaveRequests.AddAsync(Leave);
       await _dbcontext.SaveChangesAsync();
       
       return Leave;
    }

    public async Task<LeaveRequest> UpdateLeave(int id, UpdateLeaveRequestDto dto)
    {
        var leave = await _dbcontext.LeaveRequests.FindAsync(id);
        if (leave == null)
        {
            throw new Exception("Leave does not exist");
        }

        var allowedstatuses = new List<string>
        {
            "Pending",
            "Approved",
            "Rejected"
        };

        if (!allowedstatuses.Contains(dto.Status))
        {
            throw new Exception("Invalid status");
        }

        leave.LeaveType = dto.LeaveType;
        leave.StartDate = dto.StartDate;
        leave.EndDate = dto.EndDate;
        leave.Reason = dto.Reason;
        leave.Status = dto.Status;

        _dbcontext.LeaveRequests.Update(leave);
        await _dbcontext.SaveChangesAsync();
        
        return leave;
    }

    public async Task<bool> DeleteLeave(int id)
    {
        var leave = await _dbcontext.LeaveRequests.FindAsync(id);
        if (leave == null)
        {
            return false;
        }
        
        _dbcontext.LeaveRequests.Remove(leave);
        await _dbcontext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ApproveLeave(int id)
    {
        var leave = await _dbcontext.LeaveRequests.FindAsync(id);
        if (leave == null)
        {
            return false;
        }
        
        leave.Status = "Approved";
        _dbcontext.LeaveRequests.Update(leave);
        await _dbcontext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> RejectLeave(int id)
    {
        var leave = await _dbcontext.LeaveRequests.FindAsync(id);
        if (leave == null)
        {
            return false;
        }
        
        leave.Status = "Rejected";
        _dbcontext.LeaveRequests.Update(leave);
        await _dbcontext.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<LeaveRequest>> GetLeaveByStatus(string status)
    {
        var leaves = await _dbcontext.LeaveRequests.Where(x => x.Status == status).ToListAsync();
        return leaves;
    }

    public async Task<IEnumerable<Employee>> GetEmployeeCurrentlyOnLeave()
    {
        var today = DateTime.Today;

        return await _dbcontext.Employees
            .Include(e => e.LeavesRequests)
            .Where(e => e.LeavesRequests.Any(l =>
                l.Status == "Approved" &&
                l.StartDate <= today &&
                l.EndDate >= today))
            .ToListAsync();
    }

    public async Task<object> GetDepartmentLeaveStatistics()
    {
        var statistics = await _dbcontext.LeaveRequests.Include(l => l.Employee).GroupBy(l => l.Employee.Department)
            .Select(g => new
            {
                Department = g.Key,
                TotalLeaves = g.Count(),
                Approved = g.Count(x => x.Status == "Approved"),
                Rejected = g.Count(x => x.Status == "Rejected"),
                Pending = g.Count(x => x.Status == "Pending")
            }).ToListAsync();
        return statistics;
    }
}