using System;
using System.Windows.Forms;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Models;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Services;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms
{
    public partial class LoginForm : Form
    {
        private readonly EurekaBankService _service = new EurekaBankService();
        public LoginResponse EmpleadoLogueado { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            HudTheme.Apply(this);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Complete usuario y contraseña.";
                return;
            }

            btnLogin.Enabled = false;
            lblError.Text = "Autenticando...";

            try
            {
                var result = _service.Login(txtUsername.Text.Trim(), txtPassword.Text.Trim());
                if (result != null && result.Exito)
                {
                    EmpleadoLogueado = result;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    lblError.Text = result?.Mensaje ?? "Error al conectar con el servidor.";
                    btnLogin.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
                btnLogin.Enabled = true;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnLogin_Click(sender, e);
        }
    }
}
