using System;

namespace MiFloraGateway.GraphQL
{
    public class DeviceError
    {
        public DateTime When { get; set; }
        public string? Message { get; set; }
        public DeviceError(DateTime when, string? message)
        {
            this.When = when;
            this.Message = message;
        }
    }
}
