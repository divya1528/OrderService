using ApplicationCore.Abstract;
using ApplicationCore.Concrete;
using Confluent.Kafka;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;


namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             //.ConfigureAppConfiguration((hostContext, config) =>
             //{
             //    // Configure the app here.
             //    config
             //        .SetBasePath(Environment.CurrentDirectory)
             //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             //        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);

             //})
            .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IConfigSettings, ConfigSettings>();
                    services.AddSingleton<IContext, Context>();
                    services.AddSingleton<IOrderRepository, OrderRepository>();
                    services.AddSingleton(option =>
                    {
                        ConsumerConfig config = new ConsumerConfig();
                        config.BootstrapServers = "localhost:9092";//Configuration.GetValue<string>("ConsumerSettings:bootstrapservers");
                        config.GroupId = Guid.NewGuid().ToString(); //Configuration.GetValue<string>("ConsumerSettings:groupid");
                        config.AutoOffsetReset = AutoOffsetReset.Earliest;

                        return config;
                    });
                    services.AddHostedService<OrderProcessor>();
                });
    }
}
