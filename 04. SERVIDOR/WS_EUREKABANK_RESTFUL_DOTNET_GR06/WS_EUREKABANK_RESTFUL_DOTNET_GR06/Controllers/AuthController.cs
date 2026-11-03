using System;
using System.Web.Http;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Data;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Logging;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly DatabaseService _db = new DatabaseService();

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            LogService.Info($"POST api/auth/login -> username='{request?.Username}'");

            if (request == null || string.IsNullOrWhiteSpace(request.Username))
            {
                LogService.Warning("POST api/auth/login -> request nula o sin username");
                return BadRequest("Request invalida.");
            }

            try
            {
                var result = _db.Login(request.Username, request.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogService.Error("POST api/auth/login EXCEPCION NO CONTROLADA", ex);
                return InternalServerError(ex);
            }
        }
    }
}
