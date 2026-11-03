using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Logging;
using WS_EUREKABANK_RESTFUL_DOTNET_GR06.Models;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["EurekaBank"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                LogService.Error("Connection string 'EurekaBank' no encontrada en Web.config.");
                throw new InvalidOperationException("Missing connection string 'EurekaBank' in Web.config.");
            }

            LogService.Info($"DatabaseService inicializado. DataSource: {ExtractDataSource(_connectionString)}");
        }

        private static string ExtractDataSource(string cs)
        {
            // Log only the Data Source, never credentials
            foreach (string part in cs.Split(';'))
            {
                if (part.TrimStart().StartsWith("Data Source", StringComparison.OrdinalIgnoreCase))
                    return part.Trim();
            }
            return "(desconocido)";
        }

        public LoginResponse Login(string username, string password)
        {
            LogService.Info($"sp_Login -> username='{username}'");
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_Login", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var resp = new LoginResponse
                            {
                                Exito          = true,
                                EmpleadoID     = rdr.GetInt32(0),
                                Nombre         = rdr.GetString(1),
                                Apellido       = rdr.GetString(2),
                                Username       = rdr.GetString(3),
                                Rol            = rdr.GetString(4),
                                SucursalID     = rdr.GetInt32(5),
                                NombreSucursal = rdr.GetString(6),
                                Mensaje        = "Login exitoso"
                            };
                            LogService.Info($"sp_Login OK -> EmpleadoID={resp.EmpleadoID}, Rol={resp.Rol}");
                            return resp;
                        }
                    }
                }

                LogService.Warning($"sp_Login -> credenciales incorrectas para '{username}'");
                return new LoginResponse { Exito = false, Mensaje = "Credenciales incorrectas" };
            }
            catch (Exception ex)
            {
                LogService.Error($"sp_Login FALLO para '{username}'", ex);
                throw;
            }
        }

        public List<CuentaDto> ObtenerCuentasPorSucursal(int sucursalId)
        {
            LogService.Info($"sp_ObtenerCuentasPorSucursal -> SucursalID={sucursalId}");
            try
            {
                var lista = new List<CuentaDto>();
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_ObtenerCuentasPorSucursal", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@SucursalID", sucursalId);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new CuentaDto
                            {
                                CuentaID           = rdr.GetInt32(0),
                                NumeroCuenta       = rdr.GetString(1),
                                ClienteNombre      = rdr.GetString(2),
                                Cedula             = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                                Saldo              = rdr.GetDecimal(4),
                                Estado             = rdr.GetString(5),
                                Bloqueada          = rdr.GetBoolean(6),
                                BloqueadaPorNombre = rdr.IsDBNull(7) ? "" : rdr.GetString(7)
                            });
                        }
                    }
                }
                LogService.Info($"sp_ObtenerCuentasPorSucursal OK -> {lista.Count} cuentas");
                return lista;
            }
            catch (Exception ex)
            {
                LogService.Error($"sp_ObtenerCuentasPorSucursal FALLO SucursalID={sucursalId}", ex);
                throw;
            }
        }

        public List<CuentaDto> ObtenerTodasCuentasEstado()
        {
            LogService.Info("sp_ObtenerTodasCuentasEstado");
            try
            {
                var lista = new List<CuentaDto>();
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_ObtenerTodasCuentasEstado", conn) { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new CuentaDto
                            {
                                CuentaID           = rdr.GetInt32(0),
                                NumeroCuenta       = rdr.GetString(1),
                                ClienteNombre      = rdr.GetString(2),
                                Saldo              = rdr.GetDecimal(3),
                                Estado             = rdr.GetString(4),
                                Bloqueada          = rdr.GetBoolean(5),
                                BloqueadaPorNombre = rdr.IsDBNull(6) ? "" : rdr.GetString(6),
                                NombreSucursal     = rdr.GetString(7)
                            });
                        }
                    }
                }
                LogService.Info($"sp_ObtenerTodasCuentasEstado OK -> {lista.Count} cuentas");
                return lista;
            }
            catch (Exception ex)
            {
                LogService.Error("sp_ObtenerTodasCuentasEstado FALLO", ex);
                throw;
            }
        }

        public bool BloquearCuenta(int cuentaId, int empleadoId)
        {
            LogService.Info($"sp_BloquearCuenta -> CuentaID={cuentaId}, EmpleadoID={empleadoId}");
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_BloquearCuenta", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@CuentaID", cuentaId);
                    cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            bool resultado = rdr.GetInt32(0) == 1;
                            LogService.Info($"sp_BloquearCuenta -> resultado={resultado}");
                            return resultado;
                        }
                    }
                }
                LogService.Warning($"sp_BloquearCuenta sin filas -> CuentaID={cuentaId}");
                return false;
            }
            catch (Exception ex)
            {
                LogService.Error($"sp_BloquearCuenta FALLO CuentaID={cuentaId}", ex);
                throw;
            }
        }

        public bool DesbloquearCuenta(int cuentaId, int empleadoId)
        {
            LogService.Info($"sp_DesbloquearCuenta -> CuentaID={cuentaId}, EmpleadoID={empleadoId}");
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_DesbloquearCuenta", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@CuentaID", cuentaId);
                    cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            bool resultado = rdr.GetInt32(0) > 0;
                            LogService.Info($"sp_DesbloquearCuenta -> resultado={resultado}");
                            return resultado;
                        }
                    }
                }
                LogService.Warning($"sp_DesbloquearCuenta sin filas -> CuentaID={cuentaId}");
                return false;
            }
            catch (Exception ex)
            {
                LogService.Error($"sp_DesbloquearCuenta FALLO CuentaID={cuentaId}", ex);
                throw;
            }
        }

        public List<MovimientoDto> ObtenerMovimientos(int cuentaId)
        {
            LogService.Info($"sp_ObtenerMovimientos -> CuentaID={cuentaId}");
            try
            {
                var lista = new List<MovimientoDto>();
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_ObtenerMovimientos", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@CuentaID", cuentaId);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new MovimientoDto
                            {
                                MovimientoID     = rdr.GetInt32(0),
                                Tipo             = rdr.GetString(1),
                                Monto            = rdr.GetDecimal(2),
                                SaldoAnterior    = rdr.GetDecimal(3),
                                SaldoPosterior   = rdr.GetDecimal(4),
                                Fecha            = rdr.GetDateTime(5),
                                NumeroOperacion  = rdr.GetString(6),
                                EmpleadoNombre   = rdr.GetString(7),
                                NumeroCuenta     = rdr.GetString(8),
                                CuentaDestinoNum = rdr.IsDBNull(9) ? "" : rdr.GetString(9)
                            });
                        }
                    }
                }
                LogService.Info($"sp_ObtenerMovimientos OK -> {lista.Count} movimientos");
                return lista;
            }
            catch (Exception ex)
            {
                LogService.Error($"sp_ObtenerMovimientos FALLO CuentaID={cuentaId}", ex);
                throw;
            }
        }

        public RegistrarMovimientoResponse RegistrarMovimiento(RegistrarMovimientoRequest req)
        {
            LogService.Info($"sp_RegistrarMovimiento -> CuentaID={req.CuentaID}, Tipo={req.Tipo}, Monto={req.Monto}, EmpleadoID={req.EmpleadoID}");
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_RegistrarMovimiento", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@CuentaID",        req.CuentaID);
                    cmd.Parameters.AddWithValue("@EmpleadoID",      req.EmpleadoID);
                    cmd.Parameters.AddWithValue("@Tipo",            req.Tipo);
                    cmd.Parameters.AddWithValue("@Monto",           req.Monto);
                    cmd.Parameters.AddWithValue("@CuentaDestinoID", (object)req.CuentaDestinoID ?? DBNull.Value);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var resp = new RegistrarMovimientoResponse
                            {
                                Resultado       = rdr.GetInt32(0),
                                Mensaje         = rdr.GetString(1),
                                NumeroOperacion = rdr.GetString(2)
                            };
                            LogService.Info($"sp_RegistrarMovimiento OK -> Resultado={resp.Resultado}, Op={resp.NumeroOperacion}");
                            return resp;
                        }
                    }
                }

                LogService.Warning($"sp_RegistrarMovimiento sin filas -> CuentaID={req.CuentaID}");
                return new RegistrarMovimientoResponse { Resultado = -99, Mensaje = "Sin respuesta del servidor" };
            }
            catch (Exception ex)
            {
                LogService.Error($"sp_RegistrarMovimiento FALLO CuentaID={req.CuentaID}", ex);
                throw;
            }
        }
    }
}
