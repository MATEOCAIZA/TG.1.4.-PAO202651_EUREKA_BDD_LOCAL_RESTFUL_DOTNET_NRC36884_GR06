namespace EurekaMovil;

// Lo que las páginas necesitan; cada Servicio.cs convierte la respuesta de su servidor a estos tipos.
public record Empleado(int Id, string Nombre, int SucursalId, string Sucursal);
public record Cuenta(int Id, string Numero, string Cliente, decimal Saldo, string Estado, bool Bloqueada, string? BloqueadaPor);
public record Movimiento(DateTime Fecha, string Tipo, decimal Monto, decimal SaldoAnterior, decimal SaldoPosterior, string? Destino, string? Operacion);
public record Resultado(bool Exito, string Mensaje, string? Operacion);
