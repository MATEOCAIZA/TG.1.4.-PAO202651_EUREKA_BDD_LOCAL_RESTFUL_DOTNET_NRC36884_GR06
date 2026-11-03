using System;
using System.Collections.Generic;
using CLICON_EUREKABANK_RESTFUL_DOTNET_GR01.Models;
using CLICON_EUREKABANK_RESTFUL_DOTNET_GR01.Services;

namespace CLICON_EUREKABANK_RESTFUL_DOTNET_GR01
{
    internal class Program
    {
        private static EurekaBankService _service;
        private static LoginResponse _empleado;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            _service = new EurekaBankService();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║     🏦   EUREKA BANK  —  CLI  v1.0   🏦      ║");
            Console.WriteLine("║       Sistema de Gestión Bancaria            ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.ResetColor();

            // ── LOGIN ─────────────────────────────────────────────────────────
            bool autenticado = false;
            int intentos = 0;
            while (!autenticado && intentos < 3)
            {
                Console.WriteLine();
                Console.Write("  Usuario   : ");
                string user = Console.ReadLine();
                Console.Write("  Contraseña: ");
                string pass = LeerPassword();
                Console.WriteLine();

                try
                {
                    var resultado = _service.Login(user, pass);
                    if (resultado != null && resultado.Exito)
                    {
                        _empleado = resultado;
                        autenticado = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n  ✔  Bienvenido, {_empleado.Nombre} {_empleado.Apellido}");
                        Console.WriteLine($"     Rol: {_empleado.Rol}  |  Sucursal: {_empleado.NombreSucursal}");
                        Console.ResetColor();
                    }
                    else
                    {
                        intentos++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  ✗  {resultado?.Mensaje ?? "Error al conectar"} ({intentos}/3)");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Error de conexión: " + ex.Message);
                    Console.ResetColor();
                    intentos++;
                }
            }

            if (!autenticado)
            {
                Console.WriteLine("\n  Demasiados intentos fallidos. Saliendo...");
                Console.ReadKey();
                return;
            }

            // ── MENÚ PRINCIPAL ────────────────────────────────────────────────
            bool salir = false;
            while (!salir)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ┌─────────────────────────────────┐");
                Console.WriteLine("  │         MENÚ PRINCIPAL          │");
                Console.WriteLine("  ├─────────────────────────────────┤");
                Console.WriteLine("  │  1. Ver cuentas de mi sucursal  │");
                Console.WriteLine("  │  2. Registrar transacción       │");
                Console.WriteLine("  │  3. Ver historial de movimientos│");
                Console.WriteLine("  │  4. MashUp - Estado en tiempo   │");
                Console.WriteLine("  │     real de todas las cuentas   │");
                Console.WriteLine("  │  5. Salir                       │");
                Console.WriteLine("  └─────────────────────────────────┘");
                Console.ResetColor();
                Console.Write("  Opción: ");
                string opcion = Console.ReadLine();

                switch (opcion?.Trim())
                {
                    case "1": VerCuentas(); break;
                    case "2": RegistrarTransaccion(); break;
                    case "3": VerHistorial(); break;
                    case "4": VerMashUp(); break;
                    case "5": salir = true; break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("  Opción inválida.");
                        Console.ResetColor();
                        break;
                }
            }

            Console.WriteLine("\n  Hasta luego. Sesión cerrada.");
            Console.ReadKey();
        }

        // ── OPCIONES ──────────────────────────────────────────────────────────

        static void VerCuentas()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ═══ CUENTAS — {_empleado.NombreSucursal} ═══");
            Console.ResetColor();

            var cuentas = _service.ObtenerCuentasPorSucursal(_empleado.SucursalID);
            if (cuentas.Count == 0) { Console.WriteLine("  Sin cuentas."); return; }

            Console.WriteLine($"  {"N° Cuenta",-15} {"Titular",-25} {"Saldo",12}  Estado");
            Console.WriteLine("  " + new string('─', 70));
            foreach (var c in cuentas)
            {
                Console.ForegroundColor = c.Bloqueada ? ConsoleColor.Red : ConsoleColor.Green;
                string estado = c.Bloqueada ? $"BLOQUEADA ({c.BloqueadaPorNombre})" : "DISPONIBLE";
                Console.WriteLine($"  {c.NumeroCuenta,-15} {c.ClienteNombre,-25} {("$" + c.Saldo.ToString("N2")),12}  {estado}");
                Console.ResetColor();
            }
        }

        static void RegistrarTransaccion()
        {
            VerCuentas();
            var cuentas = _service.ObtenerCuentasPorSucursal(_empleado.SucursalID);
            Console.WriteLine();
            Console.Write("  N° de cuenta a operar: ");
            string numOrigen = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(numOrigen)) { Console.WriteLine("  Número de cuenta inválido."); return; }
            var cuentaOrigen = cuentas.Find(c => c.NumeroCuenta == numOrigen);
            if (cuentaOrigen == null) { Console.WriteLine("  Cuenta no encontrada en su sucursal."); return; }
            int cuentaId = cuentaOrigen.CuentaID;

            Console.WriteLine("  Tipo: (1) Depósito  (2) Retiro  (3) Transferencia");
            Console.Write("  Opción: ");
            string tipoOp = Console.ReadLine();
            string tipo = tipoOp == "1" ? "Deposito" : tipoOp == "2" ? "Retiro" : tipoOp == "3" ? "Transferencia" : null;
            if (tipo == null) { Console.WriteLine("  Tipo inválido."); return; }

            Console.Write("  Monto ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal monto) || monto <= 0) { Console.WriteLine("  Monto inválido."); return; }

            int? cuentaDestino = null;
            if (tipo == "Transferencia")
            {
                Console.Write("  N° Cuenta Destino: ");
                string numDestino = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(numDestino)) { Console.WriteLine("  Número de cuenta destino inválido."); return; }
                var todasCuentas = _service.ObtenerTodasCuentasEstado();
                var cuentaDest = todasCuentas.Find(c => c.NumeroCuenta == numDestino);
                if (cuentaDest == null) { Console.WriteLine("  Cuenta destino no encontrada."); return; }
                cuentaDestino = cuentaDest.CuentaID;
            }

            Console.Write("  ¿Confirmar operación? (s/n): ");
            if (Console.ReadLine()?.ToLower() != "s") { Console.WriteLine("  Operación cancelada."); return; }

            _service.BloquearCuenta(cuentaId, _empleado.EmpleadoID);
            var resultado = _service.RegistrarMovimiento(cuentaId, _empleado.EmpleadoID, tipo, monto, cuentaDestino);
            _service.DesbloquearCuenta(cuentaId, _empleado.EmpleadoID);

            if (resultado.Resultado == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  ✔  {resultado.Mensaje}");
                Console.WriteLine($"     N° Operación: {resultado.NumeroOperacion}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  ✗  {resultado.Mensaje}");
            }
            Console.ResetColor();
        }

        static void VerHistorial()
        {
            VerCuentas();
            var cuentas = _service.ObtenerCuentasPorSucursal(_empleado.SucursalID);
            Console.Write("\n  N° de cuenta: ");
            string numCuenta = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(numCuenta)) { Console.WriteLine("  Número de cuenta inválido."); return; }
            var cuenta = cuentas.Find(c => c.NumeroCuenta == numCuenta);
            if (cuenta == null) { Console.WriteLine("  Cuenta no encontrada."); return; }
            int cuentaId = cuenta.CuentaID;

            var movimientos = _service.ObtenerMovimientos(cuentaId);
            if (movimientos.Count == 0) { Console.WriteLine("  Sin movimientos."); return; }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ═══ HISTORIAL — Últimos {movimientos.Count} movimientos ═══");
            Console.ResetColor();
            Console.WriteLine($"  {"N° Op",-14} {"Fecha",-18} {"Tipo",-14} {"Monto",12}  {"Saldo Post.",12}");
            Console.WriteLine("  " + new string('─', 75));

            foreach (var m in movimientos)
            {
                Console.ForegroundColor = m.Tipo == "Deposito" ? ConsoleColor.Green :
                                          m.Tipo == "Retiro"   ? ConsoleColor.Red   : ConsoleColor.Cyan;
                Console.WriteLine($"  {m.NumeroOperacion,-14} {m.Fecha:dd/MM/yyyy HH:mm,-18} {m.Tipo,-14} {("$"+m.Monto.ToString("N2")),12}  {("$"+m.SaldoPosterior.ToString("N2")),12}");
                Console.ResetColor();
            }
        }

        static void VerMashUp()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ═══ MASHUP — ESTADO EN TIEMPO REAL ═══");
            Console.ResetColor();

            var cuentas = _service.ObtenerTodasCuentasEstado();
            Console.WriteLine($"\n  {"N° Cuenta",-15} {"Titular",-25} {"Sucursal",-20} {"Saldo",12}  Estado");
            Console.WriteLine("  " + new string('─', 85));
            foreach (var c in cuentas)
            {
                Console.ForegroundColor = c.Bloqueada ? ConsoleColor.Red : ConsoleColor.Green;
                string semaforo = c.Bloqueada ? "🔴 BLOQUEADA" : "🟢 DISPONIBLE";
                Console.WriteLine($"  {c.NumeroCuenta,-15} {c.ClienteNombre,-25} {c.NombreSucursal,-20} {("$"+c.Saldo.ToString("N2")),12}  {semaforo}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            int libres = cuentas.FindAll(c => !c.Bloqueada).Count;
            Console.ForegroundColor = ConsoleColor.Red;
            int bloqueadas = cuentas.FindAll(c => c.Bloqueada).Count;
            Console.ResetColor();
            Console.WriteLine($"\n  Total: {cuentas.Count} cuentas  |  🟢 {libres} libres  |  🔴 {bloqueadas} bloqueadas");
        }

        static string LeerPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);
            return pass;
        }
    }
}
