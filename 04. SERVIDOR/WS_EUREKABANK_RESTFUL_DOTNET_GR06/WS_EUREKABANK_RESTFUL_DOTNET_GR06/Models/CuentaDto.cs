namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models
{
    public class CuentaDto
    {
        public int CuentaID { get; set; }
        public string NumeroCuenta { get; set; } = "";
        public string ClienteNombre { get; set; } = "";
        public string Cedula { get; set; } = "";
        public decimal Saldo { get; set; }
        public string Estado { get; set; } = "";
        public bool Bloqueada { get; set; }
        public string BloqueadaPorNombre { get; set; } = "";
        public string NombreSucursal { get; set; } = "";
    }
}
