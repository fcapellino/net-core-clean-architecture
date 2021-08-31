namespace MainProject.Infrastructure.Handlers.Users.DeleteRegisteredUserRequestHandler
{
    using System;
    using Common.BaseRequestHandler;
    using FluentValidation;

    public class DeleteRegisteredUserRequest : BaseRequest
    {
        public string TaskId { get; set; }
        public string Id { get; set; }
    }

    public class DeleteRegisteredUserRequestValidator
        : AbstractValidator<DeleteRegisteredUserRequest>
    {
        public DeleteRegisteredUserRequestValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty()
                .Must(x => Guid.TryParse(x, out Guid result));

            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(x => Guid.TryParse(x, out Guid result));
        }
    }
}
