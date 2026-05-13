using Application.UseCase.Reservations.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Services
{
    /// <summary>
    /// Servicio en segundo plano que verifica cada 60 segundos si hay reservas expiradas
    /// y las libera automáticamente, devolviendo las butacas al estado "Available".
    /// </summary>
    public class ReservationExpirationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationExpirationWorker> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(60);

        public ReservationExpirationWorker(
            IServiceProvider serviceProvider,
            ILogger<ReservationExpirationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReservationExpirationWorker iniciado. Verificando cada {Interval} segundos.", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Creamos un scope nuevo para resolver los servicios Scoped (DbContext, repos, etc.)
                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IReleaseExpiredReservationsHandler>();

                    int released = await handler.HandleAsync();

                    if (released > 0)
                    {
                        _logger.LogInformation("Se liberaron {Count} reserva(s) expirada(s).", released);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar reservas expiradas.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("ReservationExpirationWorker detenido.");
        }
    }
}
