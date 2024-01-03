using Confluent.Kafka;
using MediaHarbor.Bot;
using MediaHarbor.Bot.Domain.DataLake;
using MediaHarbor.Bot.Options;
using MediaHarbor.Bot.UpdatePipelines;
using MediaHarbor.Logger;
using SimpleKafka;
using TBot.Asp.Client;
using TBot.Core.CallLimiter;

var builder = WebApplication.CreateBuilder(args);

var kafkaOptions = new KafkaOptions();
builder.Configuration.Bind(nameof(KafkaOptions), kafkaOptions);

builder.Services.AddKafkaProducer<string>(new ProducerConfig { BootstrapServers = kafkaOptions.BootstrapServers });
builder.Services.AddKafkaConsumersFactory();
builder.Services.AddDependencies();
builder.Logging.AddLogger(builder.Configuration, new KafkaSettings { BootstrapServers = kafkaOptions.BootstrapServers });

builder.AddTBot(botBuilder =>
{
    botBuilder
        .AddLongPoll()
        .AddTBotStore(BotStoreType.Redis)
        .EnableCallLimiter()
        .AddUpdateServices()
        .AddService<ContentPipeline>();
});

var app = builder.Build();
app.RunTBot().WithUpdateEngine();

var dataLake = app.Services.GetRequiredService<IDataLake>();
dataLake.CreateFolderIfNotExist(Constants.ContentFolder);

await app.Services.SubscribeHandlersAsync();
await app.RunAsync();