namespace Employee_Leave_Management_Api.Model;

public class LeaveApproval
{
    public int Id { get; set; }

    public int LeaveRequestId { get; set; }

    public int ApproverId { get; set; }

    public string Action { get; set; } 

    public string? Reason { get; set; }

    public DateTime DateActed { get; set; }
        = DateTime.UtcNow;

    public LeaveRequest LeaveRequest { get; set; } 

    public Employee Approver { get; set; } 
}