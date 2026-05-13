using Application.Interfaces;
using Application.UseCase.Payments.Commands;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.UseCase.Payments.Handlers
{
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
