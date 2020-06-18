// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using System.ComponentModel.DataAnnotations;

namespace MiFloraGateway.Authentication
{
    public class UserModel
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public bool IsAdmin { get; set; }
    }
}
