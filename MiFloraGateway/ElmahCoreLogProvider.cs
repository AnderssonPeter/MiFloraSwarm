﻿using ElmahCore;
using Hangfire.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway
{
    public class ElmahCoreLogProvider : ILogProvider
    {
        private readonly ErrorLog errorLog;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ElmahCoreLogProvider(ErrorLog errorLog, IHttpContextAccessor httpContextAccessor)
        {
            this.errorLog = errorLog;
            this.httpContextAccessor = httpContextAccessor;
        }

        public ILog GetLogger(string name)
        {
            return new ElmahCoreLog(name, errorLog, httpContextAccessor.HttpContext);
        }
    }
}
