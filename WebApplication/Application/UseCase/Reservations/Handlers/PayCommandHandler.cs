using Application.Interfaces;
using Application.UseCase.Reservations.Commands;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public class PayCommandHandler : IPayCommandHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PayCommandHandler(
            IReservationRepository reservationRepository,
            ISeatRepository seatRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _reservationRepository = reservationRepository;
            _seatRepository = seatRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(PayCommand command)
        {
            // Validations outside transaction
            var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId);
            
            if (reservation == null)
                throw new Exception("Reserva no encontrada");

            if (reservation.UserId != command.UserId)
                throw new Exception("No tiene permiso para pagar esta reserva");

            if (reservation.Status != "Pending")
                throw new Exception("La reserva no está pendiente de pago");

            if (DateTime.UtcNow > reservation.ExpiresAt)
                throw new Exception("La reserva ha expirado");

            var seat = await _seatRepository.GetByIdAsync(reservation.SeatId);
            if (seat == null)
                throw new Exception("Butaca no encontrada");

            // Transactional phase
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Simulate payment gateway delay
                await Task.Delay(500); 

                // 1. Mark Reservation as Completed
                reservation.Status = "Completed";
                await _reservationRepository.UpdateAsync(reservation);

                // 2. Mark Seat as Sold
                seat.Status = "Sold";
                // Optionally increment version
                seat.Version++;
                await _seatRepository.UpdateAsync(seat);

                // 3. Register Audit Log
                var auditLog = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Action = "PAYMENT_SUCCESS",
                    EntityType = "Reservation",
                    EntityId = command.ReservationId.ToString(),
                    Details = $"Pago realizado exitosamente. Butaca {seat.Id} vendida.",
                    CreatedAt = DateTime.UtcNow
                };
                await _auditLogRepository.AddAsync(auditLog);

                // Commit
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();

                // Log failed payment attempt outside transaction
                var auditLog = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Action = "PAYMENT_FAILED",
                    EntityType = "Reservation",
                    EntityId = command.ReservationId.ToString(),
                    Details = $"Intento de pago fallido para la reserva {command.ReservationId}.",
                    CreatedAt = DateTime.UtcNow
                };
                await _auditLogRepository.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync();

                throw new Exception("Ocurrió un error al procesar el pago. Operación cancelada.");
            }
        }
    }
}
