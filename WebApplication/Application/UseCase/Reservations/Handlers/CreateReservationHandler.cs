using Application.DTOs;
using Application.Interfaces;
using Application.UseCase.Reservations.Commands;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public class CreateReservationHandler : ICreateReservationHandler
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork; // ← reemplaza AppDbContext

        public CreateReservationHandler(
            ISeatRepository seatRepository,
            IReservationRepository reservationRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork) // ← reemplaza AppDbContext
        {
            _seatRepository = seatRepository;
            _reservationRepository = reservationRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReservationResponse> HandleAsync(CreateReservationCommand command)
        {
            // Iniciamos la transacción para asegurar ACID
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var seat = await _seatRepository.GetByIdAsync(command.SeatId);
                if (seat == null) throw new Exception("Butaca no encontrada");

                if (seat.Status != "Available")
                    throw new Exception("Butaca no disponible");

                // 1. Actualización del asiento
                seat.Status = "Reserved";
                seat.Version++; // IMPORTANTE: Incremento para concurrencia optimista
                await _seatRepository.UpdateAsync(seat);

                // 2. Creación de la reserva
                var reservation = new Reservation
                {
                    Id = Guid.NewGuid(),
                    SeatId = command.SeatId,
                    UserId = command.UserId,
                    Status = "Pending",
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };
                await _reservationRepository.AddAsync(reservation);

                // 3. Log de auditoría
                var auditLog = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Action = "RESERVE_SUCCESS",
                    EntityType = "Reservation",
                    EntityId = reservation.Id.ToString(),
                    Details = $"Butaca {command.SeatId} reservada por usuario {command.UserId}",
                    CreatedAt = DateTime.UtcNow
                };
                await _auditLogRepository.AddAsync(auditLog);

                // 4. Guardado atómico
                await _unitOfWork.SaveChangesAsync();

                // 5. Si todo OK, confirmamos transacción
                await _unitOfWork.CommitTransactionAsync();

                return new ReservationResponse {
                    Id = reservation.Id,
                    SeatId = reservation.SeatId,
                    UserId = reservation.UserId,
                    Status = reservation.Status,
                    ReservedAt = reservation.ReservedAt,
                    ExpiresAt = reservation.ExpiresAt
                };
            }
            catch (Exception)
            {
                // Si algo falla, ejecutamos el Rollback completo
                await _unitOfWork.RollbackTransactionAsync();
                throw; // Re-lanzamos para que el controlador lo maneje
            }
        }
    }
}
