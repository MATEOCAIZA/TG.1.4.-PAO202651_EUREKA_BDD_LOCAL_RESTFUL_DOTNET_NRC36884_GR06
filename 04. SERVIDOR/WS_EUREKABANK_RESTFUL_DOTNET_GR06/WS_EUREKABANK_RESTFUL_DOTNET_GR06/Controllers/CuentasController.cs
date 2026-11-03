using System;
using System.Web.Http;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Data;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Logging;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Controllers
{
    [RoutePrefix("api/cuentas")]
    public class CuentasController : ApiController
    {
        private readonly DatabaseService _db = new DatabaseService();

        [HttpGet]
        [Route("sucursal/{sucursalId:int}")]
        public IHttpActionResult GetPorSucursal(int sucursalId)
        {
            LogService.Info($"GET api/cuentas/sucursal/{sucursalId}");
            try
            {
                return Ok(_db.ObtenerCuentasPorSucursal(sucursalId));
            }
            catch (Exception ex)
            {
                LogService.Error($"GET api/cuentas/sucursal/{sucursalId} EXCEPCION", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("estado")]
        public IHttpActionResult GetEstadoTodas()
        {
            LogService.Info("GET api/cuentas/estado");
            try
            {
                return Ok(_db.ObtenerTodasCuentasEstado());
            }
            catch (Exception ex)
            {
                LogService.Error("GET api/cuentas/estado EXCEPCION", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("bloquear")]
        public IHttpActionResult Bloquear([FromBody] BloquearCuentaRequest request)
        {
            LogService.Info($"POST api/cuentas/bloquear -> CuentaID={request?.CuentaID}, EmpleadoID={request?.EmpleadoID}");
            if (request == null)
            {
                LogService.Warning("POST api/cuentas/bloquear -> request nula");
                return BadRequest("Request invalida.");
            }

            try
            {
                return Ok(_db.BloquearCuenta(request.CuentaID, request.EmpleadoID));
            }
            catch (Exception ex)
            {
                LogService.Error($"POST api/cuentas/bloquear EXCEPCION CuentaID={request.CuentaID}", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("desbloquear")]
        public IHttpActionResult Desbloquear([FromBody] BloquearCuentaRequest request)
        {
            LogService.Info($"POST api/cuentas/desbloquear -> CuentaID={request?.CuentaID}, EmpleadoID={request?.EmpleadoID}");
            if (request == null)
            {
                LogService.Warning("POST api/cuentas/desbloquear -> request nula");
                return BadRequest("Request invalida.");
            }

            try
            {
                return Ok(_db.DesbloquearCuenta(request.CuentaID, request.EmpleadoID));
            }
            catch (Exception ex)
            {
                LogService.Error($"POST api/cuentas/desbloquear EXCEPCION CuentaID={request.CuentaID}", ex);
                return InternalServerError(ex);
            }
        }
    }
}
