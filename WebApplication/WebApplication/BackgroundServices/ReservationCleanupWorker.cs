using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication.BackgroundServices
{
    public class ReservationCleanupWorker : BackgroundService
    {
        private readonly ILogger<ReservationCleanupWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ReservationCleanupWorker(ILogger<ReservationCleanupWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Worker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Reservation Cleanup Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    await CleanupExpiredReservationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing Reservation Cleanup.");
                }

                // Wait 1 minute before checking again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Reservation Cleanup Worker is stopping.");
        }

        private async Task CleanupExpiredReservationsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                var seatRepository = scope.ServiceProvider.GetRequiredService<ISeatRepository>();
                var auditLogRepository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Need a way to get expired reservations. Since we don't have GetExpiredAsync in the interface yet, we will just fetch all pending ones and filter.
                // Or we can add a method to IReservationRepository.
                // For now, let's assume IReservationRepository needs a method. 
                // Wait, I will add a GetExpiredReservationsAsync method.
                var expiredReservations = await reservationRepository.GetExpiredReservationsAsync(DateTime.UtcNow);

                foreach (var reservation in expiredReservations)
                {
                    await unitOfWork.BeginTransactionAsync();
                    try
                    {
                        // Update reservation status
                        reservation.Status = "Cancelled";
                        await reservationRepository.UpdateAsync(reservation);

                        // Update seat status back to Available
                        var seat = await seatRepository.GetByIdAsync(reservation.SeatId);
                        if (seat != null)
                        {
                            seat.Status = "Available";
                            seat.Version++;
                            await seatRepository.UpdateAsync(seat);
                        }

                        // Log audit
                        var auditLog = new Audit_Log
                        {
                            Id = Guid.NewGuid(),
                            UserId = reservation.UserId,
                            Action = "RESERVATION_EXPIRED",
                            EntityType = "Seat",
                            EntityId = reservation.SeatId.ToString(),
                            Details = $"Reserva {reservation.Id} expiró. Butaca {reservation.SeatId} liberada.",
                            CreatedAt = DateTime.UtcNow
                        };
                        await auditLogRepository.AddAsync(auditLog);

                        await unitOfWork.SaveChangesAsync();
                        await unitOfWork.CommitTransactionAsync();

                        _logger.LogInformation("Reserva {ReservationId} cancelada por expiración.", reservation.Id);
                    }
                    catch (Exception ex)
                    {
                        await unitOfWork.RollbackTransactionAsync();
                        _logger.LogError(ex, "Error al cancelar la reserva expirada {ReservationId}", reservation.Id);
                    }
                }
            }
        }
    }
}
