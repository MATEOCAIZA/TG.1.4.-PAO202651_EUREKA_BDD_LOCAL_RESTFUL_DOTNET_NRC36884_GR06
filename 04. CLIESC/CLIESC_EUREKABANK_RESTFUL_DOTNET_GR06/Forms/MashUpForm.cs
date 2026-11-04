using System;
using System.Drawing;
using System.Windows.Forms;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Services;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms
{
    public class MashUpForm : Form
    {
        private readonly EurekaBankService _service;
        private FlowLayoutPanel flowPanel;
        private Timer timer;

        public MashUpForm(EurekaBankService service)
        {
            _service = service;
            InitUI();
            HudTheme.Apply(this);
            CargarMashUp();
        }

        private void InitUI()
        {
            this.Text = "MashUp — Estado de Cuentas en Tiempo Real";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Font = new Font("Segoe UI", 9f);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(26, 62, 107), Padding = new Padding(15, 0, 0, 0) };
            var lblTitulo = new Label { Text = "📊 MashUp — Concurrencia en Tiempo Real  (🟢 Libre  🔴 Bloqueada)", ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Dock = DockStyle.Left, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, AutoSize = true };
            var btnRefresh = new Button { Text = "↻ Actualizar", Dock = DockStyle.Right, Width = 110, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => CargarMashUp();
            pnlTop.Controls.Add(lblTitulo);
            pnlTop.Controls.Add(btnRefresh);

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            flowPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = true, Padding = new Padding(10) };
            scroll.Controls.Add(flowPanel);

            // Auto-refresh cada 5 segundos
            timer = new Timer { Interval = 5000 };
            timer.Tick += (s, e) => CargarMashUp();
            timer.Start();
            this.FormClosed += (s, e) => timer.Stop();

            var lblInfo = new Label { Text = "Se actualiza automáticamente cada 5 segundos.", Dock = DockStyle.Bottom, Height = 25, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, ForeColor = Color.Gray, Font = new Font("Segoe UI", 8f) };

            this.Controls.Add(scroll);
            this.Controls.Add(pnlTop);
            this.Controls.Add(lblInfo);
        }

        private void CargarMashUp()
        {
            flowPanel.SuspendLayout();
            flowPanel.Controls.Clear();
            var cuentas = _service.ObtenerTodasCuentasEstado();
            foreach (var c in cuentas)
            {
                var card = new Panel
                {
                    Size = new Size(190, 130),
                    Margin = new Padding(6),
                    BackColor = c.Bloqueada ? Color.FromArgb(255, 200, 200) : Color.FromArgb(200, 255, 200),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var lblNumero  = new Label { Text = c.NumeroCuenta, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Location = new Point(8, 8), AutoSize = true };
                var lblTitular = new Label { Text = c.ClienteNombre, Location = new Point(8, 28), AutoSize = true, Font = new Font("Segoe UI", 8f) };
                var lblSaldo   = new Label { Text = "$" + c.Saldo.ToString("N2"), Location = new Point(8, 48), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
                var lblSucursal = new Label { Text = c.NombreSucursal, Location = new Point(8, 68), AutoSize = true, Font = new Font("Segoe UI", 7.5f), ForeColor = Color.DimGray };
                var lblEstado  = new Label
                {
                    Text = c.Bloqueada ? $"🔴 BLOQUEADA\n{c.BloqueadaPorNombre}" : "🟢 DISPONIBLE",
                    Location = new Point(8, 88),
                    Size = new Size(175, 35),
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = c.Bloqueada ? Color.DarkRed : Color.DarkGreen
                };

                card.Controls.AddRange(new Control[] { lblNumero, lblTitular, lblSaldo, lblSucursal, lblEstado });
                flowPanel.Controls.Add(card);
            }
            flowPanel.ResumeLayout();
        }
    }
}
