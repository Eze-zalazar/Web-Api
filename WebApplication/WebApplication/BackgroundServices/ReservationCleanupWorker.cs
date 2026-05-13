using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.BackgroundServices
{
    public class ReservationCleanupWorker : BackgroundService
    {
        private readonly ILogger<ReservationCleanupWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ReservationCleanupWorker(ILogger<ReservationCleanupWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Worker iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredReservationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error inesperado al limpiar reservas expiradas.");
                }

                // Esperar 1 minuto antes de volver a chequear
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Reservation Cleanup Worker detenido.");
        }

        private async Task CleanupExpiredReservationsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var reservationRepo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
            var auditLogRepo = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

            var currentTime = DateTime.UtcNow;
            var expiredReservations = await reservationRepo.GetExpiredPendingReservationsAsync(currentTime);

            foreach (var reservation in expiredReservations)
            {
                if (stoppingToken.IsCancellationRequested) break;

                await unitOfWork.BeginTransactionAsync();

                try
                {
                    // 1. Cancelar Reserva
                    reservation.Status = "Expired";
                    await reservationRepo.UpdateAsync(reservation);

                    // 2. Liberar Butaca
                    if (reservation.Seat != null)
                    {
                        reservation.Seat.Status = "Available";
                        reservation.Seat.Version++;
                    }

                    // 3. Auditoría
                    var auditLog = new Audit_Log
                    {
                        Id = Guid.NewGuid(),
                        UserId = reservation.UserId,
                        Action = "Reservation Expired",
                        EntityType = "Reservation",
                        EntityId = reservation.Id.ToString(),
                        Details = $"Reserva expirada automáticamente. Butaca liberada (ID: {reservation.Seat?.Id}).",
                        CreatedAt = DateTime.UtcNow,
                        MilisegundoExacto = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    };

                    await auditLogRepo.AddAsync(auditLog);

                    // 4. Guardar y Confirmar
                    await unitOfWork.SaveChangesAsync();
                    await unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation($"Reserva expirada procesada: {reservation.Id}");
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, $"Error al intentar liberar la reserva expirada: {reservation.Id}");
                }
            }
        }
    }
}
