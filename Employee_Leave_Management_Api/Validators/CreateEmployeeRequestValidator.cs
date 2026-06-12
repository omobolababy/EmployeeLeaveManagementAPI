using Employee_Leave_Management_Api.Constants;
using Employee_Leave_Management_Api.Dto;
using FluentValidation;

namespace Employee_Leave_Management_Api.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequestDto>

{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Department)
            .NotEmpty()
            .WithMessage("Department is required.")
            .Must(d => DepartmentConstants.Departments.Contains(d))
            .WithMessage("Invalid department selected.");

    }
}