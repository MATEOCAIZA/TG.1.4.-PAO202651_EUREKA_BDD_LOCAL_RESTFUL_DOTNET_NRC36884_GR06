using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Logging;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LogService.Info("=== Eureka Bank REST Server INICIANDO ===");
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            LogService.Info("=== Eureka Bank REST Server LISTO (puerto configurado en IIS) ===");
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                var httpEx = ex as HttpException;
                // Ignorar 404 — son rutas inexistentes, no errores reales
                if (httpEx != null && httpEx.GetHttpCode() == 404)
                    return;

                LogService.Error($"Application_Error no controlado -> URL: {Request?.RawUrl}", ex);
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            LogService.Info("=== Eureka Bank REST Server DETENIDO ===");
        }
    }
}
