namespace EurekaMovil;

/// <summary>
/// Tema común EurekaBank (morado / verde neón / naranja), igual que web, escritorio, consola y Android Java.
/// Fábricas de controles ya pintados para que las páginas no repitan colores.
/// </summary>
static class Hud
{
    public static readonly Color Bg = Color.FromArgb("#0B0812"), Panel = Color.FromArgb("#E6160E24"),
        Input = Color.FromArgb("#120B1F"), Purple = Color.FromArgb("#5C2D91"), Purple2 = Color.FromArgb("#7A45B8"),
        Green = Color.FromArgb("#8FE34F"), GreenDim = Color.FromArgb("#3F6E22"), Orange = Color.FromArgb("#FF8C2E"),
        Text = Color.FromArgb("#ECE6F7"), Muted = Color.FromArgb("#A596C4"), Danger = Color.FromArgb("#FF5D6C");

    public const string Mono = "monospace";

    static Hud()
    {
        // Android dibuja una línea bajo cada Entry; el borde lo pone el Border que lo envuelve.
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("HudSinLinea", (h, v) =>
        {
#if ANDROID
            h.PlatformView.Background = null;
#endif
        });
    }

    public static void Preparar(ContentPage p)
    {
        p.BackgroundColor = Bg;
        NavigationPage.SetHasNavigationBar(p, false);
    }

    public static Label Titulo(string t, double size = 24) => new()
    {
        Text = t, TextColor = Green, FontSize = size, FontAttributes = FontAttributes.Bold, CharacterSpacing = 2,
    };

    public static Label Etiqueta(string t) => new()
    {
        Text = t, TextColor = Muted, FontSize = 13, CharacterSpacing = 1, Margin = new Thickness(0, 12, 0, 4),
    };

    public static Label Consola(string t = "", Color? color = null, double size = 13) => new()
    {
        Text = t, TextColor = color ?? Green, FontFamily = Mono, FontSize = size,
    };

    public static Border Campo(out Entry entry, string placeholder = "", Keyboard? teclado = null, bool clave = false)
    {
        entry = new Entry
        {
            Placeholder = placeholder, PlaceholderColor = Muted, TextColor = Green, FontFamily = Mono, FontSize = 16,
            Keyboard = teclado ?? Keyboard.Default, IsPassword = clave, BackgroundColor = Colors.Transparent,
        };
        var borde = new Border
        {
            Content = entry, BackgroundColor = Input, Stroke = Purple2, StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 },
            Padding = new Thickness(8, 0),
        };
        entry.Focused += (_, _) => borde.Stroke = Green;
        entry.Unfocused += (_, _) => borde.Stroke = Purple2;
        return borde;
    }

    public static Button Boton(string t, EventHandler click)
    {
        var b = new Button
        {
            Text = t, BackgroundColor = Purple, TextColor = Text, BorderColor = Green, BorderWidth = 1,
            CornerRadius = 4, FontAttributes = FontAttributes.Bold, CharacterSpacing = 1, MinimumHeightRequest = 48,
        };
        b.Clicked += click;
        b.Pressed += (_, _) => { b.BackgroundColor = Green; b.TextColor = Input; };
        b.Released += (_, _) => { b.BackgroundColor = Purple; b.TextColor = Text; };
        return b;
    }

    public static Border Caja(View contenido, double padding = 16) => new()
    {
        Content = contenido, BackgroundColor = Panel, Stroke = GreenDim, StrokeThickness = 1, Padding = padding,
        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
    };

    /// <summary>Barra morada con línea verde, como la cabecera de las webs.</summary>
    public static View Cabecera(string subtitulo, Button? derecha)
    {
        var grid = new Grid
        {
            BackgroundColor = Purple, Padding = new Thickness(14, 12),
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
        };
        grid.Add(new VerticalStackLayout
        {
            Children = { Titulo("EUREKABANK", 20), Consola(subtitulo, Orange, 12) },
        });
        if (derecha != null) grid.Add(derecha, 1);
        return new VerticalStackLayout { Children = { grid, new BoxView { HeightRequest = 2, Color = Green } } };
    }

    public static void Ok(Label l, string msg) { l.TextColor = Green; l.Text = msg; }
    public static void Error(Label l, string msg) { l.TextColor = Danger; l.Text = "✖ " + msg; }
    public static void Esperando(Label l) { l.TextColor = Muted; l.Text = "» Conectando con el servidor…"; }

    public static string Dinero(decimal d) => d.ToString("$ #,##0.00", System.Globalization.CultureInfo.InvariantCulture);
}
