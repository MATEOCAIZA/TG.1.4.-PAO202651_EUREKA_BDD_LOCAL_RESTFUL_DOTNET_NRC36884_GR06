using System.Drawing;
using System.Windows.Forms;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblBienvenida;
        private Label lblSeleccionada;
        private DataGridView dgvCuentas;
        private Button btnActualizar;
        private Button btnHistorial;
        private Button btnTransaccion;
        private Button btnMashUp;
        private Button btnSalir;
        private Panel pnlTop;
        private Panel pnlBottom;
        private Panel pnlStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Eureka Bank — Gestión de Cuentas";
            this.Size = new Size(1150, 650);
            this.MinimumSize = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);
            this.BackColor = Color.White;

            // Panel superior
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(26, 62, 107),
                Padding = new Padding(14, 0, 14, 0)
            };

            lblBienvenida = new Label
            {
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Text = ""
            };
            pnlTop.Controls.Add(lblBienvenida);

            // Barra de estado (cuenta seleccionada)
            pnlStatus = new Panel
            {
                Dock = DockStyle.Top,
                Height = 28,
                BackColor = Color.FromArgb(230, 236, 244),
                Padding = new Padding(14, 0, 14, 0)
            };

            lblSeleccionada = new Label
            {
                ForeColor = Color.FromArgb(26, 62, 107),
                Font = new Font("Segoe UI", 8.5f),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Text = "Seleccione una cuenta de la lista para operar."
            };
            pnlStatus.Controls.Add(lblSeleccionada);

            // Panel inferior de botones
            pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(12, 10, 12, 10)
            };

            btnActualizar = new Button
            {
                Text = "↻ Actualizar",
                Size = new Size(130, 38),
                Location = new Point(12, 11),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnActualizar.FlatAppearance.BorderSize = 0;
            btnActualizar.Click += btnActualizar_Click;

            btnHistorial = new Button
            {
                Text = "Historial",
                Size = new Size(110, 38),
                Location = new Point(152, 11),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnHistorial.FlatAppearance.BorderSize = 0;
            btnHistorial.Click += btnHistorial_Click;

            btnTransaccion = new Button
            {
                Text = "Transacción",
                Size = new Size(130, 38),
                Location = new Point(272, 11),
                BackColor = Color.FromArgb(26, 62, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTransaccion.FlatAppearance.BorderSize = 0;
            btnTransaccion.Click += btnTransaccion_Click;

            btnMashUp = new Button
            {
                Text = "MashUp",
                Size = new Size(110, 38),
                Location = new Point(412, 11),
                BackColor = Color.FromArgb(23, 162, 184),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnMashUp.FlatAppearance.BorderSize = 0;
            btnMashUp.Click += btnMashUp_Click;

            btnSalir = new Button
            {
                Text = "Cerrar sesión",
                Size = new Size(130, 38),
                Location = new Point(532, 11),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.Click += btnSalir_Click;

            pnlBottom.Controls.AddRange(new Control[] {
                btnActualizar, btnHistorial, btnTransaccion, btnMashUp, btnSalir
            });

            // DataGridView
            dgvCuentas = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9f),
                GridColor = Color.FromArgb(220, 220, 220),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                RowTemplate = { Height = 30 }
            };
            dgvCuentas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(26, 62, 107);
            dgvCuentas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCuentas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvCuentas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvCuentas.ColumnHeadersHeight = 36;
            dgvCuentas.EnableHeadersVisualStyles = false;
            dgvCuentas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 252);
            dgvCuentas.SelectionChanged += dgvCuentas_SelectionChanged;

            dgvCuentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNumero",        HeaderText = "Nº Cuenta",   FillWeight = 18 });
            dgvCuentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTitular",        HeaderText = "Titular",          FillWeight = 28 });
            dgvCuentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCedula",         HeaderText = "Cédula",      FillWeight = 14 });
            dgvCuentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSaldo",          HeaderText = "Saldo ($)",        FillWeight = 14,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9f, FontStyle.Bold) } });
            dgvCuentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado",         HeaderText = "Estado",           FillWeight = 14 });
            dgvCuentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstadoBloqueo",  HeaderText = "Bloqueada por",    FillWeight = 12 });
            dgvCuentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCuentaID",       HeaderText = "ID",               Visible = false });

            this.Controls.Add(dgvCuentas);
            this.Controls.Add(pnlStatus);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlTop);
        }
    }
}
