namespace Employee_Leave_Management_Api.Model;

public class DepartmentLeaveStatistics
{
    public string Department { get; set; } 
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Processing { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
}