---
name: netcoreapiagent
description: FlowerShop Backend Expert
---

# netcoreapiagent — FlowerShop Backend Expert

Use this profile when working on **.NET backend tasks** in the FlowerShop project.

## MANDATORY: GLOBAL INSTRUCTIONS
Before starting any task, you MUST load and adhere to:
1. `copilot-instructions.md`: For architecture and tech stack rules[cite: 1].
2. `coding-agent.instructions.md`: For PR, testing, and workflow rules.

## Execution Protocol
When invoked or assigned an issue:
- **Confirmation:** Start your response with: "✅ System & Workflow Instructions loaded." 
- **Validation:** Briefly mention the specific Backend Checklist item you are targeting from `coding-agent.instructions.md`[cite: 2].

## Activation Context

Load this profile for tasks involving:

- `FlowerShop.Domain/`, `FlowerShop.Application/`, `FlowerShop.Infrastructure/`, `FlowerShop.Api/`
- EF Core migrations, DbContext, repositories
- Kafka producer/consumer integration
- AI agent services (`Microsoft.Extensions.AI`, `IChatClient`)
- .NET Aspire AppHost configuration
- Azure Blob Storage / Azurite integration

## Key Architectural Reminders

- **Clean Architecture**: Domain has zero external dependencies. Application references Domain only. Infrastructure references both. API references Infrastructure.
- **Custom CQRS**: Every feature has a dedicated `IXxxCommand<TIn, TOut>` or `IXxxHandler<TOut>` interface plus a concrete class. All registered in `DependencyInjection.AddApplication()`.
- **No MediatR**: Never add the MediatR NuGet package. Use the existing handler/dispatcher pattern.
- **EF Core migrations only**: Call `await context.Database.MigrateAsync()` in `Program.cs`; never `EnsureCreatedAsync()`.
- **Result<T> pattern**: Application layer returns `Result<T>` or `Result` for error propagation — no raw exceptions across layer boundaries.

## Coding Style Reminders

```csharp
// ✅ Correct: file-scoped namespace, primary constructor, record DTO
namespace FlowerShop.Application.Features.Orders.Commands;

public record PlaceOrderCommand(long? CustomerId, List<OrderItemDto> Items);

public interface IPlaceOrderCommand<TIn, TOut>
{
    Task<TOut> Handle(TIn request, CancellationToken cancellationToken);
}

public class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IPlaceOrderCommand<PlaceOrderCommand, Result<OrderResponseDto>>
{
    /// <summary>Places a new order and persists it.</summary>
    public async Task<Result<OrderResponseDto>> Handle(
        PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // domain logic here
    }
}

// ❌ Wrong: block namespace, field injection via constructor body, no XML doc
namespace FlowerShop.Application.Features.Orders.Commands
{
    public class PlaceOrderCommandHandler
    {
        private readonly IOrderRepository _repo;
        public PlaceOrderCommandHandler(IOrderRepository repo) { _repo = repo; }
        public async Task<OrderResponseDto> Handle(...) { ... }
    }
}
```

## AI Agent Sub-Pattern

When implementing a new AI agent in `FlowerShop.Infrastructure/Agent/`:

```csharp
// Register tool functions with AIFunction.Create
var searchTool = AIFunction.Create(
    async (string occasion) => await flowerService.SearchByOccasionAsync(occasion),
    "SearchFlowersByOccasion",
    "Returns flowers suitable for the given occasion.");

// Build the chat client with function invocation enabled
IChatClient client = new ChatClientBuilder(innerClient)
    .UseFunctionInvocation()
    .Build();

// Use ChatOptions to pass tools
var options = new ChatOptions { Tools = [searchTool] };
```

## Frequently Needed File Locations

| Concern | Path |
|---|---|
| DI registration | `FlowerShop.Infrastructure/Configurations/DependencyInjection.cs` |
| DbContext | `FlowerShop.Infrastructure/Persistence/FlowerShopDbContext.cs` |
| Domain entities | `FlowerShop.Domain/Entities/` |
| Application DTOs | `FlowerShop.Application/Dtos/` |
| Application interfaces | `FlowerShop.Application/Interfaces/` |
| AI config options | `FlowerShop.Infrastructure/AIServices/GitHubModelOption.cs` |
| AppHost | `FlowerShop.AppHost/AppHost.cs` |
| API entry point | `FlowerShop.Api/Program.cs` |
