using Employee_Leave_Management_Api.Constants;
using Employee_Leave_Management_Api.Dto;
using Employee_Leave_Management_Api.Helper;
using Employee_Leave_Management_Api.Interface;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Employee_Leave_Management_Api.Validator;

public class SubmitLeaveRequestDtoValidation : AbstractValidator<SubmitLeaveRequestDto>
{
   private readonly IEmployeeRepository _employeeRepository;
   private readonly ILeaveRepository _leaveRepository;

   public SubmitLeaveRequestDtoValidation(IEmployeeRepository employeeRepository, ILeaveRepository leaveRepository)
   {
      _employeeRepository = employeeRepository;
      _leaveRepository = leaveRepository;

      RuleFor(x => x.EmployeeId).GreaterThan(0);

      RuleFor(x => x.LeaveType)
          .NotEmpty()
          .Must(type => Enum.IsDefined(typeof(LeaveTypeContants), type))
          .WithMessage("Invalid leave type");
      
      RuleFor(x => x.StartDate).NotEmpty();
      
      RuleFor(x => x.EndDate)
          .NotEmpty()
          .GreaterThanOrEqualTo(x => x.StartDate)
          .WithMessage("End date must be on or after start date.");
      
      RuleFor(x => x.Reason)
          .NotEmpty().MaximumLength(500)
          .WithMessage("Reason must be between 5 and 500 characters");
      
   }

}