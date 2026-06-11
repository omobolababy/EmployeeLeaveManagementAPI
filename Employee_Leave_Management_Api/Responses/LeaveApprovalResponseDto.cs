namespace Employee_Leave_Management_Api.Responses;

public class LeaveApprovalResponseDto
{
    public int ApproverId { get; set; }
    
    public string Action { get; set; }
    
    public string Reason { get; set; }
    
    public DateTime DateActed { get; set; }
}