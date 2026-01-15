<!--
Sync Impact Report - Constitution v1.0.0
========================================
Version Change: [unversioned template] → 1.0.0
Rationale: Initial constitution establishing core principles for Gym Buddy project

Modified Principles: N/A (initial creation)
Added Sections:
  - Core Principles (5 principles)
  - Architecture & Technology Standards
  - Development Workflow
  - Governance

Removed Sections: N/A

Templates Status:
  ✅ .specify/templates/plan-template.md - UPDATED: Added Option 2 with api/web structure for SPA+API projects
  ✅ .specify/templates/spec-template.md - reviewed, aligns with principles
  ✅ .specify/templates/tasks-template.md - reviewed, aligns with principles
  ⚠️  Future work: Frontend-specific checklist items for PWA once Next.js structure established

Follow-up TODOs:
  - Add PWA-specific checklist items once frontend scaffolding is complete
  - Consider adding mobile-first design principles to constitution in future minor version
  - Document testing strategy for Next.js components once framework is established
-->

# Gym Buddy Constitution

## Core Principles

### I. Vertical Slice Architecture (VSA)

**Backend (API)**: Every feature MUST be organized as a self-contained vertical slice in `src/api/src/WebApi/Features/{FeatureName}/`. Each slice MUST include all layers needed for that feature: endpoints, domain logic, data access, and validation.

**Frontend (when implemented)**: Features MUST be organized by user-facing capability, not by technical layer. Co-locate related components, hooks, services, and types within feature directories.

**Rationale**: VSA enables independent development and testing of features, reduces merge conflicts, and makes the codebase easier to navigate and maintain at scale.

### II. Domain-Driven Design (DDD) Patterns

All domain logic MUST reside in the Domain layer (`src/api/src/Domain/`). Business rules MUST be encapsulated in:

- **Entities**: Inherit from `Entity<TId>` or `AggregateRoot<TId>`
- **Value Objects**: Immutable types with validation (e.g., `Power`, `Mission`)
- **Strongly Typed IDs**: Use `[ValueObject<Guid>]` from Vogen (e.g., `HeroId`, `TeamId`)
- **Domain Events**: Raise via `AddDomainEvent()` on aggregates
- **Specifications**: Use Ardalis.Specification for query patterns

**Critical**: ALL new strongly typed IDs MUST be registered in `Common/Persistence/VogenEfCoreConverters.cs` with `[EfCoreConverter<YourId>]` attribute.

**Rationale**: DDD ensures business logic is testable, reusable, and protected from infrastructure concerns. Strongly typed IDs prevent primitive obsession and type-safety bugs.

### III. Test-First Development (NON-NEGOTIABLE)

Tests MUST be written BEFORE implementation. The red-green-refactor cycle is mandatory:

1. Write test (red)
2. Get user approval if applicable
3. Verify test fails
4. Implement minimal code to pass (green)
5. Refactor for clarity and maintainability

**Test Types Required**:
- **Unit Tests**: Domain logic, value objects, entity invariants (no EF mocking needed)
- **Integration Tests**: API endpoints using TestContainers + Respawn against real database
- **Architecture Tests**: NetArchTest to enforce VSA and DDD patterns

Tests are NOT optional - they define the requirements and ensure correctness.

**Rationale**: Test-first prevents over-engineering, documents expected behavior, and ensures high code quality from the start.

### IV. Progressive Web App (PWA) First

The frontend MUST be built as a Progressive Web App using Next.js with:

- **Mobile-First Design**: Primary target is phone/mobile devices
- **Installable**: MUST support "Add to Home Screen" functionality
- **Offline Capable**: Service workers for offline functionality where feasible
- **Responsive**: MUST work across phone, tablet, and desktop viewports
- **Performance**: Lighthouse PWA score MUST be 90+ before production deployment

**Rationale**: Users primarily access fitness apps on mobile devices. PWA provides native-like experience without app store friction.

### V. Observability & Developer Experience

**Backend**: Use .NET Aspire for orchestration, observability, and telemetry. All services MUST integrate with Aspire Dashboard for monitoring.

**Structured Logging**: Use Serilog with structured logging patterns. Log important business events, not implementation details.

**Development Speed**: 
- Aspire MUST auto-provision all infrastructure (database, migrations, seeding)
- TestContainers ensure integration tests run at unit test speed
- FastEndpoints auto-discovery eliminates manual endpoint registration

**Rationale**: Fast feedback loops and clear visibility into system behavior accelerate development and reduce debugging time.

## Architecture & Technology Standards

### Backend Technology Stack

- **.NET 9**: Latest LTS version of .NET
- **FastEndpoints**: REPR pattern for API endpoints (replaces Minimal APIs)
- **Entity Framework Core**: Data access with migrations and seeding
- **Aspire**: Orchestration, observability, and service discovery
- **FluentValidation**: Request validation
- **Ardalis.Specification**: Query patterns abstracted from EF Core
- **ErrorOr**: Fluent result pattern instead of exceptions
- **Vogen**: Strongly typed IDs
- **Bogus**: Fake data generation for seeding

### Frontend Technology Stack (when implemented)

- **Next.js (latest)**: React framework with App Router
- **TypeScript**: Type-safe JavaScript
- **Tailwind CSS**: Utility-first CSS framework
- **PWA Support**: next-pwa or similar for service worker generation
- **Mobile-First**: Design for small screens first, scale up

### Project Structure

```
src/
├── api/                          # Backend (existing)
│   ├── src/
│   │   ├── WebApi/               # API endpoints and features
│   │   │   ├── Features/         # Vertical slices
│   │   │   └── Common/           # Shared infrastructure
│   │   ├── Domain/               # Domain entities and logic
│   │   └── ServiceDefaults/      # Aspire configuration
│   ├── tests/                    # All test projects
│   └── tools/                    # Aspire AppHost and migrations
└── web/                          # Frontend (to be added)
    ├── src/
    │   ├── app/                  # Next.js App Router
    │   ├── features/             # Feature slices
    │   ├── components/           # Shared UI components
    │   └── lib/                  # Utilities and helpers
    └── public/                   # Static assets
```

### API Conventions

**FastEndpoints**:
- Each endpoint MUST be a separate class inheriting from `Endpoint<TRequest, TResponse>`
- Group endpoints by feature using `Group<TGroup>()` pattern
- MUST include `Summary<TEndpoint>` for OpenAPI documentation
- Validators MUST inherit from `Validator<TRequest>`
- Use `await Send.OkAsync()` and similar helpers for responses

**Entity Framework**:
- Entities MUST use strongly typed IDs registered in `VogenEfCoreConverters`
- Query patterns MUST use Specifications to abstract EF Core
- Migrations MUST be created for all schema changes
- Seeding MUST check for existing data before inserting

### Frontend Conventions (to be established)

- Server Components by default, Client Components only when needed
- Co-locate feature-specific components within feature directories
- API calls through typed service layer
- Form validation with Zod or similar
- Accessibility MUST meet WCAG 2.1 AA standards

## Development Workflow

### Adding Backend Features

1. Use template: `dotnet new ssw-vsa-slice --feature {Name} --feature-plural {Names}`
2. Register strongly typed ID in `VogenEfCoreConverters`
3. Create migration: `dotnet ef migrations add {Name}Table --project src/WebApi/WebApi.csproj --output-dir Common/Database/Migrations`
4. Write tests FIRST in `tests/WebApi.IntegrationTests/`
5. Implement feature following VSA pattern
6. Verify all tests pass before committing

### Running the Application

```bash
cd src/api/tools/AppHost/
dotnet run
```
- Aspire Dashboard opens for observability
- API available at https://localhost:7255/swagger
- Database auto-provisioned and seeded

### Testing Strategy

- **Unit Tests**: Fast, no database, test domain logic only
- **Integration Tests**: Real database via TestContainers, Respawn for cleanup
- **Architecture Tests**: Enforce naming conventions and dependencies
- All tests MUST pass before merge to main branch

### Documentation Requirements

**Create documentation for**:
- Significant architectural changes or new features
- Major refactorings affecting multiple modules
- New patterns or conventions
- Complex implementations needing explanation

**DO NOT create documentation for**:
- Minor bug fixes
- Small adjustments to existing code
- Changes adequately explained in commit messages

**Documentation location**: `src/api/docs/cli-tasks/{yyyyMMdd-II-XX-description}.md`

## Governance

**Constitution Authority**: This constitution supersedes all other development practices. When in doubt, follow these principles.

**Amendment Process**:
1. Propose amendment with rationale in GitHub issue or discussion
2. Require approval from project maintainers
3. Update constitution with new version number
4. Create migration plan if breaking changes to existing code
5. Update all related templates and documentation

**Compliance**:
- All pull requests MUST be reviewed for constitutional compliance
- Architecture tests enforce technical principles automatically
- Violations MUST be justified with explicit rationale in code comments
- Unjustified complexity or pattern violations MUST be rejected

**Versioning Policy**:
- **MAJOR**: Backward incompatible governance changes, principle removals/redefinitions
- **MINOR**: New principles added, materially expanded guidance
- **PATCH**: Clarifications, wording improvements, typo fixes

**Complexity Justification**: Any deviation from these principles MUST include:
- What principle is being violated
- Why the deviation is necessary
- Why simpler alternatives were rejected
- Plan to return to compliance (if temporary)

**Runtime Guidance**: For detailed implementation instructions, refer to `src/api/AGENTS.md`.

**Version**: 1.0.0 | **Ratified**: 2026-01-16 | **Last Amended**: 2026-01-16
