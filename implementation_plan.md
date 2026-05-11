# Plan de Implementación: Entrega 2

Este es el orden lógico sugerido para abordar todos los requerimientos que me pasaste. La idea es construir de manera incremental: primero asegurar la estructura de datos y permisos (Backend), luego asegurar la integridad de la base de datos (concurrencia y transacciones), seguido del auto-mantenimiento (worker) y finalmente conectar toda la experiencia visual en el Frontend.

## FASE 1: Estructura Base, Roles y Eventos (Backend)
El primer paso es tener los actores y los recursos bien definidos antes de operar sobre ellos.

1. **Gestión de Roles (Usuario y Admin):**
   - Asegurar que existan los roles diferenciados en el sistema.
2. **Creación de Eventos por Admin:**
   - Modificar la lógica actual para que la precarga de butacas, sectores y número de butacas no sea estática, sino que se genere cuando un **Admin** crea un evento.

## FASE 2: Auditoría y Trazabilidad (Backend)
Tener la auditoría lista desde el principio nos permitirá registrar todas las acciones de las fases siguientes sin tener que volver atrás a agregar logs.

3. **Mejorar el esquema de Auditoría (`AuditLog`):**
   - Asegurar que la tabla almacene: **quién** realizó la acción, **qué** acción fue, **sobre qué** recurso, y el **milisegundo exacto**.
   - Definir los eventos clave a registrar: Intento de reserva (exitoso y fallido), Pago realizado y Liberación de butaca.

## FASE 3: Reservas, Concurrencia y Pruebas de Estrés (Backend)
Aquí nos aseguramos de que el sistema soporte alta demanda sin sobrevender entradas.

4. **Refactorizar `POST /api/reservations`:**
   - Verificar y afianzar el mecanismo de concurrencia (optimista o pesimista) en la base de datos (o caché).
   - Asegurar que retorne `200/201` para el primer request exitoso y `409 Conflict` para los demás.
5. **Auditoría en Reservas:**
   - Agregar el registro en `AuditLog` tanto para la reserva exitosa como para el rechazo por conflicto (`409`).
6. **Prueba de Estrés (Testing):**
   - Simular múltiples peticiones concurrentes a la misma butaca para garantizar que el sistema es a prueba de balas.

## FASE 4: Pagos y Transaccionalidad ACID (Backend)
Una vez que las reservas son seguras, avanzamos al cobro, donde la atomicidad es crítica.

7. **Crear `POST /api/pagos`:**
   - Desarrollar el endpoint que simula la pasarela de pago.
8. **Implementar Transacción Estricta (ACID):**
   - Agrupar en una única transacción de base de datos:
     1. Cambiar la butaca a "Vendida".
     2. Cambiar la reserva a "Completada/Pagada".
     3. Registrar la acción en `AuditLog`.
   - Si cualquiera de estos pasos falla, ejecutar un **Rollback** completo.

## FASE 5: Auto-Mantenimiento y Worker (Backend)
Manejo de reservas abandonadas.

9. **Implementar Background Worker:**
   - Configurar un proceso en segundo plano (Worker, Cron job o HostedService como `ReservationCleanupWorker`).
10. **Lógica de Liberación (5 Minutos):**
    - El worker debe buscar periódicamente reservas activas con más de 5 minutos de antigüedad sin pagar.
    - Cancelar la reserva, liberar la butaca (cambiar estado a disponible) y registrar la acción en `AuditLog`.

## FASE 6: Experiencia de Usuario y Validaciones (Frontend)
Finalmente, reflejamos todo el trabajo del backend en una interfaz amigable e interactiva.

11. **Validaciones Preventivas:**
    - Deshabilitar visualmente (y a nivel código frontend) el botón de cualquier asiento que ya figure como ocupado o reservado.
12. **Manejo de Error `409 Conflict`:**
    - Interceptar este error desde el backend y mostrar una notificación **TOAST** amigable ("El asiento ya no está disponible").
    - Refrescar instantáneamente el mapa de asientos.
13. **Temporizador Visual (Carrito):**
    - Al lograr una reserva temporal (`201`), mostrar el carrito de compras con un contador regresivo visible (de 5:00 a 0:00).
    - Si el tiempo llega a 0:00, limpiar el carrito visualmente y mostrar un mensaje de expiración (el Backend ya habrá liberado la butaca gracias al Worker).

---

## Preguntas Abiertas

> [!TIP]
> ¿Estás de acuerdo con este orden? Si te parece bien, dime con qué punto de la **Fase 1** o **Fase 2** te gustaría que empecemos a escribir/revisar código. Ya veo que tienes abierto `IAuditLogRepository.cs` y `ReservationCleanupWorker.cs`, por lo que podríamos empezar directamente mejorando la auditoría o el worker si lo prefieres.
