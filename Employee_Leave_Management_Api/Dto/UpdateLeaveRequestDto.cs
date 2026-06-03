namespace Employee_Leave_Management_Api.Dto;

public class UpdateLeaveRequestDto
{
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; }
}