// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiFloraGateway.Onboarding
{
    public class SetupModel
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public Dictionary<Settings, object> Settings { get; set; } = null!;
    }
}
