using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Tema común EurekaBank (morado / verde neón / naranja), igual en los 4 clientes.
/// Uso: HudTheme.Apply(this) en el constructor del formulario; se pinta en Load,
/// después de que el formulario puso sus propios colores, y alcanza a los controles agregados luego.
/// </summary>
public static class HudTheme
{
    public static readonly Color Bg = Color.FromArgb(0x0B, 0x08, 0x12), Panel = Color.FromArgb(0x16, 0x0E, 0x24),
        Input = Color.FromArgb(0x12, 0x0B, 0x1F), Purple = Color.FromArgb(0x5C, 0x2D, 0x91), Purple2 = Color.FromArgb(0x7A, 0x45, 0xB8),
        Green = Color.FromArgb(0x8F, 0xE3, 0x4F), GreenDim = Color.FromArgb(0x3F, 0x6E, 0x22), Orange = Color.FromArgb(0xFF, 0x8C, 0x2E),
        Text = Color.FromArgb(0xEC, 0xE6, 0xF7);

    static readonly Font UiFont = new Font("Segoe UI", 10f), MonoFont = new Font("Consolas", 10.5f);
    static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Control, object> Done =
        new System.Runtime.CompilerServices.ConditionalWeakTable<Control, object>();

    /// <summary>"SOAP · .NET" o "REST · .NET" según el proyecto (nombre del ensamblado), para distinguirlos con los dos abiertos.</summary>
    public static readonly string Nodo =
        (System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "").ToUpperInvariant().Contains("SOAP") ? "SOAP · .NET" : "REST · .NET";

    public static void Apply(Form f)
    {
        f.Load += (s, e) => { Style(f); Marcar(f); };
    }

    // Prefijo en el título y distintivo naranja arriba a la derecha
    static void Marcar(Form f)
    {
        if (!f.Text.Contains(Nodo)) f.Text = "[" + Nodo + "] " + f.Text;
        var marca = new Label
        {
            Text = "■ " + Nodo, AutoSize = true, ForeColor = Orange, BackColor = Input,
            Font = new Font(MonoFont, FontStyle.Bold), BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(4, 2, 4, 2), Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        Done.Add(marca, null);   // que Style no lo repinte
        f.Controls.Add(marca);
        marca.Location = new Point(f.ClientSize.Width - marca.PreferredWidth - 8, 6);
        marca.BringToFront();
    }

    static void Style(Control c)
    {
        object _;
        if (Done.TryGetValue(c, out _)) return;   // ya pintado
        Done.Add(c, null);

        if (c is Form)
        {
            c.BackColor = Panel; c.ForeColor = Text;
        }
        else if (c is Button)
        {
            var b = (Button)c;
            b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderColor = Green; b.FlatAppearance.BorderSize = 1;
            b.BackColor = Purple; b.ForeColor = Text; b.Font = new Font(UiFont, FontStyle.Bold); b.Cursor = Cursors.Hand;
            b.MouseEnter += (s, e) => { b.BackColor = Green; b.ForeColor = Input; };
            b.MouseLeave += (s, e) => { b.BackColor = Purple; b.ForeColor = Text; };
        }
        else if (c is TextBox || c is ComboBox || c is NumericUpDown || c is MaskedTextBox)
        {
            c.BackColor = Input; c.ForeColor = Green; c.Font = MonoFont;
            if (c is TextBox) ((TextBox)c).BorderStyle = BorderStyle.FixedSingle;
            if (c is ComboBox) ((ComboBox)c).FlatStyle = FlatStyle.Flat;
        }
        else if (c is DataGridView)
        {
            var g = (DataGridView)c;
            g.BackgroundColor = Input; g.GridColor = GreenDim; g.BorderStyle = BorderStyle.FixedSingle;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Purple; g.ColumnHeadersDefaultCellStyle.ForeColor = Green;
            g.ColumnHeadersDefaultCellStyle.Font = new Font(UiFont, FontStyle.Bold);
            g.DefaultCellStyle.BackColor = Input; g.DefaultCellStyle.ForeColor = Text; g.DefaultCellStyle.Font = MonoFont;
            g.DefaultCellStyle.SelectionBackColor = Purple2; g.DefaultCellStyle.SelectionForeColor = Text;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(0x1C, 0x12, 0x2E);
            g.RowHeadersDefaultCellStyle.BackColor = Purple;
            // filas que el formulario pinta con fondos claros (estado): fondo oscuro y el estado pasa al color del texto
            g.CellFormatting += (s, e) =>
            {
                Color bg = e.CellStyle.BackColor;
                if (bg.IsEmpty || bg.GetBrightness() < 0.6f) return;
                e.CellStyle.ForeColor = bg.G > bg.R && bg.G > bg.B ? Green : bg.R > bg.G ? Color.FromArgb(0xFF, 0x5D, 0x6C) : Orange;
                e.CellStyle.BackColor = Input;
                e.CellStyle.SelectionBackColor = Purple2; e.CellStyle.SelectionForeColor = Text;
            };
        }
        else if (c is ListBox || c is ListView || c is RichTextBox)
        {
            c.BackColor = Input; c.ForeColor = Text; c.Font = MonoFont;
        }
        else if (c is Label)
        {
            bool titulo = c.Font.Size >= 13;
            c.ForeColor = titulo ? Green : Text;
            if (titulo) c.Font = new Font(c.Font, FontStyle.Bold);
            if (c.BackColor != Color.Transparent && c.BackColor != c.Parent?.BackColor && c.BackColor != SystemColors.Control) c.BackColor = Purple;
        }
        else if (c is GroupBox)
        {
            c.BackColor = Panel; c.ForeColor = Green;
        }
        else if (c is PictureBox)
        {
            c.BackColor = Panel;
        }
        else if (c is ToolStrip)
        {
            c.BackColor = Purple; c.ForeColor = Text;
        }
        else if (c is Panel || c is TableLayoutPanel || c is FlowLayoutPanel || c is TabPage || c is SplitContainer || c is UserControl)
        {
            c.BackColor = c.Dock == DockStyle.Top ? Purple : Panel;   // franja superior = cabecera
            c.ForeColor = Text;
        }

        foreach (Control k in c.Controls) Style(k);
        c.ControlAdded += (s, e) => Style(e.Control);
    }
}
