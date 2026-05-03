---
applyTo: '**/*'
---
# Supplemental Workflow Rules
**Context:** These rules supplement `.github/copilot-instructions.md`. 
**Priority:** If a conflict occurs regarding PR/Issue workflow, these rules take precedence over the global instructions.

## Before Starting Any Task
- Read `.github/copilot-instructions.md` fully.
- Run `dotnet build src/flowershopbackend/flowershopbackend.sln` to confirm baseline compiles.
- Run `npm ci` inside `src/flowershopspa/myflowershop/` to confirm frontend installs.
- Identify which layer(s) are affected and verify the dependency direction is preserved.

## Preferred Task Workflow
1. **Understand** — read the issue body and linked Epic before writing any code.
2. **Plan** — identify files to create/modify; keep changes surgical (minimum diff).
3. **Implement** — follow conventions in `copilot-instructions.md`; one logical unit at a time.
4. **Test** — add or update xUnit tests for any new Application/Domain logic; add Vitest tests for new Vue components.
5. **Verify** — run `dotnet build` and `dotnet test`; run `npm run build` for frontend tasks.
6. **PR** — title format: `feat: <short description>` or `fix: <short description>`; reference the issue number.

## File Access Rules
The coding agent **may** read and write:

- All files under `src/flowershopbackend/`
- All files under `src/flowershopspa/myflowershop/`
- `README.md` for documentation updates
- `.github/` for CI/CD workflow files when explicitly tasked

The coding agent **must not** modify:

- `.github/agents/` instruction files (this file, sibling files)
- `.github/copilot-instructions.md` (unless specifically tasked with updating instructions)

## Adding a New Backend Feature (Checklist)
- [ ] Domain entity or value object in `FlowerShop.Domain/Entities/` or `ValueObject/`
- [ ] Interface(s) in `FlowerShop.Application/Interfaces/`
- [ ] DTOs (records) in `FlowerShop.Application/Dtos/`
- [ ] Command and/or Query handler in `FlowerShop.Application/Features/`
- [ ] Handler registration in `FlowerShop.Infrastructure/Configurations/DependencyInjection.cs` → `AddApplication()`
- [ ] Implementation class in `FlowerShop.Infrastructure/` (repository, service, etc.)
- [ ] Controller action in `FlowerShop.Api/Controllers/` — thin; no business logic
- [ ] xUnit test for the handler in a `*.Tests` project

## Adding a New Frontend Feature (Checklist)
- [ ] Service method in `src/services/` (use `http.services.ts` base client)
- [ ] Model/type in `src/models/`
- [ ] Pinia store (if shared state needed) in `src/stores/`
- [ ] Page component in `src/pages/` (if new route)
- [ ] Child components in `src/components/`
- [ ] Route entry in `src/router/index.ts` with auth guard if protected
- [ ] Build/Refactor Vue components using `<script setup>` and Tailwind CSS
- [ ] Vitest test for the component and/or service
- [ ] Update Pinia stores if global state is affected

## Branch and Commit Conventions
- Branch: `feature/<issue-number>-<short-slug>` or `fix/<issue-number>-<short-slug>`
- Commits: imperative mood, e.g. `add FlowerDetail page and GET /api/FlowerEshop/Flowers/{id}`
- PRs merge into `develop`; `develop` → `main` for releases only

## Environment Notes
- The `.NET Aspire` AppHost orchestrates SQL Server, Kafka, and ChromaDB containers locally.
- Azurite is added via `AddAzureStorage("storage").RunAsEmulator()` in AppHost.
- All connection strings are injected by Aspire — do not hard-code connection strings.
- GitHub Models token comes from environment variable `GitHubToken` or `appsettings.json` `GitHubModel:GithubToken`.