using Employee_Leave_Management_Api.Constants;
using Employee_Leave_Management_Api.Dto;
using FluentValidation;

namespace Employee_Leave_Management_Api.Validator;

public class UpdateEmployeeValidation : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeValidation()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full Name is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");
        
        RuleFor(x => x.Department)
            .NotEmpty()
            .WithMessage("Department is required")
            .Must(department => DepartmentConstants.Departments.Contains(department));
    }
}