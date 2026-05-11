# Tareas de Implementación: Entrega 2

## FASE 1: Estructura Base, Roles y Eventos
- [x] Asegurar existencia y manejo de roles Usuario y Admin.
- [x] Modificar creación de eventos (Admin) para configurar sectores y butacas dinámicamente.

## FASE 2: Auditoría y Trazabilidad
- [ ] Revisar/Actualizar modelo `AuditLog` para incluir: quién, qué, recurso, y milisegundo exacto.
- [ ] Implementar registro de auditoría para intento de reserva (exitoso y fallido).
- [ ] Implementar registro de auditoría para liberación de butaca.
- [ ] Implementar registro de auditoría para pago realizado.

## FASE 3: Reservas, Concurrencia y Pruebas de Estrés
- [ ] Refactorizar `POST /api/reservations` para manejar concurrencia.
- [ ] Retornar `201` para éxito y `409 Conflict` para fallos de concurrencia.
- [ ] Escribir/asegurar pruebas de estrés para validar concurrencia.

## FASE 4: Pagos y Transaccionalidad ACID
- [x] Crear endpoint `POST /api/pagos` simulando pasarela de pago.
- [x] Implementar transacción estricta (ACID) agrupando: cambio de butaca a "Vendida", reserva a "Completada", y registro en AuditLog.
- [x] Asegurar Rollback en caso de falla en el pago.

## FASE 5: Auto-Mantenimiento y Worker
- [x] Implementar/Configurar `ReservationCleanupWorker` en segundo plano.
- [x] Lógica para expirar reservas con > 5 minutos sin pagar (liberar butaca, cancelar reserva, registrar auditoría).

## FASE 6: Experiencia de Usuario y Validaciones (Frontend)
- [x] Deshabilitar butacas ocupadas o reservadas visualmente.
- [x] Manejo de error 409: Mostrar TOAST amigable y refrescar mapa.
- [x] Mostrar temporizador visual regresivo (5:00 a 0:00) al reservar.
- [x] Limpiar carrito y mostrar mensaje si expira el tiempo en Frontend.
