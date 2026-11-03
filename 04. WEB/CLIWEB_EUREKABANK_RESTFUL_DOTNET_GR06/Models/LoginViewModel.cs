using System.ComponentModel.DataAnnotations;

namespace CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Usuario requerido")]
        public string Username { get; set; }

        [Required(ErrorMessage = "ContraseÃ±a requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

