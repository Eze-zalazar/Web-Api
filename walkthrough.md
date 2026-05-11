# Resumen de Implementación: Entrega 2

Se han implementado satisfactoriamente todas las funcionalidades solicitadas para la segunda entrega, garantizando escalabilidad, persistencia segura bajo concurrencia y transaccionalidad ACID.

## Cambios Realizados

### Backend (Web API)
1. **Eventos y Roles:**
   - Se añadió el atributo `Role` en la entidad `User`.
   - Se creó `CreateEventCommand` y `CreateEventCommandHandler` para permitir la creación dinámica de eventos, sectores y **generación automática de asientos** basada en la capacidad (`Capacity`) del sector configurada por el administrador.
2. **Auditoría (AuditLog):**
   - Se configuró explícitamente en Entity Framework que el campo `CreatedAt` se almacene como `datetime2(3)`, lo cual garantiza precisión exacta de **milisegundos**.
   - Se incorporó la creación de logs inmutables para: inicio de intento de reserva (fallidos por concurrencia u otros errores), reservas exitosas, pagos confirmados y liberación de asientos por expiración.
3. **Concurrencia de Reservas (Pruebas de Estrés preparadas):**
   - El `CreateReservationHandler` atrapa colisiones (`DbUpdateConcurrencyException`) debidas a la concurrencia optimista (`Version`). Si varios usuarios envían la petición simultánea sobre el mismo `SeatId`, solo uno logrará la reserva (Status 201), mientras que los demás generarán un log de auditoría del intento fallido y recibirán un `409 Conflict`.
4. **Endpoint de Pagos (ACID):**
   - Se creó `POST /api/v1/pagos` simulando la pasarela. Agrupa bajo el `UnitOfWork` (Transacción SQL Estricta) el cambio de estado de la butaca a "Vendida", la reserva a "Completada" y su respectiva auditoría. Todo ocurre o falla de manera conjunta (Rollback).
5. **Background Worker (Auto-Mantenimiento):**
   - Se implementó `ReservationCleanupWorker` inyectado como `HostedService`. Se ejecuta en segundo plano (cada 1 minuto) detectando las reservas `Pending` cuya fecha de expiración haya sido superada (5 minutos de vida útil). El worker las cancela, devuelve la butaca a "Disponible" y crea el registro en Auditoría.

### Frontend
1. **Manejo de Errores (409 Conflict):**
   - Al recibir este status de la API, se despliega instantáneamente una notificación Toast informando: _"El asiento ya no está disponible."_ y el mapa de butacas recarga su último estado.
2. **Temporizador y Carrito de Compras:**
   - Una vez confirmada una reserva temporal, se activa un reloj de 5:00 minutos decreciente de gran tamaño.
   - Cuenta con un botón para "Simular Pago".
   - Al expirar (0:00), el contador se destruye, alerta al usuario mediante Toast que su tiempo se acabó y el sistema recarga el mapa (cuyo botón asociado del backend ya habrá sido liberado por el Worker).

## Plan de Verificación (Testing Sugerido)

> [!TIP]
> **Para probar el sistema completo:**
> 1. Abre el frontend, selecciona un asiento y resérvalo. Verás el temporizador en pantalla.
> 2. Déjalo vencer. A los 5 minutos, el frontend te avisará y el Worker del Backend en la consola imprimirá que ha liberado la butaca y cancelado la reserva.
> 3. Puedes verificar en la base de datos SQL que en la tabla `AUDIT_LOG` quedó el registro con precisión a milisegundos (`.123`).
> 4. **Prueba de estrés:** Puedes usar herramientas como *Postman Runner* o *JMeter* enviando 10 peticiones POST concurrentes a `/api/v1/reservations` con el mismo asiento. Verificarás que el endpoint devuelve un `201` y nueve `409`.
