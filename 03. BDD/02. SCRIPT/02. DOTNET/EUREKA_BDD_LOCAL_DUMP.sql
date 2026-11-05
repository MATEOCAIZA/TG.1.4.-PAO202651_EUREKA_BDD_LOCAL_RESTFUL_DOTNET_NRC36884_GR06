-- ============================================================
--  EUREKA BANK - SQL Server Database Dump
--  Instrucciones para levantar la base de datos:
--
--  1. Abrir SQL Server Management Studio (SSMS)
--  2. Conectarse a: localhost (o .\SQLEXPRESS para Express)
--  3. Hacer clic en "Nueva consulta"
--  4. Abrir este archivo: Archivo > Abrir > Archivo...
--  5. Presionar F5 o clic en "Ejecutar"
--  6. La base de datos EurekaBank sera creada automaticamente
--
--  Nota: Si el servidor usa autenticacion SQL, cambia la
--  cadena de conexion en los Web.config / App.config:
--  "Data Source=localhost;Initial Catalog=EurekaBank;
--   User Id=sa;Password=TuPassword;"
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'EurekaBank')
BEGIN
    ALTER DATABASE EurekaBank SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EurekaBank;
END
GO

CREATE DATABASE EurekaBank;
GO

USE EurekaBank;
GO

-- ============================================================
-- TABLAS
-- ============================================================

CREATE TABLE Sucursales (
    SucursalID   INT           PRIMARY KEY IDENTITY(1,1),
    Nombre       NVARCHAR(100) NOT NULL,
    Direccion    NVARCHAR(200) NOT NULL,
    Ciudad       NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Empleados (
    EmpleadoID   INT           PRIMARY KEY IDENTITY(1,1),
    Nombre       NVARCHAR(100) NOT NULL,
    Apellido     NVARCHAR(100) NOT NULL,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    Password     NVARCHAR(100) NOT NULL,
    Rol          NVARCHAR(50)  NOT NULL,   -- 'SuperAdmin', 'Gerente', 'Cajero'
    SucursalID   INT           NOT NULL REFERENCES Sucursales(SucursalID),
    Estado       BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE Clientes (
    ClienteID    INT           PRIMARY KEY IDENTITY(1,1),
    Nombre       NVARCHAR(100) NOT NULL,
    Apellido     NVARCHAR(100) NOT NULL,
    Cedula       NVARCHAR(20)  NOT NULL UNIQUE,
    Email        NVARCHAR(100),
    Telefono     NVARCHAR(20),
    Estado       BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE Cuentas (
    CuentaID       INT            PRIMARY KEY IDENTITY(1,1),
    NumeroCuenta   NVARCHAR(20)   NOT NULL UNIQUE,
    ClienteID      INT            NOT NULL REFERENCES Clientes(ClienteID),
    SucursalID     INT            NOT NULL REFERENCES Sucursales(SucursalID),
    Saldo          DECIMAL(18,2)  NOT NULL DEFAULT 0,
    Estado         NVARCHAR(20)   NOT NULL DEFAULT 'Activa',  -- 'Activa','Inactiva'
    Bloqueada      BIT            NOT NULL DEFAULT 0,
    BloqueadaPor   INT            NULL     REFERENCES Empleados(EmpleadoID)
);
GO

CREATE TABLE Movimientos (
    MovimientoID      INT            PRIMARY KEY IDENTITY(1,1),
    CuentaID          INT            NOT NULL REFERENCES Cuentas(CuentaID),
    CuentaDestinoID   INT            NULL     REFERENCES Cuentas(CuentaID),
    EmpleadoID        INT            NOT NULL REFERENCES Empleados(EmpleadoID),
    Tipo              NVARCHAR(20)   NOT NULL,  -- 'Deposito','Retiro','Transferencia'
    Monto             DECIMAL(18,2)  NOT NULL,
    SaldoAnterior     DECIMAL(18,2)  NOT NULL,
    SaldoPosterior    DECIMAL(18,2)  NOT NULL,
    Fecha             DATETIME       NOT NULL DEFAULT GETDATE(),
    NumeroOperacion   NVARCHAR(30)   NOT NULL
);
GO

-- ============================================================
-- DATOS INICIALES - SUCURSALES
-- ============================================================

INSERT INTO Sucursales (Nombre, Direccion, Ciudad) VALUES
    ('Sucursal Central',    'Av. Principal 100',    'Quito'),
    ('Sucursal Norte',      'Av. del Parque 250',   'Quito'),
    ('Sucursal Sur',        'Calle 15 Sur 45',      'Guayaquil'),
    ('Sucursal Occidente',  'Av. Occidental 780',   'Cuenca');
GO

-- ============================================================
-- DATOS INICIALES - EMPLEADOS
-- Passwords estan en texto plano para la demo.
-- En produccion usar hash (SHA256 o bcrypt).
-- SuperAdmin: Monster / monster123
-- Cajeros: cajero1..cajero4 / cajero123
-- ============================================================

INSERT INTO Empleados (Nombre, Apellido, Username, Password, Rol, SucursalID) VALUES
    ('Monster',    'Admin',     'monster',  'monster123', 'SuperAdmin', 1),
    ('Carlos',     'Mendoza',   'cmendoza', 'gerente123', 'Gerente',    1),
    ('Maria',      'Lopez',     'mlopez',   'gerente123', 'Gerente',    2),
    ('Juan',       'Perez',     'jperez',   'cajero123',  'Cajero',     1),
    ('Ana',        'Torres',    'atorres',  'cajero123',  'Cajero',     1),
    ('Luis',       'Ramirez',   'lramirez', 'cajero123',  'Cajero',     2),
    ('Sofia',      'Castro',    'scastro',  'cajero123',  'Cajero',     3),
    ('Diego',      'Vargas',    'dvargas',  'cajero123',  'Cajero',     4);
GO

-- ============================================================
-- DATOS INICIALES - CLIENTES
-- ============================================================

INSERT INTO Clientes (Nombre, Apellido, Cedula, Email, Telefono) VALUES
    ('Roberto',   'Aguirre',   '1712345678', 'raguirre@mail.com',   '0991234567'),
    ('Lucia',     'Benalcazar','1723456789', 'lbenalcazar@mail.com','0992345678'),
    ('Fernando',  'Calderon',  '1734567890', 'fcalderon@mail.com',  '0993456789'),
    ('Patricia',  'Dominguez', '1745678901', 'pdominguez@mail.com', '0994567890'),
    ('Andres',    'Espinoza',  '1756789012', 'aespinoza@mail.com',  '0995678901'),
    ('Carmen',    'Flores',    '1767890123', 'cflores@mail.com',    '0996789012'),
    ('Miguel',    'Guerrero',  '1778901234', 'mguerrero@mail.com',  '0997890123'),
    ('Isabel',    'Herrera',   '1789012345', 'iherrera@mail.com',   '0998901234');
GO

-- ============================================================
-- DATOS INICIALES - CUENTAS
-- ============================================================

INSERT INTO Cuentas (NumeroCuenta, ClienteID, SucursalID, Saldo) VALUES
    ('2200100001', 1, 1, 5000.00),
    ('2200100002', 2, 1, 12500.50),
    ('2200100003', 3, 2, 800.00),
    ('2200100004', 4, 2, 32000.75),
    ('2200100005', 5, 3, 1500.00),
    ('2200100006', 6, 3, 9800.25),
    ('2200100007', 7, 4, 450.00),
    ('2200100008', 8, 4, 67000.00);
GO

-- ============================================================
-- DATOS INICIALES - MOVIMIENTOS (historial)
-- ============================================================

INSERT INTO Movimientos (CuentaID, EmpleadoID, Tipo, Monto, SaldoAnterior, SaldoPosterior, Fecha, NumeroOperacion) VALUES
    (1, 4, 'Deposito',     2000.00, 3000.00, 5000.00,  DATEADD(day,-10,GETDATE()), 'OP-00000001'),
    (1, 4, 'Retiro',        500.00, 5500.00, 5000.00,  DATEADD(day,-5, GETDATE()), 'OP-00000002'),
    (2, 5, 'Deposito',     5000.00, 7500.50,12500.50,  DATEADD(day,-8, GETDATE()), 'OP-00000003'),
    (3, 6, 'Deposito',      800.00,    0.00,  800.00,  DATEADD(day,-3, GETDATE()), 'OP-00000004'),
    (4, 4, 'Deposito',    10000.00,22000.75,32000.75,  DATEADD(day,-2, GETDATE()), 'OP-00000005'),
    (5, 7, 'Retiro',        200.00, 1700.00, 1500.00,  DATEADD(day,-1, GETDATE()), 'OP-00000006'),
    (6, 7, 'Deposito',     1500.00, 8300.25, 9800.25,  DATEADD(hour,-6,GETDATE()),'OP-00000007'),
    (8, 8, 'Deposito',    50000.00,17000.00,67000.00,  DATEADD(hour,-2,GETDATE()),'OP-00000008');
GO

-- ============================================================
-- STORED PROCEDURE: Login
-- ============================================================

CREATE OR ALTER PROCEDURE sp_Login
    @Username NVARCHAR(50),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT e.EmpleadoID, e.Nombre, e.Apellido, e.Username, e.Rol, e.SucursalID,
           s.Nombre AS NombreSucursal
    FROM   Empleados e
    INNER JOIN Sucursales s ON s.SucursalID = e.SucursalID
    WHERE  e.Username = @Username
      AND  e.Password = @Password
      AND  e.Estado   = 1;
END
GO

-- ============================================================
-- STORED PROCEDURE: ObtenerCuentasPorSucursal
-- ============================================================

CREATE OR ALTER PROCEDURE sp_ObtenerCuentasPorSucursal
    @SucursalID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.CuentaID, c.NumeroCuenta,
           cl.Nombre + ' ' + cl.Apellido AS ClienteNombre,
           cl.Cedula,
           c.Saldo, c.Estado, c.Bloqueada,
           ISNULL(e.Nombre + ' ' + e.Apellido, '') AS BloqueadaPorNombre
    FROM   Cuentas c
    INNER JOIN Clientes cl   ON cl.ClienteID  = c.ClienteID
    LEFT  JOIN Empleados e   ON e.EmpleadoID  = c.BloqueadaPor
    WHERE  c.SucursalID = @SucursalID
    ORDER BY c.NumeroCuenta;
END
GO

-- ============================================================
-- STORED PROCEDURE: ObtenerMovimientos
-- ============================================================

CREATE OR ALTER PROCEDURE sp_ObtenerMovimientos
    @CuentaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 50
           m.MovimientoID, m.Tipo, m.Monto,
           m.SaldoAnterior, m.SaldoPosterior,
           m.Fecha, m.NumeroOperacion,
           e.Nombre + ' ' + e.Apellido AS EmpleadoNombre,
           c.NumeroCuenta,
           ISNULL(cd.NumeroCuenta,'') AS CuentaDestinoNum
    FROM   Movimientos m
    INNER JOIN Empleados e ON e.EmpleadoID = m.EmpleadoID
    INNER JOIN Cuentas   c ON c.CuentaID   = m.CuentaID
    LEFT  JOIN Cuentas  cd ON cd.CuentaID  = m.CuentaDestinoID
    WHERE  m.CuentaID = @CuentaID
    ORDER BY m.Fecha DESC;
END
GO

-- ============================================================
-- STORED PROCEDURE: BloquearCuenta
-- ============================================================

CREATE OR ALTER PROCEDURE sp_BloquearCuenta
    @CuentaID   INT,
    @EmpleadoID INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Solo bloquea si no esta ya bloqueada
    IF EXISTS (SELECT 1 FROM Cuentas WHERE CuentaID = @CuentaID AND Bloqueada = 0)
    BEGIN
        UPDATE Cuentas
        SET    Bloqueada    = 1,
               BloqueadaPor = @EmpleadoID
        WHERE  CuentaID = @CuentaID;
        SELECT 1 AS Resultado;
    END
    ELSE
        SELECT 0 AS Resultado;
END
GO

-- ============================================================
-- STORED PROCEDURE: DesbloquearCuenta
-- ============================================================

CREATE OR ALTER PROCEDURE sp_DesbloquearCuenta
    @CuentaID   INT,
    @EmpleadoID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Cuentas
    SET    Bloqueada    = 0,
           BloqueadaPor = NULL
    WHERE  CuentaID     = @CuentaID;
    SELECT @@ROWCOUNT AS Resultado;
END
GO

-- ============================================================
-- STORED PROCEDURE: RegistrarMovimiento (Deposito/Retiro/Transferencia)
-- ============================================================

CREATE OR ALTER PROCEDURE sp_RegistrarMovimiento
    @CuentaID         INT,
    @EmpleadoID       INT,
    @Tipo             NVARCHAR(20),
    @Monto            DECIMAL(18,2),
    @CuentaDestinoID  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @SaldoActual    DECIMAL(18,2);
        DECLARE @SaldoNuevo     DECIMAL(18,2);
        DECLARE @NumOp          NVARCHAR(30);
        DECLARE @Bloqueada      BIT;
        DECLARE @BloqueadaPor   INT;

        -- Verificar cuenta origen
        SELECT @SaldoActual = Saldo, @Bloqueada = Bloqueada, @BloqueadaPor = BloqueadaPor
        FROM   Cuentas
        WHERE  CuentaID = @CuentaID;

        IF @SaldoActual IS NULL
        BEGIN
            SELECT -1 AS Resultado, 'Cuenta origen no encontrada' AS Mensaje, '' AS NumeroOperacion;
            ROLLBACK;
            RETURN;
        END

        -- Solo rechaza si la cuenta esta bloqueada por un empleado DIFERENTE
        IF @Bloqueada = 1 AND (@BloqueadaPor IS NULL OR @BloqueadaPor <> @EmpleadoID)
        BEGIN
            SELECT -2 AS Resultado, 'La cuenta esta bloqueada por otro cajero' AS Mensaje, '' AS NumeroOperacion;
            ROLLBACK;
            RETURN;
        END

        -- Generar numero de operacion
        SET @NumOp = 'OP-' + RIGHT('00000000' + CAST((SELECT ISNULL(MAX(MovimientoID),0)+1 FROM Movimientos) AS VARCHAR(8)), 8);

        IF @Tipo = 'Deposito'
        BEGIN
            SET @SaldoNuevo = @SaldoActual + @Monto;
            UPDATE Cuentas SET Saldo = @SaldoNuevo WHERE CuentaID = @CuentaID;
            INSERT INTO Movimientos (CuentaID, EmpleadoID, Tipo, Monto, SaldoAnterior, SaldoPosterior, NumeroOperacion)
            VALUES (@CuentaID, @EmpleadoID, 'Deposito', @Monto, @SaldoActual, @SaldoNuevo, @NumOp);
            SELECT 1 AS Resultado, 'Deposito exitoso' AS Mensaje, @NumOp AS NumeroOperacion;
        END
        ELSE IF @Tipo = 'Retiro'
        BEGIN
            IF @SaldoActual < @Monto
            BEGIN
                SELECT -3 AS Resultado, 'Saldo insuficiente' AS Mensaje, '' AS NumeroOperacion;
                ROLLBACK;
                RETURN;
            END
            SET @SaldoNuevo = @SaldoActual - @Monto;
            UPDATE Cuentas SET Saldo = @SaldoNuevo WHERE CuentaID = @CuentaID;
            INSERT INTO Movimientos (CuentaID, EmpleadoID, Tipo, Monto, SaldoAnterior, SaldoPosterior, NumeroOperacion)
            VALUES (@CuentaID, @EmpleadoID, 'Retiro', @Monto, @SaldoActual, @SaldoNuevo, @NumOp);
            SELECT 1 AS Resultado, 'Retiro exitoso' AS Mensaje, @NumOp AS NumeroOperacion;
        END
        ELSE IF @Tipo = 'Transferencia'
        BEGIN
            IF @CuentaDestinoID IS NULL
            BEGIN
                SELECT -4 AS Resultado, 'Cuenta destino requerida' AS Mensaje, '' AS NumeroOperacion;
                ROLLBACK;
                RETURN;
            END
            IF @SaldoActual < @Monto
            BEGIN
                SELECT -3 AS Resultado, 'Saldo insuficiente' AS Mensaje, '' AS NumeroOperacion;
                ROLLBACK;
                RETURN;
            END

            DECLARE @SaldoDestino DECIMAL(18,2);
            SELECT @SaldoDestino = Saldo FROM Cuentas WHERE CuentaID = @CuentaDestinoID;
            IF @SaldoDestino IS NULL
            BEGIN
                SELECT -5 AS Resultado, 'Cuenta destino no encontrada' AS Mensaje, '' AS NumeroOperacion;
                ROLLBACK;
                RETURN;
            END

            SET @SaldoNuevo = @SaldoActual - @Monto;
            UPDATE Cuentas SET Saldo = @SaldoNuevo WHERE CuentaID = @CuentaID;
            UPDATE Cuentas SET Saldo = @SaldoDestino + @Monto WHERE CuentaID = @CuentaDestinoID;

            INSERT INTO Movimientos (CuentaID, CuentaDestinoID, EmpleadoID, Tipo, Monto, SaldoAnterior, SaldoPosterior, NumeroOperacion)
            VALUES (@CuentaID, @CuentaDestinoID, @EmpleadoID, 'Transferencia', @Monto, @SaldoActual, @SaldoNuevo, @NumOp);
            SELECT 1 AS Resultado, 'Transferencia exitosa' AS Mensaje, @NumOp AS NumeroOperacion;
        END
        ELSE
        BEGIN
            SELECT -6 AS Resultado, 'Tipo de operacion no valido' AS Mensaje, '' AS NumeroOperacion;
            ROLLBACK;
            RETURN;
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        SELECT -99 AS Resultado, ERROR_MESSAGE() AS Mensaje, '' AS NumeroOperacion;
    END CATCH
END
GO

-- ============================================================
-- STORED PROCEDURE: ObtenerTodasCuentasEstado (MashUp)
-- ============================================================

CREATE OR ALTER PROCEDURE sp_ObtenerTodasCuentasEstado
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.CuentaID, c.NumeroCuenta,
           cl.Nombre + ' ' + cl.Apellido AS ClienteNombre,
           c.Saldo, c.Estado, c.Bloqueada,
           ISNULL(e.Nombre + ' ' + e.Apellido, '') AS BloqueadaPorNombre,
           s.Nombre AS NombreSucursal
    FROM   Cuentas c
    INNER JOIN Clientes   cl ON cl.ClienteID  = c.ClienteID
    INNER JOIN Sucursales  s ON  s.SucursalID  = c.SucursalID
    LEFT  JOIN Empleados   e ON  e.EmpleadoID  = c.BloqueadaPor
    ORDER BY c.SucursalID, c.NumeroCuenta;
END
GO

PRINT 'Base de datos EurekaBank creada correctamente.';
GO
