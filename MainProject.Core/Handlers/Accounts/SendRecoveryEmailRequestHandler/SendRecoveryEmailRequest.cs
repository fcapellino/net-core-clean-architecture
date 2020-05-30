namespace MainProject.Core.Handlers.Accounts.SendRecoveryEmailRequestHandler
{
    using Common.BaseRequestHandler;
    using FluentValidation;

    public class SendRecoveryEmailRequest : BaseRequest
    {
        public string Email { get; set; }
    }

    public class SendRecoveryEmailRequestValidator
        : AbstractValidator<SendRecoveryEmailRequest>
    {
        public SendRecoveryEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
