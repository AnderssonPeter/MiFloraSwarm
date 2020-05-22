using System;
using ElmahCore;
using Hangfire.Logging;
using Microsoft.AspNetCore.Http;

namespace MiFloraGateway
{
    public class ElmahCoreLog : ILog
    {
        private readonly string name;
        private readonly ErrorLog errorLog;
        private readonly HttpContext httpContext;

        public ElmahCoreLog(string name, ErrorLog errorLog, HttpContext httpContext)
        {
            this.name = name;
            this.errorLog = errorLog;
            this.httpContext = httpContext;
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            if (messageFunc == null)
                return true;
            errorLog.Log(new Error(exception, httpContext)
            {
                ApplicationName = "HangFire." + name,
                Message = messageFunc(),
                Type = logLevel.ToString(),
                Time = DateTime.Now
            });

            return true;
        }
    }
}
