namespace MiFloraGateway
{
    /// <summary>
    /// Contains regex expressions to validate different types of values
    /// </summary>
    public class ValidationPatterns
    {

        private const string ipAddressRegex = @"(([a-f0-9:]+:+)+[a-f0-9]+|([0-9]{1,3}\.){3}[0-9]{1,3})";
        private const string hostnameRegex = @"(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])";

        public const string MACAddressRegex = @"^(([0-9A-Fa-f]{2}[-]){5}([0-9A-Fa-f]{2})|([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2}))$";
        public const string IPAddressRegex = "^" + ipAddressRegex + "$";
        public const string HostnameRegex = "^" + hostnameRegex + "$";
        public const string IPAddressOrHostnameRegex = "^(" + ipAddressRegex + "|" + hostnameRegex + ")$";
    }
}
