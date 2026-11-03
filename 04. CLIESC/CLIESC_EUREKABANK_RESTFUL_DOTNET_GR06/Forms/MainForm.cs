using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Models;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Services;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms
{
    public partial class MainForm : Form
    {
        private readonly EurekaBankService _service = new EurekaBankService();
        private readonly LoginResponse _empleado;

        public MainForm(LoginResponse empleado)
        {
            _empleado = empleado;
            InitializeComponent();
            lblBienvenida.Text = $"Bienvenido, {empleado.Nombre} {empleado.Apellido}  |  {empleado.Rol}  |  {empleado.NombreSucursal}";
            CargarCuentas();
        }

        private void CargarCuentas()
        {
            try
            {
                var cuentas = _service.ObtenerCuentasPorSucursal(_empleado.SucursalID);
                dgvCuentas.Rows.Clear();
                foreach (var c in cuentas)
                {
                    int idx = dgvCuentas.Rows.Add(
                        c.NumeroCuenta, c.ClienteNombre, c.Cedula,
                        c.Saldo.ToString("N2"), c.Estado,
                        c.Bloqueada ? "BLOQUEADA" : "DISPONIBLE",
                        c.CuentaID);
                    dgvCuentas.Rows[idx].DefaultCellStyle.BackColor =
                        c.Bloqueada ? Color.FromArgb(255, 200, 200) : Color.FromArgb(200, 255, 200);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar cuentas: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e) => CargarCuentas();

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            if (dgvCuentas.SelectedRows.Count == 0) { MessageBox.Show("Seleccione una cuenta."); return; }
            var row = dgvCuentas.SelectedRows[0];
            int cuentaId = (int)row.Cells["colCuentaID"].Value;
            string numeroCuenta = row.Cells["colNumero"].Value.ToString();
            new MovimientosForm(_service, cuentaId, numeroCuenta).ShowDialog();
        }

        private void btnTransaccion_Click(object sender, EventArgs e)
        {
            if (dgvCuentas.SelectedRows.Count == 0) { MessageBox.Show("Seleccione una cuenta."); return; }
            var row = dgvCuentas.SelectedRows[0];
            string estado = row.Cells["colEstadoBloqueo"].Value.ToString();
            if (estado == "BLOQUEADA")
            {
                MessageBox.Show("La cuenta está bloqueada por otro cajero.", "Cuenta no disponible",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int cuentaId = (int)row.Cells["colCuentaID"].Value;
            string numeroCuenta = row.Cells["colNumero"].Value.ToString();
            decimal saldo = decimal.Parse(row.Cells["colSaldo"].Value.ToString().Replace(",", ""));
            new TransaccionForm(_service, _empleado, cuentaId, numeroCuenta, saldo).ShowDialog();
            CargarCuentas();
        }

        private void btnMashUp_Click(object sender, EventArgs e)
        {
            new MashUpForm(_service).ShowDialog();
        }

        private void dgvCuentas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCuentas.SelectedRows.Count == 0)
            {
                lblSeleccionada.Text = "Seleccione una cuenta de la lista para operar.";
                return;
            }
            var row = dgvCuentas.SelectedRows[0];
            string numero = row.Cells["colNumero"].Value?.ToString() ?? "";
            string titular = row.Cells["colTitular"].Value?.ToString() ?? "";
            string saldo = row.Cells["colSaldo"].Value?.ToString() ?? "";
            string estado = row.Cells["colEstadoBloqueo"].Value?.ToString() ?? "";
            lblSeleccionada.Text = $"Seleccionada: {numero}  |  Titular: {titular}  |  Saldo: ${saldo}  |  Estado: {estado}";
        }

        private void btnSalir_Click(object sender, EventArgs e) => Application.Exit();
    }
}
