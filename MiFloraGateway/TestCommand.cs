using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway
{
    public class TestCommand
    {
        private readonly ILogger<TestCommand> logger;

        public TestCommand(ILogger<TestCommand> logger)
        {
            this.logger = logger;
        }

        public void Run(PerformContext context)
        {
            context.WriteLine("Pre");
            logger.LogTrace("Trace");
            logger.LogDebug("Debug");
            logger.LogInformation("Information");
            logger.LogWarning("Warning");
            logger.LogError("Error");
            logger.LogCritical("Critical");
            context.WriteLine("Post");
        }
    }
}
