using Employee_Leave_Management_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_Management_Api.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    
    public DbSet<LeaveApproval> LeaveApprovals { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.LeaveRequest)
            .WithOne(l => l.Employee)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeaveRequest>()
            .HasMany(l => l.Approvals)
            .WithOne(a => a.LeaveRequest)
            .HasForeignKey(a => a.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeaveApproval>()
            .HasOne(a => a.Approver)
            .WithMany()
            .HasForeignKey(a => a.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}