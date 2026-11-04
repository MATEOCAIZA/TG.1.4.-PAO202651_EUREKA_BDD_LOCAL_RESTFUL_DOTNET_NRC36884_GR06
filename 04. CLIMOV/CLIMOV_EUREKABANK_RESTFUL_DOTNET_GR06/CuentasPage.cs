namespace EurekaMovil;

/// <summary>Cuentas de la sucursal del empleado; al tocar una se abre para operar.</summary>
public class CuentasPage : ContentPage
{
    readonly Empleado emp;
    readonly VerticalStackLayout lista = new() { Spacing = 10 };
    readonly Label estado = Hud.Consola("", Hud.Muted);
    List<Cuenta> cuentas = new();

    public CuentasPage(Empleado emp)
    {
        this.emp = emp;
        Hud.Preparar(this);
        Content = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            Children =
            {
                Hud.Cabecera($"NODO: {Servicio.Nodo} · {emp.Nombre}", Hud.Boton("SALIR", async (_, _) => await Navigation.PopAsync())),
            },
        };
        var cuerpo = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Children = { Hud.Etiqueta($"▌ CUENTAS · SUCURSAL {emp.Sucursal.ToUpperInvariant()}"), estado, lista },
            },
        };
        ((Grid)Content).Add(cuerpo, 0, 1);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Hud.Esperando(estado);
        try
        {
            cuentas = await Servicio.Cuentas(emp.SucursalId);
            estado.Text = cuentas.Count == 0 ? "La sucursal no tiene cuentas" : $"{cuentas.Count} cuentas · toque una para operar";
            lista.Children.Clear();
            foreach (var c in cuentas) lista.Children.Add(Tarjeta(c));
        }
        catch (Exception ex) { Hud.Error(estado, ex.Message); }
    }

    View Tarjeta(Cuenta c)
    {
        var g = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) },
        };
        g.Add(Hud.Consola("■ " + c.Numero, Hud.Green, 16));
        g.Add(new Label { Text = Hud.Dinero(c.Saldo), TextColor = Hud.Orange, FontAttributes = FontAttributes.Bold, FontSize = 16 }, 1, 0);
        g.Add(new Label { Text = c.Cliente, TextColor = Hud.Text }, 0, 1);
        g.Add(Hud.Consola(c.Bloqueada ? "EN USO" : c.Estado.ToUpperInvariant(), c.Bloqueada ? Hud.Danger : Hud.Muted, 12), 1, 1);

        var caja = Hud.Caja(g, 14);
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await Navigation.PushAsync(new CuentaPage(emp, c, cuentas));
        caja.GestureRecognizers.Add(tap);
        return caja;
    }
}
