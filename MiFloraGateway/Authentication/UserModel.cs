// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiFloraGateway.Authentication
{
    public class UserModel
    {
        public string Username { get; set; } = null!;
        public bool IsAdmin { get; set; }
    }
}
