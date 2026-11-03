namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models
{
    public class LoginResponse
    {
        public bool Exito { get; set; }
        public int EmpleadoID { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Username { get; set; } = "";
        public string Rol { get; set; } = "";
        public int SucursalID { get; set; }
        public string NombreSucursal { get; set; } = "";
        public string Mensaje { get; set; } = "";
    }
}
