using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MiFloraGateway.Database;

namespace MiFloraGateway.GraphQL
{
    public class DevicePagination : Pagination
    {
        [Required]
        [Description("collection of devices")]
        public IQueryable<Device> Devices { get; set; } = null!;
    }
}
