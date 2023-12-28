using Confluent.Kafka;
using MediaHarbor.Bot;
using MediaHarbor.Bot.Domain.DataLake;
using MediaHarbor.Bot.UpdatePipelines;
using MediaHarbor.Logger;
using SimpleKafka;
using TBot.Asp.Client;
using TBot.Core.CallLimiter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaProducer<string>(new ProducerConfig { BootstrapServers = "localhost" });
builder.Services.AddKafkaConsumersFactory();
builder.Services.AddDependencies();
builder.Logging.AddLogger(builder.Configuration, new KafkaSettings { BootstrapServers = "localhost" });

builder.AddTBot(botBuilder =>
{
    botBuilder
        .AddLongPoll()
        .AddTBotStore(BotStoreType.Redis)
        .AddUpdateServices()
        .AddService<ContentPipeline>();
});

var app = builder.Build();
app.RunTBot().WithUpdateEngine();

var dataLake = app.Services.GetRequiredService<IDataLake>();
dataLake.CreateFolderIfNotExist(Constants.ContentFolder);

await app.Services.SubscribeHandlersAsync();
await app.RunAsync();