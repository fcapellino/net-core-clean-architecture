namespace MainProject.Core.Handlers.Users.GetUsersRoleListRequestHandler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using CustomResults;
    using Domain.Models;
    using MainProject.Context.Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class GetUsersRoleListRequestHandler : BaseRequestHandler<GetUsersRoleListRequest>
    {
        private readonly IDbContext _dbContext;

        public GetUsersRoleListRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _dbContext = provider.GetService<IDbContext>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(GetUsersRoleListRequest request)
        {
            var listResult = new ListResult
            {
                ItemsList = await _dbContext.Set<ApplicationRole>()
                    .OrderBy(r => r.Name)
                    .Select(r => new
                    {
                        r.Id,
                        Name = r.Name.ToUpper()
                    })
                    .ToListAsync()
            };

            return new SuccessResponse(listResult);
        }

        public override void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
