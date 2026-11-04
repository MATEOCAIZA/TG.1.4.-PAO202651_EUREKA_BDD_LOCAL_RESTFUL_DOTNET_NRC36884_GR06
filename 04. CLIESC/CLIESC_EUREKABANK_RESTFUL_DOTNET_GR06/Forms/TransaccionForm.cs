using System;
using System.Drawing;
using System.Windows.Forms;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Models;
using CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Services;

namespace CLIESC_EUREKABANK_RESTFUL_DOTNET_GR01.Forms
{
    public class TransaccionForm : Form
    {
        private readonly EurekaBankService _service;
        private readonly LoginResponse _empleado;
        private readonly int _cuentaId;

        private ComboBox cmbTipo;
        private NumericUpDown nudMonto;
        private TextBox txtCuentaDestino;
        private Label lblDestino;
        private Button btnConfirmar;
        private Button btnCancelar;
        private Label lblInfo;
        private Label lblResult;

        public TransaccionForm(EurekaBankService service, LoginResponse empleado,
            int cuentaId, string numeroCuenta, decimal saldo)
        {
            _service = service;
            _empleado = empleado;
            _cuentaId = cuentaId;
            InitUI(numeroCuenta, saldo);
            HudTheme.Apply(this);
        }

        private void InitUI(string numeroCuenta, decimal saldo)
        {
            this.Text = "Registrar Transacción";
            this.Size = new Size(400, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9f);
            this.BackColor = Color.White;

            int y = 20;
            AddLabel($"Cuenta:  {numeroCuenta}    Saldo: ${saldo:N2}", ref y, bold: true, color: Color.FromArgb(26, 62, 107));

            y += 10;
            AddLabel("Tipo de operación:", ref y);
            cmbTipo = new ComboBox { Location = new Point(20, y), Size = new Size(340, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipo.Items.AddRange(new[] { "Deposito", "Retiro", "Transferencia" });
            cmbTipo.SelectedIndex = 0;
            cmbTipo.SelectedIndexChanged += (s, e) => lblDestino.Visible = txtCuentaDestino.Visible = cmbTipo.Text == "Transferencia";
            Controls.Add(cmbTipo);
            y += 35;

            AddLabel("Monto ($):", ref y);
            nudMonto = new NumericUpDown { Location = new Point(20, y), Size = new Size(340, 25), DecimalPlaces = 2, Minimum = 0.01m, Maximum = 9999999m, Increment = 100m };
            Controls.Add(nudMonto);
            y += 40;

            lblDestino = new Label { Text = "N° Cuenta Destino:", Location = new Point(20, y), AutoSize = true, Visible = false };
            Controls.Add(lblDestino);
            y += 22;
            txtCuentaDestino = new TextBox { Location = new Point(20, y), Size = new Size(340, 25), Visible = false };
            Controls.Add(txtCuentaDestino);
            y += 50;

            lblResult = new Label { Location = new Point(20, y), Size = new Size(340, 24), ForeColor = Color.DarkGreen, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            Controls.Add(lblResult);
            y += 30;

            btnConfirmar = new Button
            {
                Text = "Confirmar",
                Location = new Point(20, y),
                Size = new Size(160, 36),
                BackColor = Color.FromArgb(26, 62, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;
            Controls.Add(btnConfirmar);

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(200, y),
                Size = new Size(160, 36),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => Close();
            Controls.Add(btnCancelar);
        }

        private void AddLabel(string text, ref int y, bool bold = false, Color? color = null)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(20, y),
                AutoSize = true,
                Font = bold ? new Font("Segoe UI", 9f, FontStyle.Bold) : new Font("Segoe UI", 9f),
                ForeColor = color ?? Color.Black
            };
            Controls.Add(lbl);
            y += 22;
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            btnConfirmar.Enabled = false;
            try
            {
                // Resolver cuenta destino por NumeroCuenta si es Transferencia
                int? destino = null;
                if (cmbTipo.Text == "Transferencia")
                {
                    string numDestino = txtCuentaDestino.Text.Trim();
                    if (string.IsNullOrEmpty(numDestino))
                    {
                        lblResult.ForeColor = Color.Crimson;
                        lblResult.Text = "Ingrese el número de cuenta destino.";
                        btnConfirmar.Enabled = true;
                        return;
                    }
                    var todasCuentas = _service.ObtenerTodasCuentasEstado();
                    var cuentaDest = todasCuentas.Find(c => c.NumeroCuenta == numDestino);
                    if (cuentaDest == null)
                    {
                        lblResult.ForeColor = Color.Crimson;
                        lblResult.Text = "Cuenta destino no encontrada.";
                        btnConfirmar.Enabled = true;
                        return;
                    }
                    destino = cuentaDest.CuentaID;
                }

                _service.BloquearCuenta(_cuentaId, _empleado.EmpleadoID);

                var result = _service.RegistrarMovimiento(
                    _cuentaId, _empleado.EmpleadoID, cmbTipo.Text, nudMonto.Value, destino);

                _service.DesbloquearCuenta(_cuentaId, _empleado.EmpleadoID);

                if (result.Resultado == 1)
                {
                    MessageBox.Show($"Operación exitosa.\nN° Operación: {result.NumeroOperacion}",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    lblResult.ForeColor = Color.Crimson;
                    lblResult.Text = result.Mensaje;
                    btnConfirmar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _service.DesbloquearCuenta(_cuentaId, _empleado.EmpleadoID);
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConfirmar.Enabled = true;
            }
        }
    }
}
