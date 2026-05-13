using Application.Interfaces;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public class ReleaseExpiredReservationsHandler : IReleaseExpiredReservationsHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReleaseExpiredReservationsHandler(
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

        /// <summary>
        /// Busca todas las reservas expiradas (Status == "Pending" y ExpiresAt < ahora),
        /// libera las butacas correspondientes y registra cada liberación en AuditLog.
        /// Todo dentro de una transacción ACID.
        /// </summary>
        /// <returns>Cantidad de reservas liberadas.</returns>
        public async Task<int> HandleAsync()
        {
            var expiredReservations = await _reservationRepository.GetExpiredPendingReservationsAsync(DateTime.UtcNow);
            int releasedCount = 0;

            // Si no hay reservas expiradas, no hacemos nada
            var reservationsList = new System.Collections.Generic.List<Reservation>(
                (System.Collections.Generic.IEnumerable<Reservation>)expiredReservations);

            if (reservationsList.Count == 0)
                return 0;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var reservation in reservationsList)
                {
                    // 1. Cambiar estado de la reserva a "Expired"
                    reservation.Status = "Expired";
                    await _reservationRepository.UpdateAsync(reservation);

                    // 2. Liberar la butaca (volver a "Available")
                    var seat = await _seatRepository.GetByIdAsync(reservation.SeatId);
                    if (seat != null)
                    {
                        seat.Status = "Available";
                        await _seatRepository.UpdateAsync(seat);
                    }

                    // 3. Registrar en AuditLog
                    var auditLog = new Audit_Log
                    {
                        Id = Guid.NewGuid(),
                        UserId = reservation.UserId,
                        Action = "Liberación de Butaca",
                        EntityType = "Reservation",
                        EntityId = reservation.Id.ToString(),
                        Details = $"Reserva expirada. Butaca {reservation.SeatId} liberada automáticamente.",
                        CreatedAt = DateTime.UtcNow,
                        MilisegundoExacto = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    };
                    await _auditLogRepository.AddAsync(auditLog);

                    releasedCount++;
                }

                // 4. Guardar y confirmar transacción
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return releasedCount;
            }
            catch (Exception)
            {
                // Rollback completo si algo falla
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
