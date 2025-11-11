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

# Flower shop: Working with Semantic search, Chat completion service and Agents using Semantic Kernel and .NET Aspire (C#)

This project showcases what I've learned through self-study in generative AI 

A full-stack chat application built with .NET Aspire, Semantic Kernel, and GitHub models, featuring a Vue + Vite frontend. It integrates GitHub-hosted language models and Google Search for enhanced responses.

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

- `WebApi`: A .NET-based Web API that facilitates semantic search and chat interactions, leveraging .NET Aspire and Semantic Kernel. It exposes endpoints that enable seamless communication between the chat frontend and backend..
  
- `VueApp`: A Vue.js application that serves as the user interface for creating new flowers and interacting with the writer agent. It also supports semantic search functionality for exploring flower data.

- `Function App`: An Azure Function App responsible for initializing the vector database used to store flower information. It also updates the database whenever a new flower is created or updated
  
### Keyword search
![msedge_nDPHtDUdu4](https://github.com/user-attachments/assets/3b604646-fad4-49ba-bccd-505d201176b6)

### Semantic search function

#### Overiview architecture
<img width="636" height="346" alt="msedge_CWRklRHv0F" src="https://github.com/user-attachments/assets/4d9fe4db-b5cc-4d77-bccb-d8345028b8c3" />


#### Show case the semantic search

![msedge_GG9dlEmvAL](https://github.com/user-attachments/assets/a1367b59-340f-4b8f-b230-8df0b8135983)


### Writer Agent function

#### Overview architecture
<img width="645" height="387" alt="msedge_R6B2rpWP9V" src="https://github.com/user-attachments/assets/73307e6f-6cdd-46c3-8bfe-98079a63e10c" />


#### Showcase the writer agent 
![msedge_tfGFAbS2wJ](https://github.com/user-attachments/assets/90431b5b-6444-4d43-adc3-ae33edcf5bef)


### Aspire montinoring
<img width="954" height="450" alt="chrome_akR4mtAUKy" src="https://github.com/user-attachments/assets/a6aa8505-a8a4-4d47-8cdd-ec673a12a34d" />

<img width="960" height="456" alt="chrome_8bMELPvfyM" src="https://github.com/user-attachments/assets/96091356-e260-45b9-8db9-de511d965aae" />


## Getting Started

### Prerequisites for running experiments

- .NET 9 SDK
- VSCode or Visual studio

## Local Development

### Prerequisites for local development

- .NET 9 SDK
- VSCode or Visual Studio 2022 17.12
- [Node.js 22](https://docs.npmjs.com/downloading-and-installing-node-js-and-npm)

### Running the app

If using Visual Studio, open the solution file `flowershopbackend.sln` and launch/debug the `FlowerShop.AppHost` project.

For more information on local provisioning of Aspire applications, refer to the [Aspire Local Provisioning Guide](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/local-provisioning).

Example to add into a `appsettings.Development.json` in the `FlowerShop.AppHost` directory:

``` json
"Parameters": {
  "GithubToken": "Replace this with your OpenAI Api Key",
  "ChatModelId": "",
  "EmbeddingModel": "",
  "Endpoint": "https://models.inference.ai.azure.com",
  "GoogleApiKey": "",
  "GoogleSearchEngineId": ""
},
"ConnectionStrings": {
  "sql": ""
}
```


## Sample Product Data
You should feed data for your database after migrating. Sample data (sampledata/sqldata.sql)

Then trigger the function: "FlowerVectorDataInit" to init data for vector database


## Resources

- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/overview/)
- [Semantic Kernel Agent Framework Documentation](https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/?pivots=programming-language-csharp)
- [Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- The image data is sourced from the internet.

## License

This project is licensed under the terms of the MIT license. See the `LICENSE.md` file for the full license text.
