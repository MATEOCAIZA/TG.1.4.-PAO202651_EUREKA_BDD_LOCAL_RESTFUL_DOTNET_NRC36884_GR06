# EurekaBank: credenciales, tema común y pruebas de conexión

Fecha: 2026-11-04 · PC de prueba: CESAR (Windows 11, configuración regional es-ES)

## 1. Credenciales (todo local)

| Qué | Usuario | Clave | Nota |
|---|---|---|---|
| MariaDB/MySQL `localhost:3306` | `root` | `hola123` | **Clave puesta en esta PC**; antes root no tenía clave. Es la misma que ya usan los dos servidores Java (`AccesoDB.java`), así que no se cambió el código. |
| SQL Server `localhost` | (Windows) | — | Autenticación de Windows. Esta PC no acepta `sa`. |
| **Login de TODOS los clientes (16)** | **`MONSTER`** | **`MONSTER69`** | Usuario oficial del grupo. Está en `Carga_Datos.sql` (empleado `0015`, sucursal libre) y en `BD_SQLSERVER_EurekaBank.sql` (EmpleadoID 3, sucursal Sipan). Ningún cliente tiene usuarios en el código: todo se valida en la BD. |
| App Java (BD MySQL `eurekabank`) | `cromero` | `chicho` | También: `lcastro/flaca`, `creyes/linda`, `aramos/china` (ver `Carga_Datos.sql`) |
| App .NET (BD SQL Server `EurekaBank`) | `cromero` | `chicho` | Sucursal Sipan (cuentas 00100001-00100003) |
| App .NET | `lcastro` | `flaca` | Sucursal Chan Chan (cuentas 00200001-00200002) |

## 2. Bases de datos

- **Java → MySQL**: `Crea_BD.sql` + `Carga_Datos.sql` (en `TG.1.1 …/04. CLICON/SOAP_EUREKABANK_JAVA_GR02`).
  No se ejecutó el bloque `GRANT … 'eureka'@'%' IDENTIFIED BY 'admin'` del final de `Crea_BD.sql`: creaba un superusuario accesible desde la red y ningún proyecto lo usa.
- **.NET → SQL Server**: no venía ningún script, así que se reconstruyó en [`BD_SQLSERVER_EurekaBank.sql`](BD_SQLSERVER_EurekaBank.sql) a partir de los procedimientos que llaman los dos servidores (`sp_Login`, `sp_ObtenerCuentasPorSucursal`, `sp_ObtenerMovimientos`, `sp_RegistrarMovimiento`, `sp_BloquearCuenta`, `sp_DesbloquearCuenta`, `sp_ObtenerTodasCuentasEstado`).
  ```
  sqlcmd -S localhost -E -C -f 65001 -i BD_SQLSERVER_EurekaBank.sql
  ```
  `Bloqueada` funciona como semáforo: el cliente bloquea, opera y desbloquea. `sp_RegistrarMovimiento` solo rechaza si la cuenta la tiene bloqueada **otro** empleado.
- Cadenas de conexión de los dos servidores .NET (`Web.config`): `sa/Holas123!` y `localhost,1433` cambiaron a `Integrated Security=True` y `localhost`. Esta PC no tiene TCP 1433 activo ni login `sa`. Si en otra PC usan `sa`, basta con volver a poner esa línea. **Este cambio no se subió a GitHub**: el repo conserva la cadena original con `sa`; en esta PC quedó solo como cambio local.

## 3. Servidores y puertos usados en la prueba

| Proyecto | Servidor | Dónde corre | URL |
|---|---|---|---|
| TG.1.1 SOAP Java | `WS_EUREKA` | Payara 5 (`C:\payara5`) | `http://localhost:8080/WS_EUREKA/WSEureka?wsdl` |
| TG.1.2 SOAP .NET | `WS_EUREKABANK_SOAP_DOTNET_GR06` | IIS Express | `http://localhost:8086/Service1.svc` |
| TG.1.3 REST Java | `WS_EurekaBank` | Payara 5 | `http://localhost:8080/WS_EurekaBank/webresources/coreBancario` |
| TG.1.4 REST .NET | `WS_EUREKABANK_RESTFUL_DOTNET_GR06` | IIS Express | `http://localhost:8085/api/...` |

IIS Express a mano: `"C:\Program Files\IIS Express\iisexpress.exe" /path:"<carpeta del servidor>" /port:8086` (o 8085).

## 4. Interfaz común

Todo sale de la carpeta [`_TEMA_COMUN`](_TEMA_COMUN): paleta morado oscuro / verde neón / acento naranja, paneles tipo HUD e imagen `hud.jpg`.

| Tipo de cliente | Cómo se aplica |
|---|---|
| Web (4) | `eureka-hud.css` + `hud.jpg` copiados en `web/css` (Java) o `Content/` (.NET) y enlazados al final de cada página o layout |
| Escritorio Java Swing (2) | `HudTheme.java` + `HudTheme.install()` en `Login.main()`; pinta cada ventana al abrirse. La imagen del login (`recursos/background (1).png`) ahora es la del tema |
| Escritorio WinForms (2) | `HudTheme.cs` (en el `.csproj`) + `HudTheme.Apply(this)` en cada formulario; `login.jpeg` ahora es la del tema |
| Móvil Android (4) | `04. CLIMOV` en cada repo. Login con `hud.jpg`, cabecera morada con línea verde, botones morado/verde y salida en verde monoespaciado |
| Consola (4) | El mismo banner `▓▓ EUREKABANK ▓▓`, títulos `▌`, opciones `[n]` y prompt `»`. Los colores en Java solo se activan en una terminal real (no en la salida de NetBeans) |

Las imágenes originales quedaron en `_TEMA_COMUN/respaldo/`.

**MashUp oculto** en los 6 clientes .NET: se quitó del menú o del navbar y el botón quedó en `Visible = false`. Las páginas y formularios siguen existiendo; solo se ocultó la entrada.

## 5. Resultado de las pruebas (contra los servidores locales)

| # | Cliente | Login | Operación probada | Resultado |
|---|---|---|---|---|
| 1 | SOAP Java · consola | ✔ | saldo, depósito, retiro, movimientos | OK |
| 2 | SOAP Java · escritorio | ✔ | login + saldo por el controlador; ventanas renderizadas | OK |
| 3 | SOAP Java · web | ✔ | saldo, depósito, transferencia, movimientos | OK* |
| 4 | SOAP .NET · consola | ✔ | cuentas, depósito 10.50, movimientos | OK |
| 5 | SOAP .NET · escritorio | ✔ | login + carga de cuentas | OK (tras corregir el namespace) |
| 6 | SOAP .NET · web | ✔ | cuentas, depósito 25.00, saldo correcto | OK* |
| 7 | REST Java · consola | ✔ | saldo, retiro 7.5, transferencia 2.5, movimientos | OK |
| 8 | REST Java · escritorio | ✔ | login + saldo por el controlador; ventanas renderizadas | OK |
| 9 | REST Java · web | ✔ | saldo, depósito, transferencia, movimientos | OK* |
| 10 | REST .NET · consola | ✔ | cuentas, transferencia 5.25, historial | OK |
| 11 | REST .NET · escritorio | ✔ | login + carga de cuentas | OK |
| 12 | REST .NET · web | ✔ | cuentas, retiro 20.50, depósito 12.75 | OK* |

\* Estos clientes apuntan a IPs de Radmin VPN (`26.243.233.245`, `26.61.187.252`), que **se dejaron como estaban**. Esas IPs no respondían durante la prueba, así que se probaron con una copia temporal (fuera del proyecto) con la IP cambiada a `localhost`. El código y la configuración originales mantienen la IP.

| 13 | SOAP Java · móvil | ✔ MONSTER | saldo, depósito 25.00, retiro 10.50, transferencia 5, retiro excesivo rechazado, movimientos | OK* |
| 14 | SOAP .NET · móvil | ✔ MONSTER | cuentas de Sipan, depósito 25.00, transferencia 10.50, retiro excesivo → "Saldo insuficiente", movimientos | OK* |
| 15 | REST Java · móvil | ✔ MONSTER | saldo, depósito 12.75, retiro 7.5, transferencia 2.5, movimientos | OK* |
| 16 | REST .NET · móvil | ✔ MONSTER | cuentas de Sipan, depósito 25.00, transferencia 10.50, retiro excesivo → "Saldo insuficiente", movimientos | OK* |

Los móviles se probaron en el emulador Pixel 9 Pro (Android 16) con `adb reverse tcp:8080/8085/8086`, escribiendo `localhost:<puerto>` en el campo **Servidor** del login. Los 4 servidores también se probaron con `MONSTER / MONSTER69`.

Al final se restauraron las dos BDs a sus datos iniciales (ya con MONSTER).

## 5b. Clientes móviles (`04. CLIMOV`)

| Proyecto | Carpeta | Tecnología | Servidor por defecto |
|---|---|---|---|
| TG.1.1 SOAP Java | `CLIMOV_EUREKABANK_SOAP_JAVA_GR06` | Android nativo en Java, sin librerías (SOAP armado a mano + `XmlPullParser`) | `26.243.233.245:8080` |
| TG.1.2 SOAP .NET | `CLIMOV_EUREKABANK_SOAP_DOTNET_GR06` | .NET MAUI (net10.0-android) + WCF `System.ServiceModel.Http`, mismo contrato que la consola | `26.243.233.245:8086` |
| TG.1.3 REST Java | `CLIMOV_EUREKABANK_RESTFUL_JAVA_GR06` | Android nativo en Java, sin librerías (`HttpURLConnection` + `org.json`) | `26.243.233.245:8080` |
| TG.1.4 REST .NET | `CLIMOV_EUREKABANK_RESTFUL_DOTNET_GR06` | .NET MAUI (net10.0-android) + `HttpClient` | `26.243.233.245:8085` |

- La IP del servidor se escribe en el login y se recuerda. En el emulador use `10.0.2.2:<puerto>`, en un celular la IP de la PC (Radmin VPN o Wi-Fi). IIS Express solo acepta `localhost`: para un celular real hay que publicar los .NET en IIS o usar `adb reverse`.
- Java: abrir la carpeta en Android Studio (o `gradlew assembleDebug`). MAUI: abrir el `.csproj` en Visual Studio y elegir un dispositivo Android.
- MAUI: `Directory.Build.props` manda `obj/` a `%LOCALAPPDATA%\climov\` porque la ruta del deber supera los 260 caracteres y aapt2 falla. Para una APK suelta (sin Visual Studio): `dotnet build -f net10.0-android -p:EmbedAssembliesIntoApk=true`.
- Igual que en los otros clientes .NET, el móvil bloquea la cuenta, registra el movimiento y la desbloquea siempre. No tiene MashUp.

## 6. Errores encontrados y corregidos

1. **Escritorio SOAP .NET no podía iniciar sesión**: `Services/EurekaServiceClient.cs` tenía el namespace `…SOAP_DOTNET_GR01` y el servidor usa `…_GR06`, así que las respuestas llegaban vacías. Se corrigió a `GR06`.
2. **Escritorio SOAP .NET y consola SOAP .NET usaban puertos distintos** (8083 y 8086) para el mismo servidor. El endpoint `localhost` del escritorio pasó a 8086; la entrada `26.61.187.252:8083` no se tocó.
3. **Montos multiplicados por 100**: con Windows en español, "25.00" se leía como 2500. Se fijó la cultura `en-US` (punto decimal y `$`) en el `Web.config` de las webs .NET, en el `Main` de las consolas y escritorios .NET, y con `Scanner.useLocale(Locale.US)` en las consolas Java. Coincide con lo que piden los formularios ("Ej. 250.00").
4. **Textos rotos** en la web REST .NET ("OperaciÃ³n", "contraseÃ±a"): estaban mal codificados en el propio código fuente.
5. **Fecha con ",-18"** en los movimientos de las consolas .NET: la alineación estaba escrita después del formato.

## 7. Archivos agregados o modificados

- Nuevos: `BD_SQLSERVER_EurekaBank.sql`, `CREDENCIALES_Y_PRUEBAS.md`, `_TEMA_COMUN/*`, `HudTheme.java` (2), `HudTheme.cs` (2), `eureka-hud.css` + `hud.jpg` (4 webs).
- Modificados: los 14 JSP, `Site.Master`, `Login.aspx`, `Cuentas.aspx`, `_Layout.cshtml`, `Auth/Login.cshtml`, `Cuentas/Index.cshtml`, `Web.config` (2 webs .NET y 2 servidores .NET), los 4 `Main`/`Program.cs` de consola, los `Login.java`, los formularios WinForms, `App.config` del escritorio SOAP .NET y los 4 `.csproj` (web + escritorio .NET).
- Compilar los .NET genera `bin/`, `obj/` y `packages/` en sus carpetas; es normal y Visual Studio los recrea.
