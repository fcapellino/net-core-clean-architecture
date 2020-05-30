namespace MainProject.Core.Handlers.Accounts.UserLoginRequestHandler
{
    using Common.BaseRequestHandler;
    using FluentValidation;

    public class UserLoginRequest : BaseRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginRequestValidator
        : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
