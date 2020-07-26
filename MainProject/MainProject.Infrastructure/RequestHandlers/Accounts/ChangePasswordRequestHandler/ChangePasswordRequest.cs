namespace MainProject.Infrastructure.Handlers.Accounts.ChangePasswordRequestHandler
{
    using Common.BaseRequestHandler;
    using FluentValidation;

    public class ChangePasswordRequest : BaseRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordRequestValidator
        : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotNull()
                .MinimumLength(8);

            RuleFor(x => x.NewPassword)
                .NotNull()
                .MinimumLength(8);
        }
    }
}
