using Apire.Worker.Domain.Configurations;
using Apire.Worker.Domain.Entities;
using Apire.Worker.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Apire.Worker.Workers
{
    public class PaymentWorkerService : BackgroundService
    {
        private readonly ILogger<PaymentWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _rabbitConfig;
        private IConnection? _connection;
        private IModel? _channel;

        public PaymentWorkerService(
            ILogger<PaymentWorkerService> logger,
            IServiceProvider serviceProvider,
            IOptions<RabbitMQSettings> rabbitConfig)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitConfig = rabbitConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000, stoppingToken);

            _logger.LogInformation("Payment Worker Service iniciado.");

            try
            {
                InitializeRabbitMQ();
                ConsumeMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar o worker");
            }

            // Mantém o worker rodando
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitConfig.HostName,
                UserName = _rabbitConfig.UserName,
                Password = _rabbitConfig.Password,
                VirtualHost = _rabbitConfig.VirtualHost,
                Port = _rabbitConfig.Port,
                Ssl = new SslOption
                {
                    Enabled = _rabbitConfig.SslEnabled,
                    ServerName = _rabbitConfig.SslServerName
                }
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declarar exchange e queue
            _channel.ExchangeDeclare(_rabbitConfig.ExchangeName, ExchangeType.Direct, true);
            _channel.QueueDeclare(_rabbitConfig.QueueName, true, false, false, null);
            _channel.QueueBind(_rabbitConfig.QueueName, _rabbitConfig.ExchangeName, _rabbitConfig.RoutingKey);

            _logger.LogInformation("RabbitMQ inicializado com sucesso");
        }

        private void ConsumeMessages(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Mensagem recebida: {Message}", message);

                try
                {
                    var payment = JsonSerializer.Deserialize<PaymentMessage>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (payment != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var processingService = scope.ServiceProvider.GetRequiredService<IPaymentProcessingService>();

                        await processingService.ProcessPaymentAsync(payment);

                        // Acknowledger a mensagem após processamento bem-sucedido
                        _channel?.BasicAck(ea.DeliveryTag, false);
                        _logger.LogInformation("Mensagem processada e confirmada");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem: {Message}", message);

                    // Rejeitar a mensagem e reenviar para a fila (ou dead letter queue)
                    _channel?.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(_rabbitConfig.QueueName, false, consumer);
            _logger.LogInformation("Consumidor configurado para a fila: {QueueName}", _rabbitConfig.QueueName);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
