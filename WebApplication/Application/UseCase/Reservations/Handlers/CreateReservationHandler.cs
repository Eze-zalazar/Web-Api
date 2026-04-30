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
        private readonly IUnitOfWork _unitOfWork;

        public CreateReservationHandler(
            ISeatRepository seatRepository,
            IReservationRepository reservationRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _seatRepository = seatRepository;
            _reservationRepository = reservationRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReservationResponse> HandleAsync(CreateReservationCommand command)
        {
            // 1. Validaciones previas a la transacción
            //    Si fallan, se registra el intento en auditoría ANTES de lanzar la excepción.
            var seat = await _seatRepository.GetByIdAsync(command.SeatId);

            if (seat == null)
            {
                await RegisterFailedAttempt(command, "RESERVE_FAILED_NOT_FOUND",
                    $"Butaca {command.SeatId} no encontrada");
                throw new Exception("Butaca no encontrada");
            }

            if (seat.Status != "Available")
            {
                await RegisterFailedAttempt(command, "RESERVE_FAILED_UNAVAILABLE",
                    $"Butaca {command.SeatId} no disponible (estado actual: {seat.Status})");
                throw new Exception("Butaca no disponible");
            }

            // 2. Operación transaccional — solo se ejecuta si las validaciones pasaron
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Actualización del asiento
                seat.Status = "Reserved";
                seat.Version++; // Incremento para concurrencia optimista
                await _seatRepository.UpdateAsync(seat);

                // Creación de la reserva
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

                // Log de auditoría (éxito)
                var auditLog = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Action = "RESERVE_SUCCESS",
                    EntityType = "Seat",
                    EntityId = command.SeatId.ToString(),
                    Details = $"Butaca {command.SeatId} reservada por usuario {command.UserId}",
                    CreatedAt = DateTime.UtcNow
                };
                await _auditLogRepository.AddAsync(auditLog);

                // Guardado atómico
                await _unitOfWork.SaveChangesAsync();

                // Si todo OK, confirmamos transacción
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

        /// <summary>
        /// Registra un intento de reserva fallido en la tabla de auditoría.
        /// Se ejecuta fuera de la transacción principal para garantizar que
        /// el registro persista incluso cuando la operación es rechazada.
        /// </summary>
        private async Task RegisterFailedAttempt(CreateReservationCommand command, string action, string details)
        {
            var auditLog = new Audit_Log
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                Action = action,
                EntityType = "Seat",
                EntityId = command.SeatId.ToString(),
                Details = details,
                CreatedAt = DateTime.UtcNow
            };
            await _auditLogRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
