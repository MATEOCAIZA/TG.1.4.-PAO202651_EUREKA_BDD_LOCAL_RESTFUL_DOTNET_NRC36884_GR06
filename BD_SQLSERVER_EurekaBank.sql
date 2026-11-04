/* EurekaBank - SQL Server (para los servidores .NET: TG.1.2 SOAP y TG.1.4 RESTful)
   Reconstruido a partir de los procedimientos que llaman Service1.svc.cs y DatabaseService.cs.
   Ejecutar:  sqlcmd -S localhost -E -C -f 65001 -i BD_SQLSERVER_EurekaBank.sql
   Usuarios de prueba: ver CREDENCIALES_Y_PRUEBAS.md */
USE master;
IF DB_ID('EurekaBank') IS NOT NULL BEGIN ALTER DATABASE EurekaBank SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE EurekaBank; END
GO
CREATE DATABASE EurekaBank;
GO
USE EurekaBank;
GO
CREATE TABLE Sucursal (SucursalID INT PRIMARY KEY, Nombre VARCHAR(50) NOT NULL);
CREATE TABLE Empleado (
  EmpleadoID INT PRIMARY KEY, Nombre VARCHAR(50) NOT NULL, Apellido VARCHAR(50) NOT NULL,
  Username VARCHAR(30) NOT NULL UNIQUE, PasswordHash VARBINARY(32) NOT NULL,
  Rol VARCHAR(20) NOT NULL, SucursalID INT NOT NULL REFERENCES Sucursal, Activo BIT NOT NULL DEFAULT 1);
CREATE TABLE Cliente (ClienteID INT PRIMARY KEY, Nombre VARCHAR(50), Apellido VARCHAR(50), Cedula VARCHAR(10));
CREATE TABLE Cuenta (
  CuentaID INT PRIMARY KEY, NumeroCuenta VARCHAR(10) NOT NULL UNIQUE, ClienteID INT NOT NULL REFERENCES Cliente,
  SucursalID INT NOT NULL REFERENCES Sucursal, Saldo DECIMAL(15,2) NOT NULL DEFAULT 0,
  Bloqueada BIT NOT NULL DEFAULT 0, BloqueadaPor INT NULL REFERENCES Empleado);
CREATE TABLE Movimiento (
  MovimientoID INT IDENTITY PRIMARY KEY, CuentaID INT NOT NULL REFERENCES Cuenta, CuentaDestinoID INT NULL REFERENCES Cuenta,
  EmpleadoID INT NOT NULL REFERENCES Empleado, Tipo VARCHAR(20) NOT NULL, Monto DECIMAL(15,2) NOT NULL,
  SaldoAnterior DECIMAL(15,2) NOT NULL, SaldoPosterior DECIMAL(15,2) NOT NULL,
  Fecha DATETIME NOT NULL DEFAULT GETDATE(), NumeroOperacion VARCHAR(20) NOT NULL);
GO
INSERT Sucursal VALUES (1,'Sipan'),(2,'Chan Chan');
INSERT Empleado (EmpleadoID,Nombre,Apellido,Username,PasswordHash,Rol,SucursalID) VALUES
 (1,'Carlos','Romero','cromero',HASHBYTES('SHA2_256','chicho'),'Cajero',1),
 (2,'Lidia','Castro','lcastro',HASHBYTES('SHA2_256','flaca'),'Cajero',2),
 (3,'MONSTER','MONSTER','MONSTER',HASHBYTES('SHA2_256','MONSTER69'),'Cajero',1);
INSERT Cliente VALUES (1,'Juan','Perez','1712345678'),(2,'Ana','Lopez','1723456789'),(3,'Luis','Mora','1734567890'),(4,'Maria','Vera','1745678901');
INSERT Cuenta (CuentaID,NumeroCuenta,ClienteID,SucursalID,Saldo) VALUES
 (1,'00100001',1,1,1500.00),(2,'00100002',2,1,820.50),(3,'00100003',3,1,4300.00),
 (4,'00200001',4,2,2500.00),(5,'00200002',1,2,300.00);
GO
CREATE PROCEDURE sp_Login @Username VARCHAR(30), @Password VARCHAR(100) AS
  SELECT e.EmpleadoID, e.Nombre, e.Apellido, e.Username, e.Rol, e.SucursalID, s.Nombre AS NombreSucursal
  FROM Empleado e JOIN Sucursal s ON s.SucursalID = e.SucursalID
  WHERE e.Username = @Username AND e.PasswordHash = HASHBYTES('SHA2_256', @Password) AND e.Activo = 1;
GO
CREATE PROCEDURE sp_ObtenerCuentasPorSucursal @SucursalID INT AS
  SELECT c.CuentaID, c.NumeroCuenta, cl.Nombre + ' ' + cl.Apellido AS ClienteNombre, cl.Cedula, c.Saldo,
         CASE WHEN c.Bloqueada = 1 THEN 'Bloqueada' ELSE 'Activa' END AS Estado, c.Bloqueada,
         ISNULL(b.Nombre + ' ' + b.Apellido, '') AS BloqueadaPorNombre
  FROM Cuenta c JOIN Cliente cl ON cl.ClienteID = c.ClienteID LEFT JOIN Empleado b ON b.EmpleadoID = c.BloqueadaPor
  WHERE c.SucursalID = @SucursalID ORDER BY c.NumeroCuenta;
GO
CREATE PROCEDURE sp_ObtenerTodasCuentasEstado AS
  SELECT c.CuentaID, c.NumeroCuenta, cl.Nombre + ' ' + cl.Apellido AS ClienteNombre, c.Saldo,
         CASE WHEN c.Bloqueada = 1 THEN 'Bloqueada' ELSE 'Activa' END AS Estado, c.Bloqueada,
         ISNULL(b.Nombre + ' ' + b.Apellido, '') AS BloqueadaPorNombre, s.Nombre AS NombreSucursal
  FROM Cuenta c JOIN Cliente cl ON cl.ClienteID = c.ClienteID JOIN Sucursal s ON s.SucursalID = c.SucursalID
  LEFT JOIN Empleado b ON b.EmpleadoID = c.BloqueadaPor ORDER BY c.NumeroCuenta;
GO
CREATE PROCEDURE sp_ObtenerMovimientos @CuentaID INT AS
  SELECT m.MovimientoID, m.Tipo, m.Monto, m.SaldoAnterior, m.SaldoPosterior, m.Fecha, m.NumeroOperacion,
         e.Nombre + ' ' + e.Apellido AS EmpleadoNombre, c.NumeroCuenta, d.NumeroCuenta AS CuentaDestinoNum
  FROM Movimiento m JOIN Cuenta c ON c.CuentaID = m.CuentaID JOIN Empleado e ON e.EmpleadoID = m.EmpleadoID
  LEFT JOIN Cuenta d ON d.CuentaID = m.CuentaDestinoID
  WHERE m.CuentaID = @CuentaID ORDER BY m.Fecha DESC, m.MovimientoID DESC;
GO
CREATE PROCEDURE sp_BloquearCuenta @CuentaID INT, @EmpleadoID INT AS
  UPDATE Cuenta SET Bloqueada = 1, BloqueadaPor = @EmpleadoID WHERE CuentaID = @CuentaID AND Bloqueada = 0;
  SELECT CASE WHEN @@ROWCOUNT = 1 THEN 1 ELSE 0 END;
GO
CREATE PROCEDURE sp_DesbloquearCuenta @CuentaID INT, @EmpleadoID INT AS
  UPDATE Cuenta SET Bloqueada = 0, BloqueadaPor = NULL WHERE CuentaID = @CuentaID AND Bloqueada = 1;
  SELECT @@ROWCOUNT;
GO
CREATE PROCEDURE sp_RegistrarMovimiento @CuentaID INT, @EmpleadoID INT, @Tipo VARCHAR(20), @Monto DECIMAL(15,2), @CuentaDestinoID INT = NULL AS
BEGIN
  SET NOCOUNT ON; SET XACT_ABORT ON;
  DECLARE @op VARCHAR(20) = 'OP' + FORMAT(GETDATE(),'yyMMddHHmmss') + RIGHT('000' + CAST(ABS(CHECKSUM(NEWID())) % 1000 AS VARCHAR),3);
  DECLARE @saldo DECIMAL(15,2), @bloq BIT, @saldoD DECIMAL(15,2), @bloqD BIT;
  IF @Monto IS NULL OR @Monto <= 0 BEGIN SELECT 0 AS Resultado, 'El monto debe ser mayor a 0' AS Mensaje, '' AS NumeroOperacion; RETURN; END
  BEGIN TRAN;
  -- Bloqueada funciona como semáforo: el cliente la bloquea antes de operar; solo frena a OTRO empleado
  SELECT @saldo = Saldo, @bloq = CASE WHEN Bloqueada = 1 AND ISNULL(BloqueadaPor, -1) <> @EmpleadoID THEN 1 ELSE 0 END FROM Cuenta WITH (UPDLOCK) WHERE CuentaID = @CuentaID;
  IF @saldo IS NULL BEGIN ROLLBACK; SELECT 0 AS Resultado, 'Cuenta no existe' AS Mensaje, '' AS NumeroOperacion; RETURN; END
  IF @bloq = 1 BEGIN ROLLBACK; SELECT 0 AS Resultado, 'La cuenta está en uso por otro empleado' AS Mensaje, '' AS NumeroOperacion; RETURN; END
  IF @Tipo = 'Deposito'
  BEGIN
    UPDATE Cuenta SET Saldo = Saldo + @Monto WHERE CuentaID = @CuentaID;
    INSERT Movimiento (CuentaID,EmpleadoID,Tipo,Monto,SaldoAnterior,SaldoPosterior,NumeroOperacion) VALUES (@CuentaID,@EmpleadoID,@Tipo,@Monto,@saldo,@saldo+@Monto,@op);
  END
  ELSE IF @Tipo IN ('Retiro','Transferencia')
  BEGIN
    IF @saldo < @Monto BEGIN ROLLBACK; SELECT 0 AS Resultado, 'Saldo insuficiente' AS Mensaje, '' AS NumeroOperacion; RETURN; END
    IF @Tipo = 'Transferencia'
    BEGIN
      SELECT @saldoD = Saldo, @bloqD = CASE WHEN Bloqueada = 1 AND ISNULL(BloqueadaPor, -1) <> @EmpleadoID THEN 1 ELSE 0 END FROM Cuenta WITH (UPDLOCK) WHERE CuentaID = @CuentaDestinoID;
      IF @saldoD IS NULL OR @CuentaDestinoID = @CuentaID BEGIN ROLLBACK; SELECT 0 AS Resultado, 'Cuenta destino inválida' AS Mensaje, '' AS NumeroOperacion; RETURN; END
      IF @bloqD = 1 BEGIN ROLLBACK; SELECT 0 AS Resultado, 'La cuenta destino está en uso por otro empleado' AS Mensaje, '' AS NumeroOperacion; RETURN; END
      UPDATE Cuenta SET Saldo = Saldo + @Monto WHERE CuentaID = @CuentaDestinoID;
      INSERT Movimiento (CuentaID,CuentaDestinoID,EmpleadoID,Tipo,Monto,SaldoAnterior,SaldoPosterior,NumeroOperacion) VALUES (@CuentaDestinoID,@CuentaID,@EmpleadoID,@Tipo,@Monto,@saldoD,@saldoD+@Monto,@op);
    END
    UPDATE Cuenta SET Saldo = Saldo - @Monto WHERE CuentaID = @CuentaID;
    INSERT Movimiento (CuentaID,CuentaDestinoID,EmpleadoID,Tipo,Monto,SaldoAnterior,SaldoPosterior,NumeroOperacion) VALUES (@CuentaID,@CuentaDestinoID,@EmpleadoID,@Tipo,@Monto,@saldo,@saldo-@Monto,@op);
  END
  ELSE BEGIN ROLLBACK; SELECT 0 AS Resultado, 'Tipo de movimiento inválido' AS Mensaje, '' AS NumeroOperacion; RETURN; END
  COMMIT;
  SELECT 1 AS Resultado, 'Operación exitosa' AS Mensaje, @op AS NumeroOperacion;
END
GO
