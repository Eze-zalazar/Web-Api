# Web-Api

🎟️ **Sistema de Venta de Entradas - Entrega Final (Fases 1 y 2)**
Sistema robusto de gestión y venta de entradas para eventos masivos, desarrollado con Clean Architecture y rigurosamente preparado para manejar alta concurrencia, transacciones atómicas y liberación automática de recursos.

---

## 📋 Tabla de Contenidos
- [Características Principales](#-características-principales)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Requisitos Previos](#-requisitos-previos)
- [Configuración del Proyecto](#-configuración-del-proyecto)
- [Ejecución del Proyecto](#-ejecución-del-proyecto)
- [Endpoints de la API](#-endpoints-de-la-api)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Datos de Prueba](#-datos-de-prueba)
- [Solución de Problemas Comunes](#-solución-de-problemas-comunes)
- [Equipo de Desarrollo](#-equipo-de-desarrollo)

---

## ✨ Características Principales

*   **Arquitectura de Datos (Code-First):** Modelo de dominio normalizado con generación automática de esquemas mediante migraciones de Entity Framework Core.
*   **Alta Concurrencia (Optimistic Locking):** Protección contra *race conditions* usando un token de concurrencia (`Version`). Si dos usuarios intentan reservar la misma butaca al mismo milisegundo, solo uno tiene éxito y el otro recibe un amigable error HTTP 409 Conflict.
*   **Transaccionalidad (ACID):** El procesamiento de pagos y confirmación de reservas está agrupado en un `UnitOfWork`. Si alguna parte falla, se ejecuta un *Rollback* automático y seguro.
*   **Auto-Mantenimiento (Background Jobs):** Un servicio en segundo plano (`ReservationExpirationWorker`) escanea constantemente reservas que lleven más de 5 minutos sin ser pagadas, liberando las butacas automáticamente para nuevos compradores.
*   **Auditoría y Trazabilidad:** Todo intento de reserva (exitoso o fallido por concurrencia), procesamiento de pago y liberación de sistema queda registrado inmutablemente en un `AuditLog`, registrando acción, usuario y milisegundo exacto.
*   **UX Reactiva y Temporizadores:** El Frontend implementa una cuenta regresiva (05:00 a 00:00) para pagar las reservas, notificaciones emergentes (Toasts) amigables, y comunicación asíncrona total sin recarga de página.

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core
- **Base de Datos:** SQL Server
- **Arquitectura:** Clean Architecture (Domain, Application, Infrastructure, WebApi)
- **Seguridad:** Autenticación por Cookies (`CookieAuthenticationDefaults`) y autorización por Roles.
- **Documentación:** Swagger / OpenAPI

### Frontend
- **Lenguaje:** JavaScript (Vanilla ES6+), HTML5, CSS3
- **Arquitectura:** Componentes modulares, servicios asíncronos (`Fetch API` con `credentials: 'include'`).
- **Feedback Visual:** Spinners, Toasts para errores y éxitos, Temporizadores de expiración.

---

## ✅ Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:
- .NET 8.0 SDK
- SQL Server 2019+ o SQL Server Express
- Visual Studio 2022 (recomendado) o Visual Studio Code
- Node.js (opcional, para Live Server en frontend)
- SQL Server Management Studio (SSMS) (opcional, para administrar la BD)

---

## ⚙️ Configuración del Proyecto

### 1. Clonar el Repositorio
```bash
git clone <URL_DEL_REPOSITORIO>
cd <NOMBRE_DEL_PROYECTO>
```

### 2. Configurar la Base de Datos

#### Opción A: Usando Visual Studio
1. Abre la solución `WebApplication.sln` en Visual Studio 2022
2. Abre el archivo `appsettings.json` en el proyecto `WebApi`
3. Modifica la cadena de conexión con tus credenciales:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TicketingDB;User Id=sa;Password=TU_PASSWORD_AQUI;TrustServerCertificate=True;"
  }
}
```

#### Opción B: Usando la terminal
1. Navega a la carpeta del proyecto `WebApi`:
```bash
cd WebApplication
```
2. Edita `appsettings.json` con tu editor preferido y actualiza la contraseña de tu instancia SQL Server.

### 3. Aplicar Migraciones y Seed de Datos
Las migraciones y la inyección de datos semilla (seeding) **se aplican automáticamente** al iniciar la aplicación gracias a `MigrateAsync()` y `DatabaseSeeder.SeedAsync()` en `Program.cs`.

Si prefieres aplicarlas manualmente:
```bash
cd WebApplication
dotnet ef database update
```

---

## 🚀 Ejecución del Proyecto

### Backend (API)

#### Opción 1: Desde Visual Studio
1. Abre la solución `WebApplication.sln`
2. Selecciona el perfil de ejecución `http` o `https`
3. Presiona `F5` o click en el botón ▶️
4. La API se levantará en:
   - HTTP: `http://localhost:5280`
   - HTTPS: `https://localhost:7253`
5. Swagger UI se abrirá automáticamente en `/swagger`

#### Opción 2: Desde la Terminal
```bash
cd WebApplication/WebApplication
dotnet run --launch-profile http
```

### Frontend

Dado que la aplicación maneja sesiones seguras por Cookies (`credentials: 'include'`), es **estrictamente necesario** abrir el Frontend a través de un servidor HTTP local. No abras `index.html` directamente desde tu sistema de archivos.

#### Opción 1: Usando Live Server (Recomendado)
1. Instala la extensión "Live Server" en VS Code.
2. Abre la carpeta `Front-api` en VS Code.
3. Click derecho en `index.html` → Open with Live Server.

#### Opción 2: Usando npx serve
```bash
cd Front-api
npx serve .
```

---

## 📡 Endpoints de la API (Resumen)

### Autenticación (`/api/v1/auth`)
- **POST `/login`**: Inicia sesión y devuelve una Cookie persistente (requerida para reservar y pagar).
- **POST `/logout`**: Destruye la sesión actual.

### Eventos y Butacas (`/api/v1/events`)
- **GET `/`**: Listado paginado de eventos.
- **GET `/{id}/seats`**: Obtener el estado actual (Available, Reserved, Sold) de todas las butacas de un evento.
- **POST `/`**: (Solo Admins) Crea un nuevo evento.

### Reservas y Pagos (`/api/v1/reservations` | `/api/v1/payments`)
- **POST `/reservations`**: Bloqueo temporal (5 mins). Si la butaca está siendo tomada por otro usuario, retorna `409 Conflict`.
- **POST `/payments`**: Procesa la transacción atómica. Pasa la reserva a "Completed" y la butaca a "Sold". Retorna error si han pasado los 5 minutos.

---

## 🧪 Datos de Prueba

Al iniciar por primera vez, el sistema autoconstruye el siguiente set de datos:

**Usuarios:**
- Administrador: `admin@admin.com` | Pass: `admin123`
- Cliente: `cliente@cliente.com` | Pass: `cliente123`

**Evento Activo:**
- **Rock en el Estadio - Babasonicos**
- Sectores: **Campo** ($15.000) y **Platea** ($25.000).
- Butacas: **50 butacas numeradas** disponibles para venta por cada sector.

---

## 🔧 Solución de Problemas Comunes

**1. No puedo loguearme o reservar (Error al Parsear JSON)**
- **Solución:** Asegúrate de ejecutar el frontend desde un servidor HTTP local (ej. Live Server). Las políticas modernas de navegadores prohíben el envío de Cookies de sesión desde orígenes `file://`.

**2. Error: "409 Conflict" al reservar**
- **Solución:** ¡Esto es un feature! El sistema está protegiendo la butaca porque otro usuario la reservó una fracción de segundo antes que tú o ya no está disponible. Verás un toast en la UI notificándotelo.

**3. Las reservas desaparecen sin hacer nada**
- **Solución:** Es el Worker de mantenimiento. Cualquier reserva no pagada en 5 minutos se autodestruye y la butaca vuelve a estar verde.

---

## 👥 Equipo de Desarrollo
- Ezequiel Zalazar 
- Eliana Vazquez
