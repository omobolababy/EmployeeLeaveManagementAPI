using Employee_Leave_Management_Api.Data;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_Management_Api.Services;

public class LeaveRepository : ILeaveRepository
{

    private readonly ApplicationDbContext _dbcontext;
   private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase){"PENDING","PROCESSING", "APPROVED", "REJECTED"};
    public LeaveRepository(ApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
    {
        var leaves = await _dbcontext.LeaveRequests
            .Include(l => l.Approvals)
            .OrderByDescending(l => l.DateCreated)
            .ToListAsync();
        
        return leaves;
    }

    public async Task<LeaveRequest?> GetByIdAsync(int id)
    {
        var leave = await _dbcontext.LeaveRequests
            .Include(l => l.Approvals)
            .FirstOrDefaultAsync();
        
        return leave;
    }

    public async Task<IEnumerable<LeaveRequest>> GetEmployeeLeaveHistoryAsync(int employeeId)
    {
        var leave = await _dbcontext.LeaveRequests
            .Where(l => l.EmployeeId == employeeId)
            .Include(l => l.Approvals)
            .OrderByDescending(l => l.DateCreated)
            .ToListAsync();
        
        return leave;
    }

    public async Task<LeaveRequest> SubmitAsync(SubmitLeaveRequestDto dto)
    {
        var employeeExists = await _dbcontext.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists)
            throw new Exception("Employee does not exist.");
        
        if (dto.StartDate.Date > dto.EndDate.Date)
            throw new Exception("Start Date cannot be later than End Date.");

        var overlaps = await _dbcontext.LeaveRequests.AnyAsync(l =>
            l.EmployeeId == dto.EmployeeId &&
            !l.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) &&
            dto.StartDate.Date <= l.EndDate.Date &&
            dto.EndDate.Date >= l.StartDate.Date
        );
        
        if (overlaps)
            throw new Exception("Employee has an overlapping leave request.");

        var leave = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate.Date,
            EndDate = dto.EndDate.Date,
            Reason = dto.Reason,
            Status = "Pending",
            DateCreated = DateTime.UtcNow
        };
        
        _dbcontext.LeaveRequests.Add(leave);
        await _dbcontext.SaveChangesAsync();

        return leave;

    }

    public async Task<LeaveRequest?> UpdateAsync(int id, SubmitLeaveRequestDto dto)
    {
        var employeeExists = await _dbcontext.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
        if (!employeeExists)
            throw new Exception("Employee does not exist.");
        
        var leave = await _dbcontext
            .LeaveRequests
            .FirstOrDefaultAsync(l => l.Id == id);
        if (leave is null)
            return null;
        
        if (!leave.Status
                .Equals("Pending", StringComparison.OrdinalIgnoreCase))
            throw new Exception("Only Pending leave requests can be updated.");
        
        if (dto.StartDate.Date > dto.EndDate.Date)
            throw new Exception("Start Date cannot be later than End Date.");

        var overlaps = await _dbcontext.LeaveRequests.AnyAsync(l =>
            l.Id != id &&
            l.EmployeeId == dto.EmployeeId &&
            !l.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) &&
            dto.StartDate.Date <= l.EndDate.Date &&
            dto.EndDate.Date >= l.StartDate.Date
        );   
        
        if (overlaps)
            throw new InvalidOperationException("Employee has an overlapping leave request.");

        leave.EmployeeId = dto.EmployeeId;
        leave.LeaveType = dto.LeaveType;
        leave.StartDate = dto.StartDate.Date;
        leave.EndDate = dto.EndDate.Date;
        leave.Reason = dto.Reason;

        await _dbcontext.SaveChangesAsync();
        return leave;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var leave = await _dbcontext.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);
        if (leave is null)
            return false;

        _dbcontext.LeaveRequests.Remove(leave);
        await _dbcontext.SaveChangesAsync();
        return true;
    }

    public async Task<LeaveRequest?> ApproveAsync(int leaveId, LeaveActionRequestDto dto)
    {
        var leave = await _dbcontext.LeaveRequests
            .Include(l => l.Approvals)
            .FirstOrDefaultAsync(l => l.Id == leaveId);

        if (leave is null)
            return null;

        if (!AllowedStatuses.Contains(leave.Status))
            throw new Exception("Invalid leave status.");
        
        if (leave.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) ||
            leave.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            throw new Exception("This leave request is already finalized.");
        
        var approverExists = await _dbcontext.Employees.AnyAsync(e => e.Id == dto.ApproverId);
        if (!approverExists)
            throw new Exception("Approver does not exist.");
        
        if (dto.ApproverId == leave.EmployeeId)
            throw new Exception("You cannot approve your own leave request.");
        
        var alreadyActed = leave.Approvals.Any(a => a.ApproverId == dto.ApproverId);
        if (alreadyActed)
            throw new Exception("This approver has already acted on this leave request.");

        //  (2-step workflow)
        if (leave.Approvals.Count >= 2)
            throw new Exception("This leave request already has two actions recorded.");
        
        leave.Approvals.Add(new LeaveApproval
        {
            LeaveRequestId = leave.Id,
            ApproverId = dto.ApproverId,
            Action = "Approve",
            Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason,
            DateActed = DateTime.UtcNow
        });
        
        var approvalCount = leave.Approvals.Count(a => a.Action == "Approve");
        var rejectionCount = leave.Approvals.Count(a => a.Action == "Reject");

        if (rejectionCount > 0)
        {
            leave.Status = "Rejected";
        }
        else if (approvalCount == 1)
        {
            leave.Status = "Processing";
        }
        
        else if (approvalCount == 2)
        {
            leave.Status = "Approved";
        }

        await _dbcontext.SaveChangesAsync();
        return leave;
    }

    public async Task<LeaveRequest?> RejectAsync(int leaveId, LeaveActionRequestDto dto)
    {
        var leave = await _dbcontext.LeaveRequests
            .Include(l => l.Approvals)
            .FirstOrDefaultAsync(l => l.Id == leaveId);

        if (leave is null)
            return null;

        if (!AllowedStatuses.Contains(leave.Status))
            throw new Exception("Invalid leave status.");

        if (leave.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) ||
            leave.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            throw new Exception("This leave request is already finalized.");
        
        var approverExists = await _dbcontext.Employees.AnyAsync(e => e.Id == dto.ApproverId);
        if (!approverExists)
            throw new Exception("Approver does not exist.");

        // Rule: self-reject forbidden
        if (dto.ApproverId == leave.EmployeeId)
            throw new Exception("You cannot reject your own leave request.");

        // Rule: single action per employee per request
        var alreadyActed = leave.Approvals
            .Any(a => a.ApproverId == dto.ApproverId);
        if (alreadyActed)
            throw new Exception("This approver has already acted on this leave request.");
        
        // Rule: rejection reason required
        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new InvalidOperationException("Rejection reason is required.");

        // Rule: max 2 total actions
        if (leave.Approvals.Count >= 2)
            throw new InvalidOperationException("This leave request already has two actions recorded.");  
        
        leave.Approvals.Add(new LeaveApproval
        {
            LeaveRequestId = leave.Id,
            ApproverId = dto.ApproverId,
            Action = "Reject",
            Reason = dto.Reason,
            DateActed = DateTime.UtcNow
        });

        // Any rejection immediately finalizes
        leave.Status = "Rejected";

        await _dbcontext.SaveChangesAsync();
        return leave;
      
    }

    public async Task<IEnumerable<LeaveRequest>> GetByStatusAsync(string status)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesCurrentlyOnLeaveAsync()
    {
        var today = DateTime.UtcNow.Date;

        return await _dbcontext.Employees
            .Where(e => e.LeaveRequest.Any(l =>
                l.Status == "Approved" &&
                l.StartDate.Date <= today &&
                l.EndDate.Date >= today
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<DepartmentLeaveStatistics>> GetLeaveStatisticsByDepartmentAsync()
    {
        return await _dbcontext.Employees
            .GroupJoin(
                _dbcontext.LeaveRequests,
                e => e.Id,
                l => l.EmployeeId,
                (e, leaves) => new { e.Department, Leaves = leaves }
            )
            .GroupBy(x => x.Department)
            .Select(g => new DepartmentLeaveStatistics
            {
                Department = g.Key,
                Total = g.SelectMany(x => x.Leaves).Count(),
                Pending = g.SelectMany(x => x.Leaves).Count(l => l.Status == "Pending"),
                Processing = g.SelectMany(x => x.Leaves).Count(l => l.Status == "Processing"),
                Approved = g.SelectMany(x => x.Leaves).Count(l => l.Status == "Approved"),
                Rejected = g.SelectMany(x => x.Leaves).Count(l => l.Status == "Rejected")
            }).ToListAsync();
    }
}