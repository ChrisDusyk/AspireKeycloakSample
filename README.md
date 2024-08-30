# Example application using Keycloak

This is a sample application consisting of Dockerized [Keycloak](https://www.keycloak.org/), an API, and both a Blazor client and a React client, all orchestrated by [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview).

## Keycloak

[Keycloak](https://www.keycloak.org/) is an open-source identity provider that can be run in a container, making it easy to orchestrate and manage with Aspire. Keycloak can be run with a persistent disk in Docker, ensuring that running locally will be consistent and easy.

## Local setup

This is required to load the ASP.NET dev certificates for SSL

1. Navigate to `ApiService` project folder in command line
2. Run `dotnet dev-certs https -ep "{path to root folder}\ApsireKeycloak.React\certificates\aspnet_https.pem" --format pem -v -np`
