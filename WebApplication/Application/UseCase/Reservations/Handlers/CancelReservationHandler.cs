using Application.Interfaces;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public class CancelReservationHandler : ICancelReservationHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelReservationHandler(
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

        public async Task HandleAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetByIdWithSeatAsync(reservationId);
            if (reservation == null) throw new Exception("Reserva no encontrada.");

            if (reservation.Status != "Pending" && reservation.Status != "Reserved")
                throw new Exception("Solo se pueden cancelar reservas pendientes.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Liberar butaca
                var seat = reservation.Seat;
                seat.Status = "Available";
                seat.Version++;
                await _seatRepository.UpdateAsync(seat);

                // Cancelar reserva
                reservation.Status = "Cancelled";
                // En una implementación real se podría borrar o marcar como cancelada. 
                // Aquí la marcamos.

                // Auditoría
                var log = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = reservation.UserId,
                    Action = "RESERVE_CANCELLED_MANUAL",
                    EntityType = "Seat",
                    EntityId = seat.Id.ToString(),
                    Details = $"Usuario canceló manualmente la reserva {reservationId}",
                    CreatedAt = DateTime.UtcNow
                };
                await _auditLogRepository.AddAsync(log);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
