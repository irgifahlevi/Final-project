using BukaToko.Event;
using HotChocolate.Utilities;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BukaToko.ASyncService
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProccessor _eventProcessor;
        private readonly ILogger<MessageBusSubscriber> _logger;
        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;
        private string _walletTopupQueueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProccessor eventProcessor,
            ILogger<MessageBusSubscriber> logger)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _logger = logger;

            InitializeRabbitMQ();
        }
        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare topup wallet
            _channel.ExchangeDeclare(exchange: "trigger_topup_wallet", type: ExchangeType.Fanout);
            _walletTopupQueueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _walletTopupQueueName, exchange: "trigger_topup_wallet", routingKey: "");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("RabbitMQ Connection Shutdown");
            _logger.LogInformation("RabbitMQ Connection Shutdown");
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            // Register a consumer for the topup trigger
            var walletTopupConsumer = new EventingBasicConsumer(_channel);
            walletTopupConsumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Wallet Topup Event Received !");
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProccessEvent(notificationMessage);
            };
            _channel.BasicConsume(queue: _walletTopupQueueName, autoAck: true, consumer: walletTopupConsumer);

            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
    }
}
