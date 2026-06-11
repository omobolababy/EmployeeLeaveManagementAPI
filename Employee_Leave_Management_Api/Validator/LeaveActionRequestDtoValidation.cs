using Employee_Leave_Management_Api.Dto;
using FluentValidation;

namespace Employee_Leave_Management_Api.Validator;

public class LeaveActionRequestDtoValidation : AbstractValidator<LeaveActionRequestDto>
{
    public LeaveActionRequestDtoValidation()
    {
        RuleFor(x => x.ApproverId).GreaterThan(0)
            .WithMessage("Approver Id is required");
        
        RuleFor(x => x.Reason).NotEmpty()
            .Length(5, 250)
            .WithMessage("Reason is required and must be between 5 and 250 characters");  
        
    }
}