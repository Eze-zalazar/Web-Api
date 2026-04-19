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
            var seat = await _seatRepository.GetByIdAsync(command.SeatId);
            if (seat == null)
                throw new Exception("Butaca no encontrada");

            if (seat.Status != "Available")
                throw new Exception("Butaca no disponible");

            seat.Status = "Reserved";
            await _seatRepository.UpdateAsync(seat);

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

            await _unitOfWork.SaveChangesAsync(); // ← usa la interfaz

            return new ReservationResponse
            {
                Id = reservation.Id,
                SeatId = reservation.SeatId,
                UserId = reservation.UserId,
                Status = reservation.Status,
                ReservedAt = reservation.ReservedAt,
                ExpiresAt = reservation.ExpiresAt
            };
        }
    }
}
