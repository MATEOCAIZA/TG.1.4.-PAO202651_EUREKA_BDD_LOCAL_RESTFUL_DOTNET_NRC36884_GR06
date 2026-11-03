using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using CLICON_EUREKABANK_RESTFUL_DOTNET_GR01.Models;

namespace CLICON_EUREKABANK_RESTFUL_DOTNET_GR01.Services
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

        public LoginResponse Login(string username, string password)
        {
            var body = JsonConvert.SerializeObject(new { username, password });
            var response = _http.PostAsync(_baseUrl + "/api/auth/login",
                new StringContent(body, Encoding.UTF8, "application/json")).Result;
            return JsonConvert.DeserializeObject<LoginResponse>(response.Content.ReadAsStringAsync().Result);
        }

        public List<CuentaDto> ObtenerCuentasPorSucursal(int sucursalId)
        {
            var response = _http.GetAsync(_baseUrl + "/api/cuentas/sucursal/" + sucursalId).Result;
            return JsonConvert.DeserializeObject<List<CuentaDto>>(
                response.Content.ReadAsStringAsync().Result) ?? new List<CuentaDto>();
        }

        public List<CuentaDto> ObtenerTodasCuentasEstado()
        {
            var response = _http.GetAsync(_baseUrl + "/api/cuentas/estado").Result;
            return JsonConvert.DeserializeObject<List<CuentaDto>>(
                response.Content.ReadAsStringAsync().Result) ?? new List<CuentaDto>();
        }

        public bool BloquearCuenta(int cuentaId, int empleadoId)
        {
            var body = JsonConvert.SerializeObject(new { cuentaID = cuentaId, empleadoID = empleadoId });
            var response = _http.PostAsync(_baseUrl + "/api/cuentas/bloquear",
                new StringContent(body, Encoding.UTF8, "application/json")).Result;
            return JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);
        }

        public bool DesbloquearCuenta(int cuentaId, int empleadoId)
        {
            var body = JsonConvert.SerializeObject(new { cuentaID = cuentaId, empleadoID = empleadoId });
            var response = _http.PostAsync(_baseUrl + "/api/cuentas/desbloquear",
                new StringContent(body, Encoding.UTF8, "application/json")).Result;
            return JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);
        }

        public List<MovimientoDto> ObtenerMovimientos(int cuentaId)
        {
            var response = _http.GetAsync(_baseUrl + "/api/movimientos/cuenta/" + cuentaId).Result;
            return JsonConvert.DeserializeObject<List<MovimientoDto>>(
                response.Content.ReadAsStringAsync().Result) ?? new List<MovimientoDto>();
        }

        public RegistrarMovimientoResponse RegistrarMovimiento(
            int cuentaId, int empleadoId, string tipo, decimal monto, int? cuentaDestinoId = null)
        {
            var payload = JsonConvert.SerializeObject(new
            {
                cuentaID = cuentaId,
                empleadoID = empleadoId,
                tipo, monto,
                cuentaDestinoID = cuentaDestinoId
            });
            var response = _http.PostAsync(_baseUrl + "/api/movimientos/registrar",
                new StringContent(payload, Encoding.UTF8, "application/json")).Result;
            return JsonConvert.DeserializeObject<RegistrarMovimientoResponse>(
                response.Content.ReadAsStringAsync().Result);
        }
    }
}
