# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade FlowerShop.Domain\FlowerShop.Domain.csproj
4. Upgrade FlowerShop.ServiceDefaults\FlowerShop.ServiceDefaults.csproj
5. Upgrade FlowerShop.Application\FlowerShop.Application.csproj
6. Upgrade FlowerShop.Infrastructure\FlowerShop.Infrastructure.csproj
7. Upgrade FlowerShop.VectorInit\FlowerShop.VectorInit.csproj
8. Upgrade FlowerShop.Api\FlowerShop.Api.csproj
9. Upgrade VectorEntities\VectorEntities.csproj
10. Upgrade FlowerShop.AppHost\FlowerShop.AppHost.csproj

## Settings

### Excluded projects

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

| Package Name                        | Current Version | New Version | Description                                   |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Aspire.Confluent.Kafka              |   9.5.2        |  13.0.0     | Recommended for .NET 10.0                     |
| Aspire.Hosting.AppHost              |   9.5.2        |  13.0.0     | Recommended for .NET 10.0                     |
| Aspire.Hosting.Azure.Functions      |   9.5.2-preview.1.25522.3 | 13.0.0-preview.1.25560.3 | Recommended for .NET 10.0                     |
| Aspire.Hosting.Kafka                |   9.5.2        |  13.0.0     | Recommended for .NET 10.0                     |
| Aspire.Microsoft.EntityFrameworkCore.SqlServer | 9.5.2 | 13.0.0     | Recommended for .NET 10.0                     |
| Microsoft.AspNetCore.OpenApi        |   9.0.7        |  10.0.0     | Recommended for .NET 10.0                     |
| Microsoft.Azure.Functions.Worker    |   2.1.0        |  2.51.0     | Recommended for .NET 10.0                     |
| Microsoft.Azure.Functions.Worker.ApplicationInsights | 2.0.0 | 2.50.0 | Recommended for .NET 10.0                     |
| Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore | 2.0.2 | 2.1.0 | Recommended for .NET 10.0                     |
| Microsoft.Azure.Functions.Worker.Sdk | 2.0.5 | 2.0.7 | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.9 | 10.0.0 | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore.Tools | 9.0.9 | 10.0.0 | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Http.Resilience | 9.4.0 | 10.0.0 | Recommended for .NET 10.0                     |
| Microsoft.Extensions.ServiceDiscovery | 9.3.1 | 10.0.0 | Recommended for .NET 10.0                     |
| Newtonsoft.Json                     | 13.0.3        | 13.0.4      | Recommended for .NET 10.0                     |
| OpenTelemetry.Instrumentation.AspNetCore | 1.12.0 | 1.14.0 | Recommended for .NET 10.0                     |
| OpenTelemetry.Instrumentation.Http  | 1.12.0        | 1.14.0      | Recommended for .NET 10.0                     |
| System.Memory.Data                  | 8.0.1         | 10.0.0      | Recommended for .NET 10.0                     |

### Project upgrade details

#### FlowerShop.Domain\FlowerShop.Domain.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### FlowerShop.ServiceDefaults\FlowerShop.ServiceDefaults.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.Http.Resilience should be updated from `9.4.0` to `10.0.0`
  - Microsoft.Extensions.ServiceDiscovery should be updated from `9.3.1` to `10.0.0`
  - OpenTelemetry.Instrumentation.AspNetCore should be updated from `1.12.0` to `1.14.0`
  - OpenTelemetry.Instrumentation.Http should be updated from `1.12.0` to `1.14.0`

#### FlowerShop.Application\FlowerShop.Application.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
NuGet packages changes:
  - System.Memory.Data should be updated from `8.0.1` to `10.0.0`

#### FlowerShop.Infrastructure\FlowerShop.Infrastructure.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
NuGet packages changes:
  - Aspire.Confluent.Kafka should be updated from `9.5.2` to `13.0.0`
  - Aspire.Microsoft.EntityFrameworkCore.SqlServer should be updated from `9.5.2` to `13.0.0`
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `9.0.9` to `10.0.0`
  - Microsoft.EntityFrameworkCore.Tools should be updated from `9.0.9` to `10.0.0`

#### FlowerShop.VectorInit\FlowerShop.VectorInit.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Azure.Functions.Worker should be updated from `2.1.0` to `2.51.0`
  - Microsoft.Azure.Functions.Worker.ApplicationInsights should be updated from `2.0.0` to `2.50.0`
  - Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore should be updated from `2.0.2` to `2.1.0`
  - Microsoft.Azure.Functions.Worker.Sdk should be updated from `2.0.5` to `2.0.7`
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4`

#### FlowerShop.Api\FlowerShop.Api.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
NuGet packages changes:
  - Microsoft.AspNetCore.OpenApi should be updated from `9.0.7` to `10.0.0`

#### VectorEntities\VectorEntities.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### FlowerShop.AppHost\FlowerShop.AppHost.csproj modifications
Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
NuGet packages changes:
  - Aspire.Hosting.AppHost should be updated from `9.5.2` to `13.0.0`
  - Aspire.Hosting.Azure.Functions should be updated from `9.5.2-preview.1.25522.3` to `13.0.0-preview.1.25560.3`
  - Aspire.Hosting.Kafka should be updated from `9.5.2` to `13.0.0`
