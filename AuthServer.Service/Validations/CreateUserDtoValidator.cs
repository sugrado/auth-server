using AuthServer.Core.Dtos;
using FluentValidation;

namespace AuthServer.API.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(p => p.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(p => p.Email).EmailAddress().WithMessage("Email format is invalid.");

            RuleFor(p => p.Password).NotEmpty().WithMessage("Password is required.");

            RuleFor(p => p.UserName).NotEmpty().WithMessage("UserName is required.");
        }
    }
}
