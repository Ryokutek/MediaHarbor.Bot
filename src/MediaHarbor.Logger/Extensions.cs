using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Kafka;

namespace MediaHarbor.Logger;

public static class Extensions
{
    public static void AddLogger(
        this ILoggingBuilder loggingBuilder, 
        IConfiguration configuration, 
        KafkaSettings kafkaSettings, 
        string topic = "media_harbor")
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddConsole();
        loggingBuilder.AddSerilog(new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Kafka(
                bootstrapServers: kafkaSettings.BootstrapServers,
                formatter: new GraylogFormatter(),
                securityProtocol: SecurityProtocol.Plaintext,
                saslMechanism: SaslMechanism.Plain,
                topic: $"{topic}.logs"
            )
            .Enrich.WithThreadId()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithAssemblyName()
            .Enrich.WithAssemblyVersion()
            .Enrich.WithClientIp()
            .CreateLogger());
    }
}