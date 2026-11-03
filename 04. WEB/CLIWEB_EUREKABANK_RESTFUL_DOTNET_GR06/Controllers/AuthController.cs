using System.Web.Mvc;
using Newtonsoft.Json;
using CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Models;
using CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Services;

namespace CLIWEB_EUREKABANK_RESTFUL_DOTNET_GR06.Controllers
{
    public class AuthController : Controller
    {
        private readonly EurekaBankService _service = new EurekaBankService();

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["Empleado"] != null) return RedirectToAction("Index", "Cuentas");
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var empleado = _service.Login(model.Username, model.Password);
            if (empleado == null)
            {
                ModelState.AddModelError("", "Usuario o contraseÃ±a incorrectos.");
                return View(model);
            }

            Session["Empleado"] = JsonConvert.SerializeObject(empleado);
            return RedirectToAction("Index", "Cuentas");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

