using Aspire.Net.ApiService.Domain.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Aspire.Net.ApiService.Infrastrutura.Brokers
{
    public class PagamentoProducerMQ
    {
        private readonly ConnectionFactory _factory;

        public PagamentoProducerMQ(IOptions<RabbitMQSettings> options)
        {
            var config = options.Value;

            _factory = new ConnectionFactory()
            {
                HostName = config.HostName,
                UserName = config.UserName,
                Password = config.Password,
                VirtualHost = config.VirtualHost,
                Port = config.Port,
                Ssl = new SslOption
                {
                    Enabled = config.SslEnabled,
                    ServerName = config.SslServerName
                }
            };
        }

        public void EnviarMensagem(string mensagem)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "pagamento",
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(mensagem);

            channel.BasicPublish(
                exchange: "pagamento",
                routingKey: "pagamento.sucesso",
                basicProperties: null,
                body: body
            );
        }
    }
}
