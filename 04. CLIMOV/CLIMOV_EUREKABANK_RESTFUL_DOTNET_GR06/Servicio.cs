using System.Net.Http.Json;
using System.Text.Json;

namespace EurekaMovil;

/// <summary>
/// Cliente de la API REST WS_EUREKABANK_RESTFUL_DOTNET_GR06 (mismas rutas que la consola: api/auth, api/cuentas, api/movimientos).
/// </summary>
static class Servicio
{
    public const string Nodo = "REST · .NET";
    /// <summary>Misma IP de Radmin VPN que usa la web. En el emulador: 10.0.2.2:8085.</summary>
    public const string HostPorDefecto = "26.243.233.245:8085";
    public static string Host = HostPorDefecto;

    static readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(30) };
    static readonly JsonSerializerOptions json = new(JsonSerializerDefaults.Web);   // camelCase, sin distinguir mayúsculas

    static string Url(string ruta) => $"http://{Host}/api/{ruta}";

    static async Task<T> Get<T>(string ruta) =>
        (await http.GetFromJsonAsync<T>(Url(ruta), json))!;

    static async Task<T> Post<T>(string ruta, object cuerpo)
    {
        var r = await http.PostAsJsonAsync(Url(ruta), cuerpo, json);
        r.EnsureSuccessStatusCode();
        return (await r.Content.ReadFromJsonAsync<T>(json))!;
    }

    record LoginDto(bool Exito, int EmpleadoID, string Nombre, string Apellido, int SucursalID, string NombreSucursal, string Mensaje);
    record CuentaDto(int CuentaID, string NumeroCuenta, string ClienteNombre, decimal Saldo, string Estado, bool Bloqueada, string? BloqueadaPorNombre);
    record MovimientoDto(DateTime Fecha, string Tipo, decimal Monto, decimal SaldoAnterior, decimal SaldoPosterior, string? CuentaDestinoNum, string? NumeroOperacion);
    record RegistroDto(int Resultado, string Mensaje, string? NumeroOperacion);

    public static async Task<Empleado> Login(string usuario, string clave)
    {
        var r = await http.PostAsJsonAsync(Url("auth/login"), new { username = usuario, password = clave }, json);
        var e = await r.Content.ReadFromJsonAsync<LoginDto>(json);
        if (e == null || !e.Exito) throw new Exception(e?.Mensaje ?? "Usuario o clave incorrectos");
        return new Empleado(e.EmpleadoID, $"{e.Nombre} {e.Apellido}", e.SucursalID, e.NombreSucursal);
    }

    public static async Task<List<Cuenta>> Cuentas(int sucursalId) =>
        (await Get<List<CuentaDto>>($"cuentas/sucursal/{sucursalId}"))
        .Select(x => new Cuenta(x.CuentaID, x.NumeroCuenta, x.ClienteNombre, x.Saldo, x.Estado, x.Bloqueada, x.BloqueadaPorNombre))
        .ToList();

    public static async Task<List<Movimiento>> Movimientos(int cuentaId) =>
        (await Get<List<MovimientoDto>>($"movimientos/cuenta/{cuentaId}"))
        .Select(m => new Movimiento(m.Fecha, m.Tipo, m.Monto, m.SaldoAnterior, m.SaldoPosterior, m.CuentaDestinoNum, m.NumeroOperacion))
        .ToList();

    /// <summary>Igual que la consola: bloquea la cuenta, registra y la desbloquea siempre.</summary>
    public static async Task<Resultado> Registrar(int cuentaId, int empleadoId, string tipo, decimal monto, int destinoId)
    {
        var llave = new { cuentaID = cuentaId, empleadoID = empleadoId };
        if (!await Post<bool>("cuentas/bloquear", llave))
            return new Resultado(false, "La cuenta está en uso por otro cajero. Intente más tarde.", null);
        try
        {
            var r = await Post<RegistroDto>("movimientos/registrar", new
            {
                cuentaID = cuentaId, empleadoID = empleadoId, tipo, monto,
                cuentaDestinoID = destinoId == 0 ? (int?)null : destinoId,
            });
            return new Resultado(r.Resultado == 1, r.Mensaje, r.NumeroOperacion);
        }
        finally
        {
            await Post<bool>("cuentas/desbloquear", llave);
        }
    }
}
