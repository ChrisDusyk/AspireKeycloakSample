using AspireKeycloak.MessageContracts;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(
        "keycloak",
        realm: "aspire",
        options =>
        {
            options.RequireHttpsMetadata = false;
            options.Audience = "api-client";
        });

builder.Services.AddAuthorizationBuilder();

builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.AddAzureEventHubProducerClient("eventhubs", static config =>
{
    config.EventHubName = "keycloakhub";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (EventHubProducerClient eventHubClient) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    var reportEvent = new ReportingMessage()
    {
        Text = "Weather forecast generated",
        Timestamp = DateTimeOffset.UtcNow
    };
    using EventDataBatch batch = await eventHubClient.CreateBatchAsync();
    batch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reportEvent))));
    await eventHubClient.SendAsync(batch);

    return forecast;
})
    .RequireAuthorization();

app.MapDefaultEndpoints();

app.UseCors();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}