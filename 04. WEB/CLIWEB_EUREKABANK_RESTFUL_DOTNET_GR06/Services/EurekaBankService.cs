using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Models;

namespace CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Services
{
    // Cliente HTTP generado a partir de la referencia de servicio OpenAPI
    // apuntando a: {ApiBaseUrl}/swagger/v1/swagger.json
    public class EurekaBankService
    {
        private readonly string _baseUrl;
        private static readonly HttpClient _http = new HttpClient();

        public EurekaBankService()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
        }

        // â”€â”€ Auth â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        public EmpleadoSession Login(string username, string password)
        {
            var body = JsonConvert.SerializeObject(new { username, password });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = _http.PostAsync(_baseUrl + "/api/auth/login", content).Result;

            if (!response.IsSuccessStatusCode) return null;

            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<LoginApiResponse>(json);

            if (result == null || !result.Exito) return null;

            return new EmpleadoSession
            {
                EmpleadoID     = result.EmpleadoID,
                Nombre         = result.Nombre,
                Apellido       = result.Apellido,
                Username       = result.Username,
                Rol            = result.Rol,
                SucursalID     = result.SucursalID,
                NombreSucursal = result.NombreSucursal
            };
        }

        // â”€â”€ Cuentas â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        public List<CuentaViewModel> ObtenerCuentasPorSucursal(int sucursalId)
        {
            var response = _http.GetAsync(_baseUrl + "/api/cuentas/sucursal/" + sucursalId).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<CuentaViewModel>>(json) ?? new List<CuentaViewModel>();
        }

        public List<CuentaViewModel> ObtenerTodasCuentasEstado()
        {
            var response = _http.GetAsync(_baseUrl + "/api/cuentas/estado").Result;
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<CuentaViewModel>>(json) ?? new List<CuentaViewModel>();
        }

        public bool BloquearCuenta(int cuentaId, int empleadoId)
        {
            var body = JsonConvert.SerializeObject(new { cuentaID = cuentaId, empleadoID = empleadoId });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = _http.PostAsync(_baseUrl + "/api/cuentas/bloquear", content).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public bool DesbloquearCuenta(int cuentaId, int empleadoId)
        {
            var body = JsonConvert.SerializeObject(new { cuentaID = cuentaId, empleadoID = empleadoId });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = _http.PostAsync(_baseUrl + "/api/cuentas/desbloquear", content).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<bool>(json);
        }

        // â”€â”€ Movimientos â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        public List<MovimientoViewModel> ObtenerMovimientos(int cuentaId)
        {
            var response = _http.GetAsync(_baseUrl + "/api/movimientos/cuenta/" + cuentaId).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<MovimientoViewModel>>(json) ?? new List<MovimientoViewModel>();
        }

        public RegistrarMovimientoResult RegistrarMovimiento(int cuentaId, int empleadoId,
            string tipo, decimal monto, int? cuentaDestinoId = null)
        {
            var payload = new
            {
                cuentaID = cuentaId,
                empleadoID = empleadoId,
                tipo,
                monto,
                cuentaDestinoID = cuentaDestinoId
            };
            var body = JsonConvert.SerializeObject(payload);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = _http.PostAsync(_baseUrl + "/api/movimientos/registrar", content).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<RegistrarMovimientoResult>(json);
        }
    }

    public class RegistrarMovimientoResult
    {
        public int Resultado { get; set; }
        public string Mensaje { get; set; }
        public string NumeroOperacion { get; set; }
    }

    public class LoginApiResponse
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
}

