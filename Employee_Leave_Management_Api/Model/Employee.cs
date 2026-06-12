namespace Employee_Leave_Management_Api.Model;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public DateTime DateJoined { get; set; }
    
    public ICollection<LeaveRequest> LeaveRequest { get; set; } = new List<LeaveRequest>();
}