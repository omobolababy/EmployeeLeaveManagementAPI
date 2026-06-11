using System.Collections;
using Employee_Leave_Management_Api.Constants;
using Employee_Leave_Management_Api.Data;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Helper;
using Employee_Leave_Management_Api.Interface;
using Employee_Leave_Management_Api.Model;
using Employee_Leave_Management_Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_Management_Api.Services;

public class LeaveRepository : ILeaveRepository
{
    
    private readonly ApplicationDbContext _dbcontext;
    public LeaveRepository(ApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    
    public async Task<IEnumerable<LeaveRequestResponseDto>> GetAllAsync()
    {
        var leaves = await _dbcontext.LeaveRequests
            .Include(x => x.Approvals)
            .ToListAsync();

        return leaves.Select(MapToResponseDto);
    }

    public async Task<LeaveRequestResponseDto?> GetByIdAsync(int id)
    {
        var leave = await _dbcontext.LeaveRequests
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == id);

        return leave == null ? null : MapToResponseDto(leave);
    }

    public async Task<LeaveRequestResponseDto> CreateAsync(SubmitLeaveRequestDto dto)
    {
        var employee = await _dbcontext.Employees
            .FirstOrDefaultAsync(x => x.Id == dto.EmployeeId);

        if (employee == null)
            throw new Exception("Employee does not exist.");

        var hasOverlap = await _dbcontext.LeaveRequests
            .AnyAsync(l =>
                l.EmployeeId == dto.EmployeeId &&
                l.StartDate <= dto.EndDate &&
                l.EndDate >= dto.StartDate);

        if (hasOverlap)
            throw new Exception("Leave overlaps with existing leave.");
        
        var leave = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatuses.Pending,
            DateCreated = DateTime.DaysInMonth(dto.StartDate.Year, dto.StartDate.Month) == dto.StartDate.Day
                ? dto.StartDate
                : dto.StartDate.AddDays(-1)
        };
        
        _dbcontext.LeaveRequests.Add(leave);
        await _dbcontext.SaveChangesAsync();

        return MapToResponseDto(leave);

    }

    public async Task<LeaveRequestResponseDto?> UpdateAsync(int id, SubmitLeaveRequestDto dto)
    {
        var leave = await _dbcontext.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id);

        if (leave == null)
            return null;

        if (leave.Status != LeaveStatuses.Pending)
            throw new Exception(
                "Only pending requests can be modified.");
        
         leave.LeaveType =
                        Enum.Parse<LeaveType>(dto.LeaveType, true);
        
                    leave.StartDate = dto.StartDate;
                    leave.EndDate = dto.EndDate;
                    leave.Reason = dto.Reason;
        
                    await _dbcontext.SaveChangesAsync();
        
                    return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
         var leave = await _dbcontext.LeaveRequests
            .FirstOrDefaultAsync(x => x.Id == id);

        if (leave == null)
            return false;

        _dbcontext.LeaveRequests.Remove(leave);

        await _dbcontext.SaveChangesAsync();

        return true;
    }

    public async Task<LeaveRequestResponseDto> ApproveAsync(int leaveId, LeaveActionRequestDto dto)
    {
        var leave = await _dbcontext.LeaveRequests
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == leaveId);

        if (leave == null)
            throw new Exception("Leave request not found.");

        if (leave.Status == LeaveStatuses.Approved)
            throw new Exception("Already approved.");

        if (leave.Status == LeaveStatuses.Rejected)
            throw new Exception("Already rejected.");

        if (leave.EmployeeId == dto.ApproverId)
            throw new Exception(
                "Self approval is not allowed.");
        
         var alreadyActed = leave.Approvals
                        .Any(a => a.ApproverId == dto.ApproverId);
        
                    if (alreadyActed)
                        throw new Exception(
                            "Approver has already acted.");
        
                    var approval = new LeaveApproval
                    {
                        LeaveRequestId = leave.Id,
                        ApproverId = dto.ApproverId,
                        Action = "Approve",
                        Reason = dto.Reason,
                        DateActed = DateTime.UtcNow
                    };

                     _dbcontext.LeaveApproval.Add(approval);
                    
                                var approvalCount =
                                    leave.Approvals.Count(a => a.Action == "Approve");
                    
                                if (approvalCount == 0)
                                {
                                    leave.Status = LeaveStatuses.Processing;
                                }
                                else if (approvalCount == 1)
                                {
                                    leave.Status = LeaveStatuses.Approved;
                                }
                                
                                await _dbcontext.SaveChangesAsync();
                                
                                return await GetByIdAsync(leaveId);
    }

    public async Task<LeaveRequestResponseDto> RejectAsync(int leaveId, LeaveActionRequestDto dto)
    {
       var leave = await _dbcontext.LeaveRequests
                       .Include(x => x.Approvals)
                       .FirstOrDefaultAsync(x => x.Id == leaveId);
       
                   if (leave == null)
                       throw new Exception("Leave request not found.");
       
                   if (leave.EmployeeId == dto.ApproverId)
                       throw new Exception(
                           "Self rejection is not allowed.");
                   
                   if (string.IsNullOrWhiteSpace(dto.Reason))
                       throw new Exception(
                           "Reason is required for rejection.");

                   var alreadyActed = leave.Approvals
                       .Any(x => x.ApproverId == dto.ApproverId);

                   if (alreadyActed)
                       throw new Exception(
                           "Approver already acted.");
                   
                   var rejection = new LeaveApproval
                   {
                       LeaveRequestId = leave.Id,
                       ApproverId = dto.ApproverId,
                       Action = "Reject",
                       Reason = dto.Reason,
                       DateActed = DateTime.UtcNow
                   };

                   _dbcontext.LeaveApproval.Add(rejection);

                   leave.Status = LeaveStatuses.Rejected;

                   await _dbcontext.SaveChangesAsync();
                   return await GetByIdAsync(leaveId);
    }

    public async Task<IEnumerable<LeaveRequestResponseDto>> GetLeaveByStatusAsync(string status)
    {
        var leave = await _dbcontext.LeaveRequests
            .Include(x => x.Approvals)
            .Where(x => x.Status == status).ToListAsync();
        return MapToResponse(leave);
    }

    public async Task<IEnumerable> GetLeaveStatisticsAsync()
    {
        return await _dbcontext.LeaveRequests
                        .Include(x => x.Employee)
                        .GroupBy(x => x.Employee.Department)
                        .Select(g => new
                        {
                            Department = g.Key,
                            TotalLeaves = g.Count(),
                            Approved = g.Count(x =>
                                x.Status == LeaveStatuses.Approved),
                            Rejected = g.Count(x =>
                                x.Status == LeaveStatuses.Rejected),
                            Pending = g.Count(x =>
                                x.Status == LeaveStatuses.Pending),
                            Processing = g.Count(x =>
                                x.Status == LeaveStatuses.Processing)
                        })
                        .ToListAsync();

    }
    
    private static LeaveRequestResponseDto MapToResponseDto(LeaveRequest leave)
    {
        return new LeaveRequestResponseDto
        {
            Id = leave.Id,
            EmployeeId = leave.EmployeeId,
            LeaveType = leave.LeaveType.ToString(),
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            Status = leave.Status,
            DateCreated = leave.DateCreated,

            Approvals = leave.Approvals.Select(a => new LeaveApprovalResponseDto
            {
                ApproverId = a.ApproverId,
                Action = a.Action,
                Reason = a.Reason,
                DateActed = a.DateActed
            }).ToList()
        };
    }
}