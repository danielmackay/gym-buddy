<!--
Sync Impact Report - Constitution v2.1.0
========================================
Version Change: 2.0.0 → 2.1.0
Rationale: MINOR - Expanded technology stack specifications with concrete tooling decisions

Modified Principles:
  - No principle changes
  
Updated Sections:
  - Frontend Technology Stack: Expanded with specific libraries and tools
    * Added Shadcn/ui for components
    * Added Zustand for state management
    * Added React Hook Form + Zod for forms
    * Added TanStack Query for API client
    * Added Auth0 React SDK for authentication
    * Added date-fns for date/time handling
    * Added Lucide React for icons
  - Backend Technology Stack: Added explicit database and auth specifications
    * Added SQL Server as database engine
    * Added Auth0 JWT validation for authentication
  - New Section: Deployment & Infrastructure
    * Frontend: Vercel
    * Backend: Azure Container App
    * Database: Azure SQL Database
    * Telemetry: Application Insights + OpenTelemetry

Removed Sections: N/A

Templates Status:
  ✅ .specify/templates/plan-template.md - No changes needed
  ✅ .specify/templates/spec-template.md - No changes needed
  ✅ .specify/templates/tasks-template.md - No changes needed

Follow-up TODOs:
  - Update frontend scaffolding to include all specified libraries
  - Configure Auth0 for both frontend and backend
  - Set up Azure infrastructure for deployment
  - Configure Application Insights and OpenTelemetry
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

### III. Testing on Demand

Tests are created when explicitly requested or when the complexity of the feature warrants test coverage. Tests are a valuable tool but not a mandatory first step.

**Test Types Available**:
- **Unit Tests**: Domain logic, value objects, entity invariants (no EF mocking needed)
- **Integration Tests**: API endpoints using TestContainers + Respawn against real database
- **Architecture Tests**: NetArchTest to enforce VSA and DDD patterns

When tests ARE written, they MUST:
- Be comprehensive for the feature being tested
- Run reliably without flakiness
- Use appropriate test type (unit vs integration vs architecture)
- Pass before merging to main branch

**Rationale**: Pragmatic testing approach allows faster iteration while maintaining quality through explicit test coverage decisions based on risk and complexity.

### IV. Mobile-First Design & Progressive Web App (PWA)

The frontend MUST be built as a Progressive Web App using Next.js following strict mobile-first design principles:

**Mobile-First Design (MANDATORY)**:
- Design for smallest screen sizes first (320px minimum width)
- Touch-friendly interface with minimum 44x44px touch targets
- Optimize for thumb-reachable zones on mobile devices
- Progressive enhancement: mobile core experience, enhanced for larger screens
- Performance budget: <3s initial load on 3G networks

**PWA Requirements**:
- **Installable**: MUST support "Add to Home Screen" functionality
- **Offline Capable**: Service workers for offline functionality where feasible
- **Responsive**: MUST work across phone, tablet, and desktop viewports
- **Performance**: Lighthouse PWA score MUST be 90+ before production deployment
- **App-like**: Navigation patterns and interactions feel native

**Rationale**: Users primarily access fitness apps on mobile devices. Mobile-first ensures optimal experience on the most constrained devices, while PWA provides native-like experience without app store friction.

### V. Observability & Developer Experience

**Backend**: Use .NET Aspire for orchestration, observability, and telemetry. All services MUST integrate with Aspire Dashboard for monitoring.

**Structured Logging**: Use Serilog with structured logging patterns. Log important business events, not implementation details.

**Development Speed**: 
- Aspire MUST auto-provision all infrastructure (database, migrations, seeding)
- TestContainers available for integration tests when needed
- FastEndpoints auto-discovery eliminates manual endpoint registration

**Rationale**: Fast feedback loops and clear visibility into system behavior accelerate development and reduce debugging time.

## Architecture & Technology Standards

### Backend Technology Stack

- **.NET 10**: Latest version of .NET
- **FastEndpoints**: REPR pattern for API endpoints (replaces Minimal APIs)
- **Entity Framework Core**: Data access with migrations and seeding
- **SQL Server**: Database engine
- **Aspire**: Orchestration, observability, and service discovery
- **FluentValidation**: Request validation
- **Ardalis.Specification**: Query patterns abstracted from EF Core
- **ErrorOr**: Fluent result pattern instead of exceptions
- **Vogen**: Strongly typed IDs
- **Bogus**: Fake data generation for seeding
- **Auth0 JWT validation**: Backend authentication

### Frontend Technology Stack

- **Next.js (latest)**: React framework with App Router
- **TypeScript**: Type-safe JavaScript
- **Tailwind CSS**: Utility-first CSS framework (mobile-first by default)
- **Shadcn/ui**: Component library built on Radix UI
- **next-pwa**: Progressive Web App support and service worker generation
- **Zustand**: Lightweight state management for gym app state
- **React Hook Form + Zod**: Form handling with type-safe validation
- **TanStack Query (React Query)**: Data fetching and caching for REST APIs
- **Auth0 React SDK (@auth0/auth0-react)**: Authentication integration
- **date-fns**: Date and time manipulation
- **Lucide React**: Icon library (used by Shadcn/ui)

### Deployment & Infrastructure

- **Frontend**: Vercel for Next.js application hosting
- **Backend**: Azure Container App for .NET API
- **Database**: Azure SQL Database
- **Telemetry**: Application Insights + OpenTelemetry for monitoring and observability

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
│   ├── tests/                    # All test projects (when tests are needed)
│   └── tools/                    # Aspire AppHost and migrations
└── frontend/                     # Frontend Next.js PWA
    ├── src/
    │   ├── app/                  # Next.js App Router
    │   ├── features/             # Feature slices (VSA)
    │   ├── components/           # Shared UI components
    │   └── lib/                  # Utilities and helpers
    └── public/                   # Static assets and PWA manifest
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

### Frontend Conventions

- **Server Components by Default**: Use Client Components only when interactivity required
- **Mobile-First Styling**: Start with mobile layouts, use Tailwind breakpoints for larger screens
- **Touch-First Interactions**: Minimum 44x44px touch targets, no hover-dependent UI
- **Co-location**: Feature-specific components within feature directories (VSA)
- **API Client**: Typed service layer for all backend communication
- **Form Validation**: Zod or similar for type-safe form validation
- **Accessibility**: MUST meet WCAG 2.1 AA standards
- **Performance**: Monitor Core Web Vitals, optimize for mobile networks

## Development Workflow

### Adding Backend Features

1. Use template: `dotnet new ssw-vsa-slice --feature {Name} --feature-plural {Names}`
2. Register strongly typed ID in `VogenEfCoreConverters`
3. Create migration: `dotnet ef migrations add {Name}Table --project src/WebApi/WebApi.csproj --output-dir Common/Database/Migrations`
4. Implement feature following VSA pattern
5. Add tests if explicitly requested or if feature complexity warrants it
6. Verify all tests pass before committing (if tests exist)

### Running the Application

```bash
cd src/api/tools/AppHost/
dotnet run
```
- Aspire Dashboard opens for observability
- API available at https://localhost:7255/swagger
- Database auto-provisioned and seeded

### Testing Strategy

Tests are written when explicitly requested or when feature complexity requires validation:

- **Unit Tests**: Fast, no database, test domain logic only
- **Integration Tests**: Real database via TestContainers, Respawn for cleanup
- **Architecture Tests**: Enforce naming conventions and dependencies
- **Frontend Tests**: Component tests with Testing Library, E2E with Playwright (when requested)

When tests exist, all tests MUST pass before merge to main branch.

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

**Version**: 2.1.0 | **Ratified**: 2026-01-16 | **Last Amended**: 2026-01-16
