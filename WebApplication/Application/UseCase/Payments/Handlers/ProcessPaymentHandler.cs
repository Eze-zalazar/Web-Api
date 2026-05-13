using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.UseCase.Payments.Handlers
{
    public class ProcessPaymentHandler : IProcessPaymentHandler
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessPaymentHandler(
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

        public async Task<bool> HandleAsync(ProcessPaymentRequest request, int userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Obtener reserva
                var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId);
                if (reservation == null)
                    throw new Exception("Reservation not found.");

                if (reservation.Status != "Pending")
                    throw new Exception("Reservation is not pending payment.");

                // 2. Obtener butaca
                var seat = await _seatRepository.GetByIdAsync(reservation.SeatId);
                if (seat == null)
                    throw new Exception("Seat not found.");

                // 3. Simular procesamiento de pago (Acá iría la llamada a Stripe, MercadoPago, etc.)
                // Asumimos que el pago es exitoso

                // 4. Cambiar estado de la butaca a "Vendida"
                seat.Status = "Vendida";
                await _seatRepository.UpdateAsync(seat);

                // 5. Cambiar estado de la reserva a "Completada/Pagada"
                reservation.Status = "Completada/Pagada";
                await _reservationRepository.UpdateAsync(reservation);

                // 6. Registrar en AuditLog con milisegundo exacto
                var auditLog = new Audit_Log
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Action = "Pago Realizado",
                    EntityType = "Reservation",
                    EntityId = reservation.Id.ToString(),
                    Details = $"Payment method: {request.PaymentMethod}",
                    CreatedAt = DateTime.UtcNow, // DateTime.UtcNow captura hasta milisegundos (100-nanosecond intervals)
                    MilisegundoExacto = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                await _auditLogRepository.AddAsync(auditLog);

                // 7. Confirmar transacción
                await _unitOfWork.CommitTransactionAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                // 8. En caso de fallo en cualquier paso, hacer rollback completo
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Payment failed: {ex.Message}", ex);
            }
        }
    }
}
