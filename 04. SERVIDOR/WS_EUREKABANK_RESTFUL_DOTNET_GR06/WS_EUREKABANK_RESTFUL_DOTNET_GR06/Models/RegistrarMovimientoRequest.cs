namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models
{
    public class RegistrarMovimientoRequest
    {
        public int CuentaID { get; set; }
        public int EmpleadoID { get; set; }
        public string Tipo { get; set; } = "";
        public decimal Monto { get; set; }
        public int? CuentaDestinoID { get; set; }
    }
}
