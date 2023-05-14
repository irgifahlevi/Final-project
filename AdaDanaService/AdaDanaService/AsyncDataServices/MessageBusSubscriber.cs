using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaDanaService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AdaDanaService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private readonly ILogger<MessageBusSubscriber> _logger;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;
        private string _cashOutQueueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor,
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

            // Declare product
            _channel.ExchangeDeclare(exchange: "trigger_cashout_wallet", type: ExchangeType.Fanout);
            _cashOutQueueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _cashOutQueueName, exchange: "trigger_cashout_wallet", routingKey: "");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            //Console.WriteLine("RabbitMQ Connection Shutdown");
            _logger.LogInformation("RabbitMQ Connection Shutdown");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var walletConsumer = new EventingBasicConsumer(_channel);
            walletConsumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Cash out Event Received !");
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProccessEvent(notificationMessage);
            };
            _channel.BasicConsume(queue: _cashOutQueueName, autoAck: true, consumer: walletConsumer);


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