using System;
using System.Collections.Generic;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Models
{
    public class LoginResponse
    {
        public bool Exito { get; set; }
        public int EmpleadoID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public string Rol { get; set; }
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; }
        public string Mensaje { get; set; }
    }

    public class CuentaDto
    {
        public int CuentaID { get; set; }
        public string NumeroCuenta { get; set; }
        public string ClienteNombre { get; set; }
        public string Cedula { get; set; }
        public decimal Saldo { get; set; }
        public string Estado { get; set; }
        public bool Bloqueada { get; set; }
        public string BloqueadaPorNombre { get; set; }
        public string NombreSucursal { get; set; }
    }

    public class MovimientoDto
    {
        public int MovimientoID { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoPosterior { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroOperacion { get; set; }
        public string EmpleadoNombre { get; set; }
        public string NumeroCuenta { get; set; }
        public string CuentaDestinoNum { get; set; }
    }

    public class RegistrarMovimientoResponse
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
        public string NumeroOperacion { get; set; }
    }
}
