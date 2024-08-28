var builder = DistributedApplication.CreateBuilder(args);

var adminUsername = builder.AddParameter("adminUsername", secret: true);
var adminPassword = builder.AddParameter("adminPassword", secret: true);

var keycloak = builder
    .AddKeycloak("keycloak", port: 8080, adminUsername: adminUsername, adminPassword: adminPassword)
    .WithDataVolume();

var apiService = builder.AddProject<Projects.AspireKeycloak_ApiService>("apiservice")
    .WithReference(keycloak);

builder.AddProject<Projects.AspireKeycloak_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(keycloak);

builder.AddNpmApp("react", "../AspireKeycloak.React", scriptName: "dev")
    .WithReference(apiService)
    .WithEnvironment("VITE_API_URL", apiService.GetEndpoint("https"))
    .WithHttpsEndpoint(isProxied: false, port: 3000)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();