using System;
using System.Drawing;
using System.Windows.Forms;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Services;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms
{
    public class MovimientosForm : Form
    {
        private readonly EurekaBankService _service;
        private readonly int _cuentaId;
        private DataGridView dgv;

        public MovimientosForm(EurekaBankService service, int cuentaId, string numeroCuenta)
        {
            _service = service;
            _cuentaId = cuentaId;
            InitUI(numeroCuenta);
            CargarMovimientos();
        }

        private void InitUI(string numeroCuenta)
        {
            this.Text = $"Historial — Cuenta {numeroCuenta}";
            this.Size = new Size(950, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9f);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(26, 62, 107), Padding = new Padding(10, 0, 0, 0) };
            var lbl = new Label { Text = $"📄 Últimos 50 movimientos — Cuenta {numeroCuenta}", ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            pnlTop.Controls.Add(lbl);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9f)
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(26, 62, 107);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add("colOp",       "N° Operación");
            dgv.Columns.Add("colFecha",    "Fecha");
            dgv.Columns.Add("colTipo",     "Tipo");
            dgv.Columns.Add("colMonto",    "Monto ($)");
            dgv.Columns.Add("colAnterior", "Saldo Anterior");
            dgv.Columns.Add("colPosterior","Saldo Posterior");
            dgv.Columns.Add("colEmpleado", "Empleado");
            dgv.Columns.Add("colDestino",  "Cta. Destino");

            this.Controls.Add(dgv);
            this.Controls.Add(pnlTop);
        }

        private void CargarMovimientos()
        {
            var movimientos = _service.ObtenerMovimientos(_cuentaId);
            dgv.Rows.Clear();
            foreach (var m in movimientos)
            {
                int idx = dgv.Rows.Add(
                    m.NumeroOperacion,
                    m.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    m.Tipo,
                    m.Monto.ToString("N2"),
                    m.SaldoAnterior.ToString("N2"),
                    m.SaldoPosterior.ToString("N2"),
                    m.EmpleadoNombre,
                    string.IsNullOrEmpty(m.CuentaDestinoNum) ? "—" : m.CuentaDestinoNum);

                dgv.Rows[idx].DefaultCellStyle.BackColor =
                    m.Tipo == "Deposito" ? Color.FromArgb(220, 255, 220) :
                    m.Tipo == "Retiro"   ? Color.FromArgb(255, 220, 220) :
                                           Color.FromArgb(220, 240, 255);
            }
        }
    }
}
