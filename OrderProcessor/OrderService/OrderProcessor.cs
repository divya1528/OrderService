using ApplicationCore.Entities;
using Confluent.Kafka;
using Infrastructure.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService
{
    public class OrderProcessor : BackgroundService
    {
        private readonly ILogger<OrderProcessor> _logger;
        readonly ConsumerConfig _consumerConfig;
        IOrderRepository _orderRepository;
        public OrderProcessor(ConsumerConfig consumerConfig, IOrderRepository orderRepository, ILogger<OrderProcessor> logger)
        {
            _orderRepository = orderRepository;
            _consumerConfig = consumerConfig;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var c = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                c.Subscribe("orders");
                while (!stoppingToken.IsCancellationRequested)
                {
                    var cr = c.Consume();
                    //update status in db
                    Order newOrder = JsonSerializer.Deserialize<Order>(cr.Value);
                    await _orderRepository.Update(newOrder.Id, "Processed");
                    _logger.LogInformation("Worker running at: {time} with value {1}" , DateTimeOffset.Now, cr.Value);
                    //await Task.Delay(1000, stoppingToken);
                }
            }

        }
    }
}
