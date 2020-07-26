namespace MainProject.Infrastructure.Handlers.Users.GetUsersListRequestHandler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Infrastructure.DataBaseContext;
    using MainProject.Infrastructure.Results;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class GetUsersListRequestHandler : BaseRequestHandler<GetUsersListRequest>
    {
        private readonly IDbContext _dbContext;

        public GetUsersListRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _dbContext = provider.GetService<IDbContext>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(GetUsersListRequest request)
        {
            var query = _dbContext.Set<ApplicationUser>()
                 .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                 .AsQueryable();

            query = ApplyQueryFilters(query, request);
            var pagedListResult = new PagedListResult
            {
                TotalItemCount = await query.CountAsync(),
                ItemsList = await query
                    .Select(item => new
                    {
                        item.Id,
                        item.FirstName,
                        item.LastName,
                        item.IsDisabled,
                        item.Email,
                        Role = new
                        {
                            Id = item.UserRoles.FirstOrDefault().RoleId,
                            item.UserRoles.FirstOrDefault().Role.Name
                        }
                    })
                    .ApplyOrdering(request.OrderByColumn)
                    .ApplyPaging(request.Page, request.PageSize)
                    .ToListAsync()
            };

            return new SuccessResponse(pagedListResult);
        }

        private IQueryable<ApplicationUser> ApplyQueryFilters(IQueryable<ApplicationUser> query, GetUsersListRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                query = query.Where(item => (item.FirstName + item.LastName + item.Email)
                    .ToLower().Contains(request.SearchQuery.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(request.RoleId))
            {
                query = query.Where(item => item.UserRoles.Any(x => x.RoleId.ToString().Equals(request.RoleId)));
            }

            return query;
        }
        public override void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
