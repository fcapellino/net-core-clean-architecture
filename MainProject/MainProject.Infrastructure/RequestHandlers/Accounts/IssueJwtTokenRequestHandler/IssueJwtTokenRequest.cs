namespace MainProject.Infrastructure.Handlers.Accounts.IssueJwtTokenRequestHandler
{
    using Common.BaseRequestHandler;
    using FluentValidation;

    public class IssueJwtTokenRequest : BaseRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class IssueJwtTokenRequestValidator
        : AbstractValidator<IssueJwtTokenRequest>
    {
        public IssueJwtTokenRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
