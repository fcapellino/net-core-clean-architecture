namespace MainProject.Core.CustomResults
{
    using System.Collections.Generic;

    public class ListResult
    {
        public IEnumerable<object> ItemsList { get; set; }

        public ListResult()
        {
        }
        public ListResult(IEnumerable<object> itemsList)
            : this()
        {
            ItemsList = itemsList;
        }
    }
}
