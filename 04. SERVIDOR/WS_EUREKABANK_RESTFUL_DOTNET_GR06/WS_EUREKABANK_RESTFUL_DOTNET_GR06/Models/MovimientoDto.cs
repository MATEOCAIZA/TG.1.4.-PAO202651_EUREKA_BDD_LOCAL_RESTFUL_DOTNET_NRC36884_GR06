using System;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models
{
    public class MovimientoDto
    {
        public int MovimientoID { get; set; }
        public string Tipo { get; set; } = "";
        public decimal Monto { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoPosterior { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroOperacion { get; set; } = "";
        public string EmpleadoNombre { get; set; } = "";
        public string NumeroCuenta { get; set; } = "";
        public string CuentaDestinoNum { get; set; } = "";
    }
}
