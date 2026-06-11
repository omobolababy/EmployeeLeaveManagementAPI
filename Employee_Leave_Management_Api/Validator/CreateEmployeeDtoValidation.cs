using Employee_Leave_Management_Api.Constants;
using Employee_Leave_Management_Api.Dto;
using FluentValidation;
namespace Employee_Leave_Management_Api.Validator;

public class CreateEmployeeDtoValidation : AbstractValidator<CreateEmployeeDto>
{

    public CreateEmployeeDtoValidation()
    {
    
        RuleFor(x => x.FullName)
            .NotEmpty().MaximumLength(100)
            .WithMessage("Full Name is required and must be less than 100 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress()
            .WithMessage("Email is required");
       
       RuleFor(x => x.Department)
           .NotEmpty().WithMessage("Department is required")
           .Must(department => DepartmentConstants.Departments.Contains(department))
           .WithMessage("invalid department selected");
    }
}