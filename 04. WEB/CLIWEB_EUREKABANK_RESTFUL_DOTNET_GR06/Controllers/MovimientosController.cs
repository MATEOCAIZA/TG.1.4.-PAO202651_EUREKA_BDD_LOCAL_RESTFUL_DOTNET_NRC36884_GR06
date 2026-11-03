using System.Web.Mvc;
using Newtonsoft.Json;
using CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Models;
using CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Services;

namespace CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Controllers
{
    public class MovimientosController : Controller
    {
        private readonly EurekaBankService _service = new EurekaBankService();

        private EmpleadoSession GetEmpleado()
        {
            if (Session["Empleado"] == null) return null;
            return JsonConvert.DeserializeObject<EmpleadoSession>(Session["Empleado"].ToString());
        }

        public ActionResult Index(int cuentaId, string numeroCuenta)
        {
            var emp = GetEmpleado();
            if (emp == null) return RedirectToAction("Login", "Auth");

            var movimientos = _service.ObtenerMovimientos(cuentaId);
            ViewBag.Empleado = emp;
            ViewBag.CuentaId = cuentaId;
            ViewBag.NumeroCuenta = numeroCuenta;
            return View(movimientos);
        }
    }
}

