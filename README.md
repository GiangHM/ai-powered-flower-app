<!--
---
page_type: sample
languages:
- csharp
- typescript
products:
- Github model
- dotnet
- dotnet-aspire
---
-->

# Flower shop: A simple e-commerce, working with Vue, Microsfot Agent Framework and .NET Aspire (C#)

This project showcases what I've learned through self-study in generative AI 

A full-stack chat application built with .NET Aspire, Microsoft Agent Framework, and GitHub models, featuring a Vue + Vite frontend. It integrates GitHub-hosted language models and Google Search for enhanced responses.

- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites for running experiments](#prerequisites-for-running-experiments)
- [Local Development](#local-development)
  - [Prerequisites for local development](#prerequisites-for-local-development)
- [Sample Product Data](#sample-product-data)
- [Resources](#resources)
- [License](#license)

## Features

The application consists of 2 main projects: 

- `WebApi`: A .NET-based Web API that facilitates semantic search and chat interactions, leveraging .NET Aspire and Microsoft Agent Framework. It exposes endpoints that enable seamless communication between the chat frontend and backend..
  
- `VueApp`: A Vue.js application that serves as the user interface for creating new flowers and interacting with the writer agent. It also supports semantic search functionality for exploring flower data.

- `Function App`: An Azure Function App responsible for initializing the vector database used to store flower information. It also updates the database whenever a new flower is created or updated

Note: _The image data is sourced from the internet._

### Keyword search
![msedge_nDPHtDUdu4](https://github.com/user-attachments/assets/3b604646-fad4-49ba-bccd-505d201176b6)

### Semantic search function

#### Overiview architecture
<img width="636" height="346" alt="msedge_CWRklRHv0F" src="https://github.com/user-attachments/assets/4d9fe4db-b5cc-4d77-bccb-d8345028b8c3" />


#### Show case the semantic search

![msedge_GG9dlEmvAL](https://github.com/user-attachments/assets/a1367b59-340f-4b8f-b230-8df0b8135983)

### Sales Assistant

#### Overview architecture
<img width="560" height="230" alt="SalesAssistant_Arch" src="https://github.com/user-attachments/assets/41f4983c-3782-4d5a-aea2-67967746cd2a" />

#### Showcase the sales assistant
<img width="1886" height="940" alt="HomePage" src="https://github.com/user-attachments/assets/c2ae0463-c878-4900-987a-b34be51688b4" />


### Admin page
<img width="1882" height="763" alt="AdminPage" src="https://github.com/user-attachments/assets/101eeeda-cb81-4414-82f9-555501e5c491" />


### Writer Agent function

#### Overview architecture
<img width="645" height="387" alt="msedge_R6B2rpWP9V" src="https://github.com/user-attachments/assets/73307e6f-6cdd-46c3-8bfe-98079a63e10c" />

<img width="796" height="629" alt="WriterFlow" src="https://github.com/user-attachments/assets/fcf98893-7f09-451f-adfe-17e6f13f0e95" />


#### Showcase the writer agent 
![msedge_tfGFAbS2wJ](https://github.com/user-attachments/assets/90431b5b-6444-4d43-adc3-ae33edcf5bef)


### Aspire montinoring
<img width="954" height="450" alt="chrome_akR4mtAUKy" src="https://github.com/user-attachments/assets/a6aa8505-a8a4-4d47-8cdd-ec673a12a34d" />

<img width="960" height="456" alt="chrome_8bMELPvfyM" src="https://github.com/user-attachments/assets/96091356-e260-45b9-8db9-de511d965aae" />

<img width="1882" height="895" alt="Observability_Sales_Assistant_Trace" src="https://github.com/user-attachments/assets/7b529641-b134-4807-b7f4-e5fa5044da88" />


## Getting Started

### Prerequisites for running experiments

- .NET 10 SDK
- VSCode or Visual studio

## Local Development

### Prerequisites for local development

- .NET 10 SDK
- VSCode or Visual Studio 2022 17.12
- [Node.js 22](https://docs.npmjs.com/downloading-and-installing-node-js-and-npm)

### Running the app

If using Visual Studio, open the solution file `flowershopbackend.sln` and launch/debug the `FlowerShop.AppHost` project.

For more information on local provisioning of Aspire applications, refer to the [Aspire Local Provisioning Guide](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/local-provisioning).

Example to add into a `appsettings.Development.json` in the `FlowerShop.AppHost` directory:

``` json
"Parameters": {
  "GithubToken": "Replace this with your OpenAI Api Key",
  "ChatModelId": "gpt-4o-mini",
  "EmbeddingModel": "text-embedding-3-small",
  "Endpoint": "https://models.inference.ai.azure.com"
  "GoogleApiKey": "",
  "GoogleSearchEngineId": "",
  "VisionModelId": "gpt-4o"
},
"ConnectionStrings": {
  "sql": ""
}
```


## Database Migrations

The project uses **EF Core Migrations** for schema management. Migrations are stored in `FlowerShop.Infrastructure/Migrations/` and are applied automatically on startup via `MigrateAsync()`.

### Applying migrations (automatic)

Migrations are applied automatically every time the API starts. No manual steps are required in normal operation.

### Creating a new migration

When the domain model changes, generate a new migration from the solution root:

```bash
dotnet ef migrations add <MigrationName> \
  --project flowershopbackend/FlowerShop.Infrastructure \
  --startup-project flowershopbackend/FlowerShop.Api \
  --output-dir Migrations
```

### Applying migrations manually

To apply pending migrations to a target database without starting the API:

```bash
dotnet ef database update \
  --project flowershopbackend/FlowerShop.Infrastructure \
  --startup-project flowershopbackend/FlowerShop.Api
```

Set the `ConnectionStrings__sql` environment variable (or update `appsettings.Development.json`) to point to the target database before running the command.

### Rolling back a migration

```bash
dotnet ef database update <PreviousMigrationName> \
  --project flowershopbackend/FlowerShop.Infrastructure \
  --startup-project flowershopbackend/FlowerShop.Api
```

## Sample Product Data
You should feed data for your database after migrating. Sample data (sampledata/sqldata.sql)

Then trigger the function: "FlowerVectorDataInit" to init data for vector database


## Resources

- [Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Microsoft Agent Framework](https://learn.microsoft.com/en-us/agent-framework/overview/?pivots=programming-language-csharp)
- The image data is sourced from the internet.

## License

This project is licensed under the terms of the MIT license. See the `LICENSE.md` file for the full license text.
