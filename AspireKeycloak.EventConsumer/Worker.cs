using AspireKeycloak.MessageContracts;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using System.Text;
using System.Text.Json;

namespace AspireKeycloak.EventConsumer;

public class Worker(ILogger<Worker> logger, EventProcessorClient eventProcessor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting event processor.");

        eventProcessor.ProcessEventAsync += ProcessEventHandler;
        eventProcessor.ProcessErrorAsync += ProcessErrorHandler;

        await eventProcessor.StartProcessingAsync(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Stopping event processor.");

        await eventProcessor.StopProcessingAsync(stoppingToken);

        eventProcessor.ProcessEventAsync -= ProcessEventHandler;
        eventProcessor.ProcessErrorAsync -= ProcessErrorHandler;
    }

    private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        var body = eventArgs.Data.EventBody;
        var text = Encoding.UTF8.GetString(body.ToArray());
        var message = JsonSerializer.Deserialize<ReportingMessage>(text);

        if (message is null)
        {
            logger.LogError("Failed to deserialize message: {Text}", text);
        }
        else
        {
            logger.LogInformation("Received message: {Text}", message.Text);
        }

        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        logger.LogError(eventArgs.Exception, "Error processing event.");
        return Task.CompletedTask;
    }
}