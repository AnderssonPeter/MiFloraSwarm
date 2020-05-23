// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using System.ComponentModel.DataAnnotations;

namespace MiFloraGateway.Authentication
{
    public class InitialRegisterModel
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
