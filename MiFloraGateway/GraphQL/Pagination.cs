using System.ComponentModel;

namespace MiFloraGateway.GraphQL
{
    public class Pagination
    {
        [Description("total pages based on page size")]
        public int PageCount { get; set; }
    }
}
