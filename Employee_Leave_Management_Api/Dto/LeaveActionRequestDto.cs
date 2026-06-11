namespace Employee_Leave_Management_Api.Dto;

public class LeaveActionRequestDto
{
    public int ApproverId { get; set; }
    public string Reason { get; set; }
}