namespace MainProject.Infrastructure.Handlers.Users.EditRegisteredUserRequestHandler
{
    using System;
    using System.Linq;
    using Common.BaseRequestHandler;
    using FluentValidation;
    using MainProject.Common;

    public class EditRegisteredUserRequest : BaseRequest
    {
        public string TaskId { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; }
        public string Role { get; set; }
    }

    public class EditRegisteredUserRequestValidator
        : AbstractValidator<EditRegisteredUserRequest>
    {
        public EditRegisteredUserRequestValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty()
                .Must(x => Guid.TryParse(x, out Guid result));

            RuleFor(x => x.Id)
                .NotEmpty()
                .Must(x => Guid.TryParse(x, out Guid result));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.IsDisabled)
                .NotNull();

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(x =>
                {
                    return new[] { UserRoles.Administrator, UserRoles.Regular }.Contains(x.ToLower());
                });

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
