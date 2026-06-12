namespace Employee_Leave_Management_Api.Dto;

public class SubmitLeaveRequestDto
{
    public int EmployeeId { get; set; }

    public string LeaveType { get; set; } 
    
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } 
}