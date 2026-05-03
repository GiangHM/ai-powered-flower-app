# Copilot Instructions — FlowerShop

## Project Overview
Flower Shop e-commerce application built with:
- **Backend**: .NET 10, Clean Architecture, .NET Aspire, CQRS (custom dispatcher pattern — no MediatR), EF Core, SQL Server, Kafka, ChromaDB
- **Frontend**: Vue 3 + Vite + Tailwind CSS + Pinia
- **AI Stack**: Microsoft.Agents.AI + Microsoft.Extensions.AI + GitHub Models (OpenAI-compatible)
- **Infra**: .NET Aspire AppHost, Azurite (dev blob storage), Kafka container, ChromaDB container

## Repository Structure
```
flowershop/
  flowershopbackend/
    FlowerShop.Domain/          ← Entities, value objects, domain events, no external dependencies
    FlowerShop.Application/     ← CQRS handlers, DTOs, interfaces — uses custom dispatcher, no MediatR
    FlowerShop.Infrastructure/  ← EF Core, Blob Storage, Kafka, AI agents, external services
    FlowerShop.Api/             ← ASP.NET Core controllers, middleware, DI wiring
    FlowerShop.AppHost/         ← .NET Aspire orchestration
    FlowerShop.ServiceDefaults/ ← Shared Aspire service defaults
    FlowerShop.VectorInit/      ← Vector DB seeding (Azure Functions)
    VectorEntities/             ← Shared ChromaDB vector entity models
  flowershopspa/
    myflowershop/               ← Vue 3 + Vite + Tailwind frontend
net-ai-beginer/                 ← Introductory AI samples (separate from FlowerShop)
```

## Architecture Rules — MUST follow
- **Dependency direction**: Domain ← Application ← Infrastructure ← API. Never reverse.
- **No logic in controllers**: Controllers only validate input, dispatch commands/queries, return results.
- **CQRS strictly**: Commands (mutate state) and Queries (read state) are separate handlers.
- **No MediatR**: Use the existing custom dispatcher/handler pattern already in the codebase. If implementing a new dispatcher, follow a clean `ICommandHandler<TCommand, TResult>` / `IQueryHandler<TQuery, TResult>` interface pattern.
- **DTOs in Application layer**: Never expose Domain entities directly from API.
- **Interfaces in Application**: Define `IFlowerService`, `IOrderRepository`, `IEmailService` etc. in Application; implement in Infrastructure.
- **IImageStorageService always**: Never call Azure Blob SDK directly — always go through `IImageStorageService`.
- **Microsoft.Agents.AI only for NEW agents**: `WriterAgent.cs`, `WriterService.cs`, `GoogleTextSearchPlugin.cs` are commented out pending migration — do not delete them, do not uncomment them unless specifically tasked with migration. New agent code must use `AIFunction.Create()` and `ChatClientBuilder(...).UseFunctionInvocation()`.

## Backend Conventions (C#)
- Target framework: `net10.0`
- Nullable reference types: **enabled** (`<Nullable>enable</Nullable>`)
- Use **file-scoped namespaces**: `namespace FlowerShop.Application.Commands;`
- Use **primary constructors** where applicable
- Use **record types** for DTOs, Commands, and Queries
- All public API methods must have XML doc comments (`/// <summary>`)
- All new Application layer handlers must be registered via the custom dispatcher — no direct instantiation
- EF Core: use `MigrateAsync()` on startup — never `EnsureCreatedAsync()`
- All admin endpoints: `[Authorize(Roles = "Admin")]`
- All customer endpoints: `[Authorize]`
- Rate limiting applied to `/api/SemanticSearch` and `/api/Chat` endpoints

## Custom Dispatcher Pattern (replaces MediatR)
If the codebase has an existing dispatcher, follow it exactly.
If implementing from scratch, use this pattern:

```csharp
// Interfaces in Application layer
public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}

// Registration in Infrastructure DependencyInjection.cs
services.AddTransient<ICommandHandler<CreateFlowerCommand, FlowerResponseItem>, CreateFlowerCommandHandler>();
services.AddTransient<IQueryHandler<GetAllFlowersQuery, IEnumerable<FlowerResponseItem>>, GetAllFlowersQueryHandler>();

// Usage in controllers — inject handler directly, never instantiate
[HttpPost]
public async Task<IActionResult> Create(
    [FromBody] CreateFlowerDto dto,
    [FromServices] ICommandHandler<CreateFlowerCommand, FlowerResponseItem> handler)
{
    var result = await handler.Handle(new CreateFlowerCommand(dto));
    return Ok(result);
}
```

## Frontend Conventions (Vue 3 / TypeScript)
- Use **Composition API** (`<script setup>`) — no Options API
- Use **Pinia** for all shared state (cart, auth, UI state)
- All HTTP calls go through `http.services.ts` — never use `fetch` or `axios` directly in components
- Use **TypeScript** everywhere; no implicit `any`
- Services live in `src/services/`; models in `src/models/`; pages in `src/pages/`; components in `src/components/`
- Tailwind CSS for all styling — no inline styles, no scoped CSS unless unavoidable
- Vue Router guards protect `/admin` and `/profile` routes

## AI / Agent Development Conventions
- Use `Microsoft.Extensions.AI` (`IChatClient`) for all LLM calls — not raw HTTP to OpenAI
- Register tools with `AIFunction.Create(async (...) => { ... })` — no Semantic Kernel plugins
- Enable automatic tool-call loop: `new ChatClientBuilder(innerClient).UseFunctionInvocation().Build()`
- Stream responses via `IAsyncEnumerable<StreamingChatCompletionUpdate>` and Server-Sent Events (SSE) on the API side
- Always inject `IChatClient` via DI — never construct `OpenAIClient` directly in service classes
- GitHub Models endpoint: `https://models.inference.ai.azure.com` — configured via `appsettings.json` `GitHubModel` section

## Testing Conventions
- **Backend**: xUnit + Moq + FluentAssertions; test project per layer (`*.Tests` suffix)
- **Frontend**: Vitest + `@vue/test-utils`; tests colocated with source or in `__tests__/` directory
- Mock all infrastructure boundaries (repositories, HTTP clients, blob storage) in unit tests
- Use `WebApplicationFactory<Program>` for controller integration tests
- Target >80% coverage on Application + Domain layers

## Common Pitfalls to Avoid
- Do not add `Microsoft.SemanticKernel` NuGet packages to new code; existing SK references are legacy pending removal
- Do not use `EnsureCreatedAsync()` — use `MigrateAsync()` only
- Do not expose `IQueryable` outside repositories; return materialised collections
- Do not add `[AllowAnonymous]` to admin endpoints
- Do not store secrets in `appsettings.json`; use Aspire secret management or environment variables
- Do not add `Console.WriteLine` in production paths; use `ILogger<T>` injected via DI
