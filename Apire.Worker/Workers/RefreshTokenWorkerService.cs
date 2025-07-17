using Apire.Worker.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Apire.Worker.Workers
{
    public class RefreshTokenWorkerService(ILogger<RefreshTokenWorkerService> logger, IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly ILogger<RefreshTokenWorkerService> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private DateTime _ultimaExecucao = DateTime.MinValue;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Refresh Worker Service iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow.Date;

                if (_ultimaExecucao.Date < now)
                {
                    try
                    {
                        _logger.LogInformation("Executando tarefa diária em: {Data}", now);

                        await ExecuteTarefaDiaria();
                        _ultimaExecucao = now;

                        _logger.LogInformation("Tarefa diária concluída em: {Data}", now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao executar tarefa diária");
                    }
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task ExecuteTarefaDiaria()
        {
            await Initialize();
            await Task.CompletedTask;
        }

        private async Task Initialize()
        {
            using var scope = _serviceProvider.CreateScope();
            var processingService = scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();

            await processingService.DeleteAllrefreshToken(CancellationToken.None);

            _logger.LogInformation("Refresh Worker Service finalizado.");
        }
    }
}
