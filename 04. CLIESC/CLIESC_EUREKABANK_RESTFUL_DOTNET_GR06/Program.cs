using System;
using System.Windows.Forms;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // montos con punto decimal y $ en cualquier PC (es-ES/es-EC usan coma)
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                    Application.Run(new MainForm(loginForm.EmpleadoLogueado));
            }
        }
    }
}
