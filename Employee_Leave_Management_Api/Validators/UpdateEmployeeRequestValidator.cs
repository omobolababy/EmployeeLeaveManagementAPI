using Employee_Leave_Management_Api.Constants;
using Employee_Leave_Management_Api.Dto;
using FluentValidation;

namespace Employee_Leave_Management_Api.Validators;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequestDto>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Department)
            .NotEmpty()
            .Must(d => DepartmentConstants.Departments.Contains(d))
            .WithMessage("Invalid department selected.");
    }
}