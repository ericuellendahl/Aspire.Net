namespace Apire.Worker.Domain.Configurations
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public bool SslEnabled { get; set; }
        public string SslServerName { get; set; }
        public string ExchangeName { get; set; } 
        public string QueueName { get; set; }    
        public string RoutingKey { get; set; }   
    }
}
