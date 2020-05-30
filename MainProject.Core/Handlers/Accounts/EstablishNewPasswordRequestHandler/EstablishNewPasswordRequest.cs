namespace MainProject.Core.Handlers.Accounts.EstablishNewPasswordRequestHandler
{
    using Common.BaseRequestHandler;
    using FluentValidation;

    public class EstablishNewPasswordRequest : BaseRequest
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class EstablishNewPasswordRequestValidator
        : AbstractValidator<EstablishNewPasswordRequest>
    {
        public EstablishNewPasswordRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Token)
                .NotEmpty();

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
