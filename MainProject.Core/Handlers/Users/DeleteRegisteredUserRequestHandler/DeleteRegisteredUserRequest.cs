namespace MainProject.Core.Handlers.Users.DeleteRegisteredUserRequestHandler
{
    using System;
    using Common.BaseRequestHandler;
    using FluentValidation;

    public class DeleteRegisteredUserRequest : BaseRequest
    {
        public string Id { get; set; }
    }

    public class DeleteRegisteredUserRequestValidator
        : AbstractValidator<DeleteRegisteredUserRequest>
    {
        public DeleteRegisteredUserRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(x =>
                {
                    return Guid.TryParse(x, out Guid result);
                });
        }
    }
}
