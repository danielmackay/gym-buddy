# gym-buddy Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-01-17

## Active Technologies

### Backend (.NET 10)
- **Framework**: .NET 10, FastEndpoints, Entity Framework Core
- **Architecture**: Vertical Slice Architecture (VSA) with Domain-Driven Design (DDD)
- **Database**: SQL Server (Azure SQL Database in production)
- **Validation**: FluentValidation, ErrorOr
- **Testing**: xUnit, TestContainers, Respawn, NetArchTest
- **Orchestration**: .NET Aspire (local development)
- **Observability**: Serilog, OpenTelemetry, Application Insights

### Frontend (Next.js 16)
- **Framework**: Next.js 16 (App Router), React 19, TypeScript 5
- **Styling**: Tailwind CSS v4 (mobile-first)
- **PWA**: next-pwa with service workers
- **UI Components**: Shadcn/ui (Radix UI primitives)
- **State Management**: Zustand with localStorage persistence
- **Forms**: React Hook Form + Zod validation
- **Data Fetching**: TanStack Query (React Query)
- **Utilities**: date-fns, Lucide React icons, dnd-kit (drag and drop)
- **Authentication**: Simple user selection (Auth0 planned for future)

## Project Structure

```text
tools/                            # Aspire AppHost, MigrationService
├── AppHost/
└── MigrationService/
src/
├── api/                          # Backend (.NET 10)
│   ├── src/
│   │   ├── WebApi/               # API endpoints and features
│   │   │   ├── Features/         # Vertical slices (Users, Exercises, WorkoutPlans)
│   │   │   └── Common/           # Shared infrastructure
│   │   ├── Domain/               # Domain entities and logic (DDD)
│   │   └── ServiceDefaults/      # Aspire configuration
│   └── tests/                    # Unit, Integration, Architecture tests
└── frontend/                     # Frontend (Next.js 16 PWA)
    ├── src/
    │   ├── app/                  # Next.js App Router (pages)
    │   ├── features/             # Feature slices (admin, trainer, exercise-library, workout-plans)
    │   ├── components/           # Shared UI components (Shadcn/ui)
    │   └── lib/                  # API client, stores, validation, utils
    ├── public/                   # PWA manifest, icons, static assets
    ├── next.config.ts            # Next.js + PWA configuration
    ├── tailwind.config.ts        # Tailwind CSS v4 configuration
    └── package.json              # Frontend dependencies
```

## Commands

### Full Stack Development
```bash
# Start entire stack (Backend API + DB + Migrations + Frontend) with Aspire
aspire run

# This will:
# - Start the backend API with Aspire dashboard
# - Provision and run SQL Server database
# - Run database migrations and seeding
# - Start the Next.js frontend dev server
# - Open Aspire Dashboard for observability
```

### Backend Only
```bash
# Start backend stack only (API + DB + Migrations)
cd tools/AppHost
dotnet run

# Add EF Core migration
dotnet ef migrations add MigrationName --project src/api/src/WebApi/WebApi.csproj --startup-project src/api/src/WebApi/WebApi.csproj --output-dir Common/Database/Migrations

# Run all tests
cd src/api
dotnet test

# Run specific test project
dotnet test tests/WebApi.IntegrationTests/
```

### Frontend Only
```bash
cd src/frontend

# Install dependencies
npm install

# Development server
npm run dev

# Production build (MUST use --webpack flag due to next-pwa compatibility with Turbopack)
npm run build -- --webpack

# Start production build
npm start

# Lint
npm run lint
```

### API Endpoints
- **Swagger/OpenAPI**: https://localhost:7255/scalar/v1 (when backend is running)
- **Dev Server**: https://localhost:7255
- **Frontend Dev**: http://localhost:3000

## Code Style

### Backend
- **Architecture**: Vertical Slice Architecture (VSA) - features in `Features/{FeatureName}/`
- **Domain**: DDD patterns - entities, value objects, strongly typed IDs (Vogen), domain events
- **Endpoints**: FastEndpoints pattern - `Endpoint<TRequest, TResponse>` with validators
- **Validation**: FluentValidation for request validation, display all errors to users
- **Error Handling**: ErrorOr pattern for domain errors, avoid throwing exceptions
- **Testing**: Unit tests for domain, Integration tests with TestContainers, Architecture tests

### Frontend
- **Architecture**: Feature-based organization (VSA) - features in `features/{feature-name}/`
- **Mobile-First**: Design for 320px+ screens, 44x44px minimum touch targets
- **PWA**: Progressive Web App with offline capability, installable, Lighthouse 90+ score
- **Components**: Server Components by default, Client Components only when needed
- **Styling**: Tailwind CSS v4 with utility-first approach, CSS variables for theming
- **Forms**: React Hook Form + Zod for type-safe validation
- **Data**: TanStack Query for server state, Zustand for client state
- **Accessibility**: WCAG 2.1 AA compliance

## Recent Changes

- 2026-01-17: Phase 1 & 2 Complete - Project setup and shared frontend foundation
  - ✅ Backend: Admin user seeding added to MigrationService
  - ✅ Frontend: Next.js 16 + PWA initialized with all dependencies
  - ✅ Frontend: Base API client with error handling
  - ✅ Frontend: Type definitions (User, Exercise, WorkoutPlan)
  - ✅ Frontend: Zod validation schemas
  - ✅ Frontend: Zustand user store with persistence
  - ✅ Frontend: 10 Shadcn/ui components + custom components
  - ✅ Frontend: Root layout, user selection page, navigation
  - ✅ Build: Production build verified (using --webpack flag)

## Implementation Status

**Completed**: Phase 1 (T001-T020) + Phase 2 (T020-T050) = 50/181 tasks

**Next**: Phase 3 (T051-T073) - User Story 1: Admin Creates Trainers
- Backend Users feature slice with endpoints
- Frontend admin pages and components
- Integration tests

See `specs/001-gym-workout-pwa/` for detailed specification and task breakdown.

## Key Configuration Notes

### Frontend Build
⚠️ **IMPORTANT**: Due to next-pwa compatibility with Turbopack, MUST use webpack for builds:
```bash
npm run build -- --webpack
```

### Environment Variables
**Frontend**:
- `.env.local` (development): `NEXT_PUBLIC_API_URL=https://localhost:7255`
- `.env.production` (production): `NEXT_PUBLIC_API_URL=https://api.gymbuddy.com`

**Backend**:
- Configured via Aspire in `tools/AppHost/Program.cs`
- Connection strings managed by Aspire (SQL Server in Docker/Podman for local dev)

### Authentication
**Current**: Simple user selection dropdown (no auth)
**Future**: Auth0 integration planned (frontend: @auth0/auth0-react, backend: JWT validation)

## References

- **Constitution**: `.specify/memory/constitution.md` - Core principles and architectural standards
- **Backend Guide**: `src/api/AGENTS.md` - Detailed VSA/DDD implementation patterns
- **Feature Spec**: `specs/001-gym-workout-pwa/spec.md` - Full feature specification
- **Implementation Plan**: `specs/001-gym-workout-pwa/plan.md` - Technical architecture and design
- **Tasks**: `specs/001-gym-workout-pwa/tasks.md` - Detailed task breakdown (181 tasks)

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
