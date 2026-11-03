using System.ComponentModel.DataAnnotations;

namespace CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Models
{
    public class RegistrarMovimientoViewModel
    {
        [Required]
        public int CuentaID { get; set; }

        [Required]
        public string Tipo { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        public int? CuentaDestinoID { get; set; }
        public string NumeroCuentaOrigen { get; set; }
        public string NumeroCuentaDestino { get; set; }
    }
}

