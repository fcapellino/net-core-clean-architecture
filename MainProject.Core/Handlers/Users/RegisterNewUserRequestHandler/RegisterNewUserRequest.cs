namespace MainProject.Core.Handlers.Users.RegisterNewUserRequestHandler
{
    using System.Linq;
    using Common.BaseRequestHandler;
    using FluentValidation;
    using MainProject.Common;

    public class RegisterNewUserRequest : BaseRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; }
        public string Role { get; set; }
    }

    public class RegisterNewUserRequestValidator
        : AbstractValidator<RegisterNewUserRequest>
    {
        public RegisterNewUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.IsDisabled)
                .NotNull();

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(x =>
                {
                    return new[] { UserRoles.Administrador, UserRoles.Regular }.Contains(x.ToLower());
                });
        }
    }
}
