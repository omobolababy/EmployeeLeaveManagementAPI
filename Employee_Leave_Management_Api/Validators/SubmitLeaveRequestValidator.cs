using Employee_Leave_Management_Api.Constants;
using Employee_Leave_Management_Api.Dto;
using FluentValidation;

namespace Employee_Leave_Management_Api.Validators;

public class SubmitLeaveRequestValidator : AbstractValidator<SubmitLeaveRequestDto>
{
    public SubmitLeaveRequestValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("Employee Id is required.");

        RuleFor(x => x.LeaveType)
            .NotEmpty()
            .Must(x => LeaveTypeConstants.LeaveTypes.Contains(x))
            .WithMessage("Invalid leave type.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.");
        
     RuleFor(x => x.StartDate)
         .NotEmpty()
         .GreaterThan(DateTime.Now); 
     
     RuleFor(x => x.EndDate)
         .NotEmpty()
         .GreaterThan(x => x.StartDate);
    }
}