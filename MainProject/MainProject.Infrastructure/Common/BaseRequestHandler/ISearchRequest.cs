namespace MainProject.Infrastructure.Common.BaseRequestHandler
{
    public interface ISearchRequest
    {
        string SearchQuery { get; set; }
        string OrderByColumn { get; set; }
        int Page { get; set; }
        int PageSize { get; set; }
    }
}
