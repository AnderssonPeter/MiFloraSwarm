using System.ComponentModel;

namespace MiFloraGateway.GraphQL
{
    public class Pagination
    {
        [Description("total records to match search")]
        public int Total { get; set; }
        [Description("total pages based on page size")]
        public int PageCount { get; set; }
    }
}
