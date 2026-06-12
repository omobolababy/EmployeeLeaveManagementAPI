namespace Employee_Leave_Management_Api.ResponseDto;

public class LeaveRequestResponseDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string LeaveType { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; }

    public ICollection<LeaveApprovalResponseDto> Approvals { get; set; }
        = new List<LeaveApprovalResponseDto>();
}