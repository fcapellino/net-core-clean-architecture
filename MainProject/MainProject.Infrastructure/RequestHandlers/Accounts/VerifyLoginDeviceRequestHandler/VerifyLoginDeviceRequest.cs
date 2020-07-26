namespace MainProject.Infrastructure.Handlers.Accounts.VerifyLoginDeviceRequestHandler
{
    using FluentValidation;
    using Common.BaseRequestHandler;

    public class VerifyLoginDeviceRequest : BaseRequest
    {
        public string VerificationCode { get; set; }
    }

    public class VerifyLoginDeviceRequestValidator
        : AbstractValidator<VerifyLoginDeviceRequest>
    {
        public VerifyLoginDeviceRequestValidator()
        {
            RuleFor(x => x.VerificationCode)
                .NotEmpty();
        }
    }
}
