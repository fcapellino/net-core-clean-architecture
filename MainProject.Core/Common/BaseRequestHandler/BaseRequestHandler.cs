namespace MainProject.Core.Common.BaseRequestHandler
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using ElmahCore;
    using FluentValidation;
    using MainProject.Common;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using ResponseTypes;

    public abstract class BaseRequestHandler<TRequest>
        : IRequestHandler<TRequest, IResponse>, IDisposable where TRequest : IRequest<IResponse>
    {
        protected readonly HttpContext _httpContext;
        protected readonly IValidator<TRequest> _validator;

        protected BaseRequestHandler(IServiceProvider provider)
        {
            _httpContext = provider.GetService<IHttpContextAccessor>().HttpContext;
            _validator = provider.GetService<IValidator<TRequest>>();
        }

        public async Task<IResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                if (_validator != null)
                {
                    _validator.CascadeMode = CascadeMode.StopOnFirstFailure;
                    var validationResult = _validator?.Validate(request);

                    if (validationResult != null && !validationResult.IsValid)
                        throw new CustomException("El modelo de datos recibido es inválido.");
                }

                using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromHours(1), TransactionScopeAsyncFlowOption.Enabled))
                {
                    var response = await ExecuteAsync(request);
                    transactionScope.Complete();
                    return response;
                }
            }
            catch (Exception ex)
            {
                var exception = ex.GetBaseException();
                var errorMessage = (exception is CustomException)
                        ? $"Error. {exception.Message}"
                        : $"Error. No es posible completar la operación.";

                _httpContext.RiseError(exception);
                return new ErrorResponse(errorMessage, exception.GetBaseException());
            }
        }

        protected abstract Task<SuccessResponse> ExecuteAsync(TRequest request);

        public abstract void Dispose();
    }
}
