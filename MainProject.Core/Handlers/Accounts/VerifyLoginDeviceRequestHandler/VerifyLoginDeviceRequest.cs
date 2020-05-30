namespace MainProject.Core.Handlers.Accounts.VerifyLoginDeviceRequestHandler
{
    using FluentValidation;
    using MainProject.Core.Common.BaseRequestHandler;

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
