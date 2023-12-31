using Confluent.Kafka;
using MediaHarbor.Bot;
using MediaHarbor.Bot.Domain.DataLake;
using MediaHarbor.Bot.Options;
using MediaHarbor.Bot.UpdatePipelines;
using MediaHarbor.Logger;
using SimpleKafka;
using TBot.Asp.Client;

var builder = WebApplication.CreateBuilder(args);

var kafkaOptions = builder.Configuration.BindOptions<KafkaOptions>();
var redisOptions = builder.Configuration.BindOptions<RedisOptions>();
var harborOptions = builder.Configuration.BindOptions<MediaHarborOptions>();

builder.Services.AddKafkaProducer<string>(new ProducerConfig { BootstrapServers = kafkaOptions.BootstrapServers });
builder.Services.AddKafkaConsumersFactory();
builder.Services.AddDependencies();
builder.Logging.AddLogger(builder.Configuration, new KafkaSettings { BootstrapServers = kafkaOptions.BootstrapServers });

builder.AddTBot(botBuilder =>
{
    if (!harborOptions.EnableWebhook) {
        botBuilder.AddLongPoll();
    }
    
    botBuilder
        .AddRedisStore(redisOptions.ToString())
        .EnableCallLimiter()
        .AddUpdateServices()
        .AddService<ContentPipeline>();
});

var app = builder.Build();
var dataLake = app.Services.GetRequiredService<IDataLake>();
dataLake.CreateFolderIfNotExist(Constants.ContentFolder);

await app.Services.SubscribeHandlersAsync(new ConsumerConfig { BootstrapServers = kafkaOptions.BootstrapServers });

app.UseRouting();
if (harborOptions.EnableWebhook) {
    app.UseTBotRoute().WithUpdateEngine();
}
else {
    app.RunTBot().WithUpdateEngine();
}

await app.RunAsync();