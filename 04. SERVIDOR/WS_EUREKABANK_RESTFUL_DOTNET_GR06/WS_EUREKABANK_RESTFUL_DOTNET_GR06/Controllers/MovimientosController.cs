using System;
using System.Web.Http;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Data;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Logging;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Controllers
{
    [RoutePrefix("api/movimientos")]
    public class MovimientosController : ApiController
    {
        private readonly DatabaseService _db = new DatabaseService();

        [HttpGet]
        [Route("cuenta/{cuentaId:int}")]
        public IHttpActionResult GetPorCuenta(int cuentaId)
        {
            LogService.Info($"GET api/movimientos/cuenta/{cuentaId}");
            try
            {
                return Ok(_db.ObtenerMovimientos(cuentaId));
            }
            catch (Exception ex)
            {
                LogService.Error($"GET api/movimientos/cuenta/{cuentaId} EXCEPCION", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult Registrar([FromBody] RegistrarMovimientoRequest request)
        {
            LogService.Info($"POST api/movimientos/registrar -> CuentaID={request?.CuentaID}, Tipo={request?.Tipo}, Monto={request?.Monto}");
            if (request == null)
            {
                LogService.Warning("POST api/movimientos/registrar -> request nula");
                return BadRequest("Request invalida.");
            }

            try
            {
                return Ok(_db.RegistrarMovimiento(request));
            }
            catch (Exception ex)
            {
                LogService.Error($"POST api/movimientos/registrar EXCEPCION CuentaID={request.CuentaID}", ex);
                return InternalServerError(ex);
            }
        }
    }
}
