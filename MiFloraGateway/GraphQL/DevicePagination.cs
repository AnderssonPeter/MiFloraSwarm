﻿using System.ComponentModel;
using System.Linq;
using MiFloraGateway.Database;

namespace MiFloraGateway.GraphQL
{
    public class DevicePagination : Pagination
    {
        [Description("collection of devices")]
        public IQueryable<Device> Devices { get; set; } = null!;
    }
}
