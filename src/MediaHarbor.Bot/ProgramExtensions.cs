using Confluent.Kafka;
using MediaHarbor.Bot.Domain.ContentProcessing;
using MediaHarbor.Bot.Domain.DataLake;
using MediaHarbor.Bot.Events;
using MediaHarbor.Bot.Handlers;
using MediaHarbor.Bot.Services.ContentProcessing;
using MediaHarbor.Bot.Services.ContentProcessing.TikTok;
using MediaHarbor.Bot.Services.DataLake;
using SimpleKafka.Interfaces;

namespace MediaHarbor.Bot;

public static class ProgramExtensions
{
    public static async Task SubscribeHandlersAsync(this IServiceProvider provider)
    {
        var logger  = provider.GetService<ILogger<Program>>();
        var kafkaConsumerFactory = provider.GetRequiredService<IKafkaConsumerFactory>();
        
        var consumerConfig = new ConsumerConfig { BootstrapServers = "localhost" };
        
        await kafkaConsumerFactory.SubscribeAsync<StartContentProcessEvent, ContentHandler>(consumerConfig);
        await kafkaConsumerFactory.SubscribeAsync<FinishContentProcessEvent, FinishContentHandler>(consumerConfig);
        logger?.LogInformation("Subscription to the event is complete");
    }

    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        return services
            .AddTransient<IDataLake, FileDataLake>()
            .AddTransient<ITelegramContentService, TelegramContentService>()
            .AddKeyedTransient<IContentProviderService, TikTokContentService>(Enum.GetName(ContentProvider.TikTok))
            .AddTransient<IContentService, ContentService>();
    }
}