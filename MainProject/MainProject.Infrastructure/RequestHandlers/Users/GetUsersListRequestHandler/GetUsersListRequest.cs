namespace MainProject.Infrastructure.Handlers.Users.GetUsersListRequestHandler
{
    using Common.BaseRequestHandler;

    public class GetUsersListRequest : BaseRequest, ISearchRequest
    {
        public string SearchQuery { get; set; }
        public string OrderByColumn { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string RoleId { get; set; }
    }
}
