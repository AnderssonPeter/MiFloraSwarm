namespace MiFloraGateway
{
    /// <summary>
    /// Contains regex expressions to validate different types of values
    /// </summary>
    public class ValidationPatterns
    {
        public const string MACAddressRegex = @"^(([0-9A-Fa-f]{2}[-]){5}([0-9A-Fa-f]{2})|([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2}))$";
        public const string IPAddressRegex = @"^(([a-f0-9:]+:+)+[a-f0-9]+|([0-9]{1,3}\.){3}[0-9]{1,3})$";
    }
}
