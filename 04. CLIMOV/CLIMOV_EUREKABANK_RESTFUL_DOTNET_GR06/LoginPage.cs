namespace EurekaMovil;

public class LoginPage : ContentPage
{
    readonly Entry servidor, usuario, clave;
    readonly Label estado = Hud.Consola("", Hud.Muted);

    public LoginPage()
    {
        Hud.Preparar(this);
        estado.HorizontalTextAlignment = TextAlignment.Center;
        estado.Margin = new Thickness(0, 12, 0, 0);

        var campoServidor = Hud.Campo(out servidor, Servicio.HostPorDefecto, Keyboard.Url);
        var campoUsuario = Hud.Campo(out usuario, "MONSTER");
        var campoClave = Hud.Campo(out clave, "", clave: true);
        servidor.Text = Preferences.Default.Get("host", Servicio.HostPorDefecto);

        var panel = Hud.Caja(new VerticalStackLayout
        {
            Children =
            {
                new Label { Text = "▓▓ EUREKABANK ▓▓", TextColor = Hud.Green, FontSize = 24, FontAttributes = FontAttributes.Bold,
                            CharacterSpacing = 2, HorizontalOptions = LayoutOptions.Center },
                new Label { Text = $"NODO: {Servicio.Nodo}   ■ EN LÍNEA", TextColor = Hud.Orange, FontFamily = Hud.Mono,
                            FontSize = 12, HorizontalOptions = LayoutOptions.Center },
                Hud.Etiqueta("SERVIDOR (IP:PUERTO)"), campoServidor,
                Hud.Etiqueta("USUARIO"), campoUsuario,
                Hud.Etiqueta("CLAVE"), campoClave,
                new ContentView { Content = Hud.Boton("» INGRESAR", Ingresar), Margin = new Thickness(0, 20, 0, 0) },
                estado,
            },
        }, 20);
        panel.Margin = new Thickness(16, -36, 16, 16);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Children = { new Image { Source = "hud.jpg", Aspect = Aspect.AspectFill, HeightRequest = 240 }, panel },
            },
        };
    }

    async void Ingresar(object? s, EventArgs e)
    {
        string u = usuario.Text?.Trim() ?? "", c = clave.Text ?? "";
        if (u == "" || c == "") { Hud.Error(estado, "Ingrese usuario y clave"); return; }
        Servicio.Host = servidor.Text?.Trim() ?? "";
        Preferences.Default.Set("host", Servicio.Host);

        Hud.Esperando(estado);
        try
        {
            var emp = await Servicio.Login(u, c);
            estado.Text = "";
            clave.Text = "";
            await Navigation.PushAsync(new CuentasPage(emp));
        }
        catch (Exception ex) { Hud.Error(estado, ex.Message); }
    }
}
