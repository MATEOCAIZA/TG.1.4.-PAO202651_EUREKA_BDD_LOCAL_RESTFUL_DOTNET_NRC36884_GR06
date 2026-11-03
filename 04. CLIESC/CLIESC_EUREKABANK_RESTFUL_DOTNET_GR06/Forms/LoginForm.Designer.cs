using System.Drawing;
using System.Windows.Forms;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        // Controles
        private Panel pnlLeft;
        private Panel pnlRight;
        private PictureBox picLogo;
        private Label lblBankName;
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblPasswordLabel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;
        private Label lblFooter;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Form ─────────────────────────────────────────────────────────
            this.Text = "Eureka Bank — Iniciar Sesión";
            this.Size = new Size(850, 520);
            this.MinimumSize = new Size(850, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9f);

            // ── Panel izquierdo (imagen) ──────────────────────────────────────
            pnlLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = 420,
                BackColor = Color.FromArgb(26, 62, 107)
            };

            picLogo = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(26, 62, 107)
            };
            try
            {
                string imgPath = System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "login.jpeg");
                if (System.IO.File.Exists(imgPath))
                    picLogo.Image = Image.FromFile(imgPath);
            }
            catch { /* imagen opcional */ }

            pnlLeft.Controls.Add(picLogo);

            // ── Panel derecho (formulario) ────────────────────────────────────
            pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40),
                BackColor = Color.White
            };

            lblBankName = new Label
            {
                Text = "\U0001F3E6 Eureka Bank",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 62, 107),
                AutoSize = true,
                Location = new Point(40, 50)
            };

            lblSubtitle = new Label
            {
                Text = "Sistema de Gestión Bancaria",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(42, 85)
            };

            lblUsername = new Label
            {
                Text = "Usuario:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 140)
            };

            txtUsername = new TextBox
            {
                Location = new Point(40, 160),
                Size = new Size(320, 30),
                Font = new Font("Segoe UI", 11f)
            };

            lblPasswordLabel = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 205)
            };

            txtPassword = new TextBox
            {
                Location = new Point(40, 225),
                Size = new Size(320, 30),
                Font = new Font("Segoe UI", 11f),
                UseSystemPasswordChar = true
            };
            txtPassword.KeyDown += txtPassword_KeyDown;

            btnLogin = new Button
            {
                Text = "Iniciar Sesión",
                Location = new Point(40, 280),
                Size = new Size(320, 40),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                BackColor = Color.FromArgb(26, 62, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += btnLogin_Click;

            lblError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Crimson,
                AutoSize = false,
                Location = new Point(40, 330),
                Size = new Size(320, 20)
            };

            lblFooter = new Label
            {
                Text = "© 2025 Eureka Bank — Todos los derechos reservados",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(40, 420)
            };

            pnlRight.Controls.AddRange(new Control[] {
                lblBankName, lblSubtitle, lblUsername, txtUsername,
                lblPasswordLabel, txtPassword, btnLogin, lblError, lblFooter
            });

            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);
        }
    }
}
