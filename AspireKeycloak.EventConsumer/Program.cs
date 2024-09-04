using AspireKeycloak.EventConsumer;
using Azure.Messaging.EventHubs.Consumer;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureBlobClient("checkpoints");
builder.AddAzureEventProcessorClient("eventhubs", static config =>
{
    config.EventHubName = "keycloakhub";
    //config.ConsumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();