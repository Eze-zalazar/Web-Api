using Application.Interfaces;
using Application.UseCase.Payments.Commands;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.UseCase.Payments.Handlers
{
    public class ProcessPaymentHandler : IProcessPaymentHandler
    {
        private readonly Interfaces.IReservationRepository _reservationRepository;
        private readonly Interfaces.ISeatRepository _seatRepository;
        private readonly Interfaces.IAuditLogRepository _auditLogRepository;
        private readonly Interfaces.IUnitOfWork _unitOfWork;

        public ProcessPaymentHandler(
            Interfaces.IReservationRepository reservationRepository,
            Interfaces.ISeatRepository seatRepository,
            Interfaces.IAuditLogRepository auditLogRepository,
            Interfaces.IUnitOfWork unitOfWork)
        {
            _reservationRepository = reservationRepository;
            _seatRepository = seatRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(Commands.ProcesarPagoCommand command)
        {
            var reservation = await _reservationRepository.GetByIdAsync(command.ReservaId);

            if (reservation == null)
                throw new Exception("Reserva no encontrada");

            if (reservation.UserId != command.UsuarioId)
                throw new Exception("La reserva no pertenece a este usuario");

            if (reservation.Status != "Pending")
                throw new Exception("La reserva no está en estado pendiente o ya ha sido pagada");

            var seat = await _seatRepository.GetByIdAsync(reservation.SeatId);
            if (seat == null)
                throw new Exception("Butaca asociada no encontrada");

            // INICIO DE TRANSACCIÓN ACID
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Cambiar la butaca a "Vendida"
                seat.Status = "Sold";
                await _seatRepository.UpdateAsync(seat);

                // 2. Cambiar la reserva a "Completada" / "Pagada"
                reservation.Status = "Completed";
                await _reservationRepository.UpdateAsync(reservation);

                // 3. Registrar la acción en AuditLog
                var auditLog = new Domain.Entities.Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UsuarioId,
                    Action = "PAYMENT_SUCCESS",
                    EntityType = "Reservation",
                    EntityId = reservation.Id.ToString(),
                    Details = $"Pago exitoso por monto {command.MontoPagado} usando {command.MetodoPago}.",
                    CreatedAt = DateTime.UtcNow,
                    MilisegundoExacto = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                await _auditLogRepository.AddAsync(auditLog);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                // Registrar el intento fallido fuera de la transacción fallida
                var failedAuditLog = new Domain.Entities.Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UsuarioId,
                    Action = "PAYMENT_FAILED",
                    EntityType = "Reservation",
                    EntityId = command.ReservaId.ToString(),
                    Details = $"Fallo al procesar pago: {ex.Message}",
                    CreatedAt = DateTime.UtcNow,
                    MilisegundoExacto = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                await _auditLogRepository.AddAsync(failedAuditLog);
                await _unitOfWork.SaveChangesAsync();

                throw;
            }
        }
    public class ProcessPaymentHandler : IProcessPaymentHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRepository _reservationRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public ProcessPaymentHandler(
            IUnitOfWork unitOfWork,
            IReservationRepository reservationRepository,
            IAuditLogRepository auditLogRepository)
        {
            _unitOfWork = unitOfWork;
            _reservationRepository = reservationRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Reservation> HandleAsync(ProcessPaymentCommand command)
        {
            // Iniciar Transacción Estricta
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var reservation = await _reservationRepository.GetByIdWithSeatAsync(command.ReservationId);

                if (reservation == null)
                    throw new Exception("Reserva no encontrada.");

                if (reservation.UserId != command.UserId)
                    throw new Exception("La reserva no pertenece al usuario especificado.");

                if (reservation.Status != "Pending" && reservation.Status != "Reserved")
                    throw new Exception("La reserva ya ha sido procesada o expirada.");

                if (DateTime.UtcNow > reservation.ExpiresAt)
                    throw new Exception("El tiempo de la reserva ha expirado.");

                // 1. Modificar Reserva a Completada
                reservation.Status = "Completed";

                // 2. Modificar Butaca a Vendida
                if (reservation.Seat != null)
                {
                    reservation.Seat.Status = "Sold";
                }

                // 3. Generar Registro de Auditoría
                var auditLog = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Action = "Payment Completed",
                    EntityType = "Reservation",
                    EntityId = reservation.Id.ToString(),
                    Details = $"Pago procesado y butaca vendida. Butaca ID: {reservation.Seat?.Id}",
                    CreatedAt = DateTime.UtcNow,
                    MilisegundoExacto = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                await _auditLogRepository.AddAsync(auditLog);

                // Guardar cambios y commitear transacción
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return reservation;
            }
            catch (Exception ex)
            {
                // Si falla en cualquier punto, hacemos Rollback
                await _unitOfWork.RollbackTransactionAsync();

                // Registrar el intento fallido fuera de la transacción fallida
                var failedAuditLog = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Action = "PAYMENT_FAILED",
                    EntityType = "Reservation",
                    EntityId = command.ReservationId.ToString(),
                    Details = $"Fallo al procesar pago: {ex.Message}",
                    CreatedAt = DateTime.UtcNow,
                    MilisegundoExacto = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                await _auditLogRepository.AddAsync(failedAuditLog);
                await _unitOfWork.SaveChangesAsync();

                throw;
            }
        }
    }
}
