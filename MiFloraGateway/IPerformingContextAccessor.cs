using Hangfire.Server;

namespace MiFloraGateway
{
    public interface IPerformingContextAccessor
    {
        PerformingContext Get();
    }
}
