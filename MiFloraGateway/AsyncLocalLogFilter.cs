using Hangfire;
using Hangfire.Annotations;
using Hangfire.Server;
using Hangfire.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Storage.Monitoring;
using Hangfire.States;
using Newtonsoft.Json;

namespace MiFloraGateway
{
    public interface IJobManager
    {
        Task<TResult> StartWaitAsync<TResult, TJob>([InstantHandle][NotNull] Expression<Func<TJob, Task>> methodCall, CancellationToken cancellationToken = default);
        void Start<TJob>([InstantHandle] [NotNull] Expression<Action<TJob>> methodCall);
    }

    public class JobManager : IJobManager
    {
        private readonly IMonitoringApi monitoringApi;
        private readonly IPerformingContextAccessor performingContextAccessor;

        private readonly IBackgroundJobClient backgroundJobClient;

        public JobManager(JobStorage jobStorage, IBackgroundJobClient backgroundJobClient, IPerformingContextAccessor performingContextAccessor)
        {   
            this.monitoringApi = jobStorage.GetMonitoringApi();
            this.performingContextAccessor = performingContextAccessor;
            this.backgroundJobClient = backgroundJobClient;
        }

        private readonly string[] runningStates = new[] { AwaitingState.StateName, EnqueuedState.StateName, ProcessingState.StateName };
        
        public async Task<TResult> StartWaitAsync<TResult, TJob>([InstantHandle, NotNull] Expression<Func<TJob, Task>> methodCall, CancellationToken cancellationToken = default)
        {
            var jobId = backgroundJobClient.Enqueue(methodCall);
            while(true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var jobDetails = monitoringApi.JobDetails(jobId);
                string currentState = jobDetails.History[0].StateName;
                if (!runningStates.Contains(currentState))
                {
                    if (currentState == SucceededState.StateName)
                        return GetResult<TResult>(jobId);
                    else if (currentState == FailedState.StateName)
                        return ThrowError<TResult>(jobId);
                    else
                        throw new InvalidOperationException($"The job must be in the state '{SucceededState.StateName}' or '{FailedState.StateName}' but is in '{currentState}'");

                }
                await Task.Delay(100, cancellationToken);
            }
        }

        public void Start<TJob>([InstantHandle] [NotNull] Expression<Action<TJob>> methodCall)
        {
            var context = performingContextAccessor.Get();
            if (context != null)
            {
                backgroundJobClient.ContinueJobWith(context.BackgroundJob.Id, methodCall);
            }
            else
            {
                backgroundJobClient.Enqueue(methodCall);
            }
        }

        private TResult GetResult<TResult>(string jobId)
        {
            var total = (int)monitoringApi.SucceededListCount();

            var numberOfJobs = 10;
            for (var i = 0; i < total; i += numberOfJobs)
            {
                var start = Math.Max(total - i - numberOfJobs, 0);
                var end = total - i;
                var count = end - start;
                var job = monitoringApi.SucceededJobs(start, count).SingleOrDefault(x => x.Key == jobId).Value;
                if (job != null)
                {
                    var result = job.Result;
                    if (result.GetType() == typeof(string))
                    {
                        return JsonConvert.DeserializeObject<TResult>((string)result);
                    }
                    return (TResult)job.Result;
                }
            }
            throw new InvalidOperationException("Failed to find job");
        }

        private TResult ThrowError<TResult>(string jobId)
        {
            var total = (int)monitoringApi.FailedCount();

            var numberOfJobs = 10;
            for (var i = 0; i < total; i += numberOfJobs)
            {
                var start = Math.Max(total - i - numberOfJobs, 0);
                var end = total - i;
                var count = end - start;
                var job = monitoringApi.FailedJobs(start, count).SingleOrDefault(x => x.Key == jobId).Value;
                if (job != null)
                {
                    throw new JobFailedException($"The job threw a exception of type '{job.ExceptionType}'\nMessage: {job.ExceptionMessage}\nDetails: {job.ExceptionDetails}");
                }
            }
            throw new InvalidOperationException("Failed to find job");
        }
    }

    [Serializable]
    public class JobFailedException : Exception
    {
        public JobFailedException() { }
        public JobFailedException(string message) : base(message) { }
        public JobFailedException(string message, Exception inner) : base(message, inner) { }
        protected JobFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class AsyncLocalLogFilter : IServerFilter, IDisposable, IPerformingContextAccessor
    {
        AsyncLocal<PerformingContext> localStorage = new AsyncLocal<PerformingContext>();

        public AsyncLocalLogFilter()
        {
            GlobalJobFilters.Filters.Add(this);
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            localStorage.Value = filterContext;
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            localStorage.Value = null;
        }

        public void Dispose()
        {
            GlobalJobFilters.Filters.Remove(this);
        }

        public PerformingContext Get()
        {
            return localStorage.Value;
        }
    }
}
