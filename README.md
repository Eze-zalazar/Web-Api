# Web-Api

🎟️ **Sistema de Venta de Entradas - Entrega 1**
Sistema robusto de gestión y venta de entradas para eventos masivos, desarrollado con arquitectura limpia y preparado para manejar alta concurrencia.

---

## 📋 Tabla de Contenidos
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Requisitos Previos](#-requisitos-previos)
- [Configuración del Proyecto](#-configuración-del-proyecto)
- [Ejecución del Proyecto](#-ejecución-del-proyecto)
- [Endpoints de la API](#-endpoints-de-la-api)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Datos de Prueba](#-datos-de-prueba)
- [Pruebas Rápidas](#-pruebas-rápidas)
- [Solución de Problemas Comunes](#-solución-de-problemas-comunes)
- [Notas Importantes](#-notas-importantes)
- [Equipo de Desarrollo](#-equipo-de-desarrollo)

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core
- **Base de Datos:** SQL Server
- **Arquitectura:** Clean Architecture (Domain, Application, Infrastructure, WebApi)
- **Documentación:** Swagger/OpenAPI

### Frontend
- **Lenguaje:** JavaScript (ES6+)
- **Estilos:** Tailwind CSS
- **Arquitectura:** Componentes modulares (Pages, Services, Components)

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
  },
  "SeederSettings": {
    "SeatsPerSector": 50
  }
}
```

#### Opción B: Usando la terminal
1. Navega a la carpeta del proyecto `WebApi`:
```bash
cd WebApplication
```
2. Edita `appsettings.json` con tu editor preferido y actualiza la contraseña.

#### Verificar SQL Server
Asegúrate de que SQL Server esté corriendo:

**Windows:**
1. Presiona `Win + R`
2. Escribe `services.msc`
3. Busca "SQL Server (MSSQLSERVER)" o "SQL Server (SQLEXPRESS)"
4. Verifica que el estado sea "En ejecución"

Alternativamente, ejecuta en CMD/PowerShell:
```bash
sqlcmd -S localhost -U sa -P TU_PASSWORD
```

### 3. Aplicar Migraciones y Seed de Datos
Las migraciones se aplican automáticamente al iniciar la aplicación gracias al método `MigrateAsync()` en `Program.cs`.

Si prefieres aplicarlas manualmente:
```bash
cd WebApplication
dotnet ef database update
```

**Datos de Seed:** El sistema carga automáticamente:
- ✅ 6 eventos activos (Babasonicos, Los Piojos, Jonas Brothers, Anuel AA, Miranda!, Duki)
- ✅ 2 sectores por evento (Campo: $15.000, Platea: $25.000)
- ✅ 50 butacas numeradas por sector
- ✅ 1 usuario de prueba

---

## 🚀 Ejecución del Proyecto

### Backend (API)

#### Opción 1: Desde Visual Studio
1. Abre la solución `WebApplication.sln`
2. Selecciona el perfil de ejecución `http` o `https` en el dropdown (al lado del botón Run)
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
Verás un mensaje similar a:
```
Now listening on: http://localhost:5280
Application started. Press Ctrl+C to shut down.
```

### Frontend

#### Opción 1: Usando Live Server (Recomendado)
1. Instala la extensión "Live Server" en VS Code
2. Abre la carpeta `Front-api` en VS Code
3. Click derecho en `index.html` → Open with Live Server
4. Se abrirá en `http://127.0.0.1:5500`

#### Opción 2: Usando npx serve
```bash
cd Front-api
npx serve .
```
Se abrirá en `http://localhost:3000`

#### Opción 3: Python Simple HTTP Server
```bash
cd Front-api
python -m http.server 8000
```
Se abrirá en `http://localhost:8000`

> ⚠️ **Importante:** No abras `index.html` directamente desde el explorador de archivos (`file://`). Los módulos ES6 requieren un servidor HTTP para funcionar correctamente.

---

## 📡 Endpoints de la API

### Eventos

**GET `/api/v1/events`**
Listado paginado de eventos activos.

- Query Parameters:
  - `page` (int, requerido): Número de página (comienza en 1)
  - `pageSize` (int, requerido): Cantidad de eventos por página

Ejemplo: `GET http://localhost:5280/api/v1/events?page=1&pageSize=10`
Respuesta `200 OK`:
```json
[
  {
    "id": 1,
    "name": "Concierto de Babasonicos",
    "venue": "Estadio Central",
    "eventDate": "2026-06-22T19:19:44.481Z",
    "status": "Active"
  }
]
```

**GET `/api/v1/events/{id}`**
Obtener un evento específico por ID.

### Butacas

**GET `/api/v1/events/{id}/seats`**
Obtener todas las butacas de un evento con su estado actual.

Ejemplo: `GET http://localhost:5280/api/v1/events/1/seats`
Respuesta `200 OK`:
```json
[
  {
    "id": "13b52825-4fe6-4e53-aa06-00e65a3e3dc0",
    "rowIdentifier": "A",
    "seatNumber": 20,
    "status": "Reserved",
    "sectorId": 2
  }
]
```
Estados posibles:
- `Available`: Butaca disponible
- `Reserved`: Butaca reservada temporalmente
- `Sold`: Butaca vendida

### Reservas

**POST `/api/v1/reservations`**
Crear una nueva reserva.

Body:
```json
{
  "seatId": "13b52825-4fe6-4e53-aa06-00e65a3e3dc0"
}
```
Respuesta `201 Created`:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "seatId": "13b52825-4fe6-4e53-aa06-00e65a3e3dc0",
  "status": "Pending",
  "reservedAt": "2026-04-28T10:30:00Z"
}
```
- `409 Conflict`: La butaca ya fue reservada por otro usuario o no está disponible.
- `404 Not Found`: La butaca no existe.

### Auditoría

**GET `/api/v1/audit-logs`**
Listado paginado de logs de auditoría.

- Query Parameters:
  - `page` (int, requerido): Número de página (comienza en 1)
  - `pageSize` (int, requerido): Cantidad de eventos por página

---

## 📁 Estructura del Proyecto

### Backend
```
WebApplication/
├── Domain/                    # Entidades de dominio
│   └── Entities/
│       ├── Event.cs
│       ├── Sector.cs
│       ├── Seat.cs
│       ├── User.cs
│       ├── Reservation.cs
│       └── Audit_Log.cs
│   └── Exceptions/
├── Application/               # Lógica de negocio
│   ├── DTOs/
│   ├── Interfaces/
│   └── UseCase/
│       ├── Eventos/
│       ├── Seats/
│       ├── Reservations/
│       └── AuditLogs/
├── Infrastructure/            # Acceso a datos
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   ├── Repositories/
│   │   └── Seeders/
│   │       └── DatabaseSeeder.cs
│   └── Migrations/
└── WebApi/                    # Capa de presentación
    ├── Controllers/
    │   ├── EventController.cs
    │   ├── SeatsController.cs
    │   ├── ReservationController.cs
    │   └── AuditLogsController.cs
    ├── Program.cs
    └── appsettings.json
```

### Frontend
```
Front-api/
├── index.html
├── Js/
│   ├── Controllers/
│   │   └── UserPageMain.js
│   ├── Pages/
│   │   ├── EventsPage.js
│   │   └── SeatSelectionPage.js
│   ├── Components/
│   │   ├── Carts/
│   │   │   ├── EventsCard.js
│   │   │   └── SectorCard.js
│   │   ├── Services/
│   │   │   ├── EventService.js
│   │   │   ├── SeatService.js
│   │   │   └── ReservationService.js
│   │   ├── Search/
│   │   │   └── filterEvents.js
│   │   └── Toast/
│   │       └── toast.js
│   └── Styles/
│       └── StyleSections/
│           └── Paleta.css
```

---

## 🧪 Datos de Prueba

**Eventos Precargados:**
| ID | Nombre | Venue | Fecha |
|---|---|---|---|
| 1 | Concierto de Babasonicos | Estadio Central | +2 meses |
| 2 | Concierto de Los Piojos | Estadio Monumental | +3 meses |
| 3 | Concierto de Jonas Brothers | Movistar Arena | +4 meses |
| 4 | Concierto de Anuel AA | Luna Park | +5 meses |
| 5 | Concierto de Miranda! | Teatro Gran Rex | +6 meses |
| 6 | Concierto de Duki | Movistar Arena | +7 meses |

**Sectores por Evento:**
Cada evento tiene 2 sectores:
- Campo: $15.000 (50 butacas)
- Platea: $25.000 (50 butacas)

**Usuario de Prueba:**
- ID: `1`
- Nombre: `Usuario Test`
- Email: `test@test.com`

---

## 🧪 Pruebas Rápidas

### Verificar que la API funciona
Abre Swagger UI en `http://localhost:5280/swagger` y ejecuta:
1. `GET /api/v1/events?page=1&pageSize=10` → Debería devolver los eventos.
2. `GET /api/v1/events/1/seats` → Debería devolver 100 butacas (2 sectores × 50).
3. `POST /api/v1/reservations` → Usa un `seatId` del paso 2.

### Verificar que el Frontend funciona
1. Abre `http://localhost:5500` (o el puerto que uses).
2. Deberías ver las cards de eventos.
3. Click en "Seleccionar butacas" de cualquier evento.
4. Deberías ver el mapa con 100 butacas (verdes = disponibles, rojas = ocupadas).
5. Click en una butaca verde → se pone azul y aparece en el panel derecho.
6. Click en "Reservar butaca" → debería mostrar toast de éxito y la butaca quedar roja.

---

## 🔧 Solución de Problemas Comunes

**Error: "Cannot open database"**
- **Causa:** SQL Server no está corriendo o la contraseña es incorrecta.
- **Solución:** Verifica que SQL Server esté corriendo. Revisa que la contraseña en `appsettings.json` sea correcta.

**Error: "Failed to load resource: net::ERR_CONNECTION_REFUSED"**
- **Causa:** La API no está corriendo o el frontend apunta al puerto incorrecto.
- **Solución:** Verifica que la API esté corriendo en `http://localhost:5280`.

**Frontend queda en "Loading..."**
- **Causa:** Archivos JS no cargan (problema CORS o servidor HTTP).
- **Solución:** NO abras `index.html` directamente desde el explorador. Usa Live Server, `npx serve`, o Python HTTP Server.

**Error: "409 Conflict" al reservar**
- **Causa:** La butaca ya fue reservada o no está disponible.
- **Solución:** Esto es el comportamiento esperado. Selecciona otra butaca disponible.

---

## 📝 Notas Importantes
- ✅ Las migraciones se aplican automáticamente al iniciar la API.
- ✅ El seeder solo corre si la tabla Events está vacía.
- ✅ Para resetear la BD: borrar `TicketingDB` en SSMS y reiniciar la API.
- ✅ Los cambios en `appsettings.json` requieren reiniciar la API.
- ⚠️ El frontend requiere que la API esté corriendo primero.
- ⚠️ No usar el frontend con `file://` — siempre usar un servidor HTTP.

---

## 👥 Equipo de Desarrollo
- Ezequiel Zalazar 
- Eliana Vazquez
