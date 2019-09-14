using Hangfire;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway
{
    public static class IBackgroundJobClientExtensions
    {
        public static Task<T> RunTaskAsync<T>(this IBackgroundJobClient backgroundJobClient, Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            var command = new AsyncCommandResult<T>(backgroundJobClient, action, cancellationToken);
            return command.RunAsync();
        }
    }

    public class AsyncCommandResult<T>
    {
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly Func<CancellationToken, Task<T>> action;
        private readonly CancellationToken cancellationToken;
        AsyncManualResetEvent monitor = new AsyncManualResetEvent();
        Exception exception;
        T result;


        public AsyncCommandResult(IBackgroundJobClient backgroundJobClient, Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            this.backgroundJobClient = backgroundJobClient;
            this.action = action;
            this.cancellationToken = cancellationToken;
        }

        public async Task<T> RunAsync()
        {
            backgroundJobClient.Schedule(() => action(cancellationToken), TimeSpan.Zero);
            await monitor.WaitAsync(cancellationToken);
            if (exception != null)
                throw new Exception("Job caused exception", exception);
            return result;
        }

        private async Task InnerAsync(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            try
            {
                result = await action(cancellationToken);
            }
            catch(Exception ex)
            {
                exception = ex;
            }
            finally
            {
                monitor.Set();
            }
        }
    }
}
