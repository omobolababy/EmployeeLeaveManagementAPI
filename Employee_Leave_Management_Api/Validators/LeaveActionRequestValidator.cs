using Employee_Leave_Management_Api.Dto;
using FluentValidation;

namespace Employee_Leave_Management_Api.Validators;

public class LeaveActionRequestValidator : AbstractValidator<LeaveActionRequestDto>
{
    public LeaveActionRequestValidator()
    {
        RuleFor(x => x.ApproverId)
            .GreaterThan(0)
            .WithMessage("Approver Id is required.");

        RuleFor(x => x.Reason)
            .Length(5, 500)
            .NotEmpty()
            .WithMessage("Reason is required.");
    }
}