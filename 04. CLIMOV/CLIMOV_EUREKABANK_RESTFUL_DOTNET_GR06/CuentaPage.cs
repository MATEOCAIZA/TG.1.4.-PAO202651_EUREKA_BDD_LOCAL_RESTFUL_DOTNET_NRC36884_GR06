using System.Globalization;

namespace EurekaMovil;

/// <summary>Una cuenta: saldo, depósito / retiro / transferencia y sus movimientos.</summary>
public class CuentaPage : ContentPage
{
    readonly Empleado emp;
    readonly List<Cuenta> cuentas;
    Cuenta cuenta;
    readonly Entry monto, destino;
    readonly Label saldo = new() { TextColor = Hud.Orange, FontSize = 28, FontAttributes = FontAttributes.Bold };
    readonly Label resultado = Hud.Consola();
    readonly Label movimientos = Hud.Consola("", Hud.Green, 12);

    public CuentaPage(Empleado emp, Cuenta cuenta, List<Cuenta> cuentas)
    {
        this.emp = emp;
        this.cuenta = cuenta;
        this.cuentas = cuentas;
        Hud.Preparar(this);

        var campoMonto = Hud.Campo(out monto, "0.00", Keyboard.Numeric);
        var campoDestino = Hud.Campo(out destino, "N° de cuenta destino", Keyboard.Numeric);

        var botones = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
            ColumnSpacing = 12, RowSpacing = 10, Margin = new Thickness(0, 14, 0, 0),
        };
        botones.Add(Hud.Boton("DEPÓSITO", (_, _) => Operar("Deposito")));
        botones.Add(Hud.Boton("RETIRO", (_, _) => Operar("Retiro")), 1, 0);
        var transferir = Hud.Boton("TRANSFERENCIA", (_, _) => Operar("Transferencia"));
        botones.Add(transferir, 0, 1);
        Grid.SetColumnSpan(transferir, 2);

        Content = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            Children = { Hud.Cabecera($"NODO: {Servicio.Nodo} · {emp.Nombre}", Hud.Boton("‹ VOLVER", async (_, _) => await Navigation.PopAsync())) },
        };
        ((Grid)Content).Add(new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Children =
                {
                    Hud.Caja(new VerticalStackLayout
                    {
                        Children = { Hud.Consola("■ CUENTA " + cuenta.Numero, Hud.Green, 16),
                                     new Label { Text = cuenta.Cliente, TextColor = Hud.Text }, saldo },
                    }),
                    Hud.Etiqueta("▌ IMPORTE (Ej. 250.00)"), campoMonto,
                    Hud.Etiqueta("▌ CUENTA DESTINO (solo transferencia)"), campoDestino,
                    botones,
                    Hud.Etiqueta("▌ RESULTADO"), Hud.Caja(resultado, 14),
                    Hud.Etiqueta("▌ MOVIMIENTOS"), Hud.Caja(movimientos, 14),
                },
            },
        }, 0, 1);
        saldo.Text = Hud.Dinero(cuenta.Saldo);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Refrescar();
    }

    async void Operar(string tipo)
    {
        if (!decimal.TryParse((monto.Text ?? "").Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var m) || m <= 0)
        {
            Hud.Error(resultado, "Importe inválido (Ej. 250.00)");
            return;
        }
        int destinoId = 0;
        if (tipo == "Transferencia")
        {
            var d = cuentas.Find(c => c.Numero == (destino.Text ?? "").Trim());
            if (d == null) { Hud.Error(resultado, "Cuenta destino no encontrada en su sucursal"); return; }
            if (d.Id == cuenta.Id) { Hud.Error(resultado, "La cuenta destino debe ser distinta"); return; }
            destinoId = d.Id;
        }

        Hud.Esperando(resultado);
        try
        {
            var r = await Servicio.Registrar(cuenta.Id, emp.Id, tipo, m, destinoId);
            if (r.Exito) Hud.Ok(resultado, $"✔ {r.Mensaje}\n  N° operación: {r.Operacion}");
            else Hud.Error(resultado, r.Mensaje);
            if (r.Exito) monto.Text = "";
            await Refrescar();
        }
        catch (Exception ex) { Hud.Error(resultado, ex.Message); }
    }

    async Task Refrescar()
    {
        try
        {
            var actual = (await Servicio.Cuentas(emp.SucursalId)).Find(c => c.Id == cuenta.Id);
            if (actual != null) { cuenta = actual; saldo.Text = Hud.Dinero(actual.Saldo); }

            var lista = await Servicio.Movimientos(cuenta.Id);
            var sb = new System.Text.StringBuilder("FECHA       TIPO            MONTO\n──────────────────────────────────\n");
            foreach (var mv in lista)
            {
                string signo = mv.SaldoPosterior >= mv.SaldoAnterior ? "+" : "-";   // la transferencia recibida también es "Transferencia"
                sb.Append($"{mv.Fecha:dd/MM/yyyy}  {Recortar(mv.Tipo, 14),-14}  {signo}{mv.Monto.ToString("0.00", CultureInfo.InvariantCulture)}\n");
            }
            sb.Append("──────────────────────────────────\nTotal: ").Append(lista.Count);
            movimientos.Text = sb.ToString();
        }
        catch (Exception ex) { Hud.Error(movimientos, ex.Message); }
    }

    static string Recortar(string s, int n) => s.Length <= n ? s : s[..n];
}
