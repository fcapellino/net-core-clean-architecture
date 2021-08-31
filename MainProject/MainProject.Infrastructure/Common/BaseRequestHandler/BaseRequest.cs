namespace MainProject.Infrastructure.Common.BaseRequestHandler
{
    using MediatR;
    using ResponseTypes;

    public abstract class BaseRequest : IRequest<IResponse> { }
}
