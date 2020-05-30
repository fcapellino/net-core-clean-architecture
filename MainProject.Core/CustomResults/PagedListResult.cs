namespace MainProject.Core.CustomResults
{
    using System.Collections.Generic;

    public sealed class PagedListResult
    {
        public IEnumerable<object> ItemsList { get; set; }
        public int TotalItemCount { get; set; }

        public PagedListResult()
        {
        }
        public PagedListResult(IEnumerable<object> itemsList, int totalItemCount)
            : this()
        {
            ItemsList = itemsList;
            TotalItemCount = totalItemCount;
        }
    }
}
