using System.Web.Mvc;
using Newtonsoft.Json;
using CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Models;
using CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Services;

namespace CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Controllers
{
    public class CuentasController : Controller
    {
        private readonly EurekaBankService _service = new EurekaBankService();

        private EmpleadoSession GetEmpleado()
        {
            if (Session["Empleado"] == null) return null;
            return JsonConvert.DeserializeObject<EmpleadoSession>(Session["Empleado"].ToString());
        }

        public ActionResult Index()
        {
            var emp = GetEmpleado();
            if (emp == null) return RedirectToAction("Login", "Auth");

            var cuentas = _service.ObtenerCuentasPorSucursal(emp.SucursalID);
            ViewBag.Empleado = emp;
            return View(cuentas);
        }

        [HttpPost]
        public ActionResult Transaccion(RegistrarMovimientoViewModel model)
        {
            var emp = GetEmpleado();
            if (emp == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction("Index");
            }

            // Resolver cuenta destino: el usuario ingresa NumeroCuenta, no CuentaID
            int? cuentaDestinoId = null;
            if (model.Tipo == "Transferencia")
            {
                if (string.IsNullOrWhiteSpace(model.NumeroCuentaDestino))
                {
                    TempData["Error"] = "Ingrese el número de cuenta destino.";
                    return RedirectToAction("Index");
                }
                var todasCuentas = _service.ObtenerTodasCuentasEstado();
                var destino = todasCuentas.Find(c => c.NumeroCuenta == model.NumeroCuentaDestino.Trim());
                if (destino == null)
                {
                    TempData["Error"] = "Cuenta destino no encontrada.";
                    return RedirectToAction("Index");
                }
                cuentaDestinoId = destino.CuentaID;
            }

            // Bloquear la cuenta antes de operar
            _service.BloquearCuenta(model.CuentaID, emp.EmpleadoID);

            var resultado = _service.RegistrarMovimiento(
                model.CuentaID, emp.EmpleadoID, model.Tipo, model.Monto, cuentaDestinoId);

            // Liberar bloqueo siempre
            _service.DesbloquearCuenta(model.CuentaID, emp.EmpleadoID);

            if (resultado.Resultado == 1)
                TempData["Exito"] = $"Operación exitosa. N° {resultado.NumeroOperacion}";
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction("Index");
        }
    }
}

