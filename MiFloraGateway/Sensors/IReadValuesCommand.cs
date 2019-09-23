﻿using Hangfire.Server;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public interface IReadValuesCommand
    {
        Task CommandAsync(PerformContext context);
    }
}