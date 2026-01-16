# Implementation Plan: Gym Workout Tracking PWA

**Branch**: `001-gym-workout-pwa` | **Date**: Sat Jan 17 2026 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-gym-workout-pwa/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Create a mobile-first Progressive Web App (PWA) using Next.js 16 for gym workout tracking, with admin and trainer-facing features. The frontend will connect to an existing .NET backend API that uses FastEndpoints, Vertical Slice Architecture (VSA), and the existing Domain model. New backend endpoints will be created for user management, exercise library, and workout plan management. The application will support offline viewing of cached data and provide role-based dashboards for admins and trainers.

## Technical Context

**Language/Version**: 
- **Frontend**: TypeScript with Next.js 16 (React 19), Node.js 20+
- **Backend**: C# with .NET 10

**Primary Dependencies**:
- **Frontend**: Next.js 16, Tailwind CSS, Shadcn/ui, Zustand, React Hook Form, Zod, TanStack Query, next-pwa, date-fns, Lucide React
- **Backend**: FastEndpoints, Entity Framework Core, FluentValidation, ErrorOr, Vogen, Ardalis.Specification, Aspire

**Storage**: 
- **Backend**: SQL Server via Azure SQL Database (managed by EF Core migrations)
- **Frontend**: IndexedDB for offline PWA caching (via next-pwa service workers)

**Testing**: 
- **Backend**: Unit tests (Domain logic), Integration tests (TestContainers), Architecture tests (NetArchTest) - created on demand
- **Frontend**: Component tests (React Testing Library), E2E tests (Playwright) - created on demand per Constitution principle III

**Target Platform**: 
- **Frontend**: Modern web browsers (Chrome, Safari, Firefox, Edge) with PWA support, primarily mobile devices (iOS 15+, Android 10+)
- **Backend**: Linux containers on Azure Container App

**Project Type**: Web application (SPA + API) - Option 2 from template

**Performance Goals**:
- Frontend initial load <3s on 3G networks (mobile-first requirement)
- Backend API response time <200ms p95
- Lighthouse PWA score 90+ before production
- Support 100+ concurrent users without degradation

**Constraints**:
- Mobile-first design with 320px minimum width
- Touch-friendly UI with minimum 44x44px touch targets
- No authentication in initial release (simple user selection)
- Offline capability limited to viewing cached data
- All validation errors from FluentValidation and Domain must be shown to users
- Generic error messages for non-validation errors

**Scale/Scope**:
- 5 user stories (admin/trainer features only)
- ~15-20 frontend components
- ~8-10 backend endpoints across 3 feature slices (Users, Exercises, WorkoutPlans)
- 4 domain aggregates (User, Exercise, WorkoutPlan, PlannedExercise)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### ✅ Principle I: Vertical Slice Architecture (VSA)

**Backend Compliance**: 
- ✅ New features will be organized in `src/api/src/WebApi/Features/{Users,Exercises,WorkoutPlans}/`
- ✅ Each slice includes endpoints, validators, and feature-specific logic
- ✅ Follows existing pattern from Heroes and Teams features

**Frontend Compliance**:
- ✅ Features organized by user capability in `src/frontend/src/features/{admin,trainer,exercise-library,workout-plans}/`
- ✅ Co-location of components, hooks, services, and types within feature directories
- ✅ Shared UI components in `src/frontend/src/components/`

**Status**: ✅ PASS - Full compliance with VSA for both frontend and backend

### ✅ Principle II: Domain-Driven Design (DDD) Patterns

**Compliance**:
- ✅ Using existing Domain model in `src/api/src/Domain/`
- ✅ Entities: User, Exercise, WorkoutPlan, PlannedExercise (already exist)
- ✅ Value Objects: UserId, ExerciseId, WorkoutPlanId, PlannedExerciseId, Duration, Weight (already defined with Vogen)
- ✅ All strongly typed IDs already registered in `VogenEfCoreConverters.cs`
- ✅ Domain errors: UserErrors, ExerciseErrors, WorkoutPlanErrors (already exist)
- ✅ Specifications: UserByIdSpec, ExerciseByIdSpec, WorkoutPlanByIdSpec (already exist)

**Note**: No new domain entities needed - all required entities exist in Domain layer

**Status**: ✅ PASS - Using existing DDD-compliant domain model

### ✅ Principle III: Testing on Demand

**Approach**:
- ⚠️ Tests will be created on demand per constitution
- ⚠️ Initial implementation will focus on functionality delivery
- ⚠️ Test coverage will be added when explicitly requested or when feature complexity warrants

**Status**: ✅ PASS - Following constitution guidance for pragmatic testing

### ✅ Principle IV: Mobile-First Design & PWA

**Frontend Compliance**:
- ✅ Mobile-first design with 320px minimum width
- ✅ Touch-friendly interface with 44x44px minimum touch targets
- ✅ Progressive Web App using next-pwa
- ✅ Service workers for offline capability
- ✅ Tailwind CSS for responsive, mobile-first styling
- ✅ Target Lighthouse PWA score 90+
- ✅ Performance budget <3s initial load on 3G

**Status**: ✅ PASS - Full PWA and mobile-first compliance

### ✅ Principle V: Observability & Developer Experience

**Backend Compliance**:
- ✅ Aspire for orchestration and observability (already configured)
- ✅ FastEndpoints auto-discovery (already configured)
- ✅ Database auto-provisioning via Aspire (already working)
- ✅ Structured logging with Serilog (infrastructure exists)

**Frontend Compliance**:
- ✅ TanStack Query for data fetching and caching
- ✅ Clear error boundaries for user-facing errors
- ✅ Development server with hot reload (Next.js default)

**Status**: ✅ PASS - Full observability compliance

### Summary

**Overall Status**: ✅ **ALL GATES PASSED**

No constitution violations. All principles are fully satisfied by the proposed architecture.

**Complexity Tracking**: N/A - No violations to justify

## Project Structure

### Documentation (this feature)

```text
specs/001-gym-workout-pwa/
├── spec.md              # Feature specification (already created)
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
│   ├── users-api.yaml   # User management endpoints (OpenAPI)
│   ├── exercises-api.yaml   # Exercise library endpoints (OpenAPI)
│   └── workout-plans-api.yaml   # Workout plan endpoints (OpenAPI)
├── checklists/
│   └── requirements.md  # Specification quality checklist (already created)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/
├── api/                          # Backend (existing - will be extended)
│   ├── src/
│   │   ├── WebApi/               # API endpoints and features
│   │   │   ├── Features/         # Vertical slices
│   │   │   │   ├── Heroes/       # [EXISTING] Example feature
│   │   │   │   ├── Teams/        # [EXISTING] Example feature
│   │   │   │   ├── Users/        # [NEW] User management (Admin creates trainers, Trainer creates clients)
│   │   │   │   │   ├── Endpoints/
│   │   │   │   │   │   ├── CreateTrainer.cs
│   │   │   │   │   │   ├── CreateClient.cs
│   │   │   │   │   │   ├── ListTrainers.cs
│   │   │   │   │   │   ├── ListClients.cs
│   │   │   │   │   │   ├── UpdateUser.cs
│   │   │   │   │   │   └── GetUser.cs
│   │   │   │   │   ├── Validators/
│   │   │   │   │   │   ├── CreateTrainerValidator.cs
│   │   │   │   │   │   ├── CreateClientValidator.cs
│   │   │   │   │   │   └── UpdateUserValidator.cs
│   │   │   │   │   └── UsersFeature.cs
│   │   │   │   ├── Exercises/    # [NEW] Exercise library management
│   │   │   │   │   ├── Endpoints/
│   │   │   │   │   │   ├── CreateExercise.cs
│   │   │   │   │   │   ├── ListExercises.cs
│   │   │   │   │   │   ├── GetExercise.cs
│   │   │   │   │   │   └── UpdateExercise.cs
│   │   │   │   │   ├── Validators/
│   │   │   │   │   │   ├── CreateExerciseValidator.cs
│   │   │   │   │   │   └── UpdateExerciseValidator.cs
│   │   │   │   │   └── ExercisesFeature.cs
│   │   │   │   └── WorkoutPlans/ # [NEW] Workout plan management
│   │   │   │       ├── Endpoints/
│   │   │   │       │   ├── CreateWorkoutPlan.cs
│   │   │   │       │   ├── ListWorkoutPlans.cs
│   │   │   │       │   ├── GetWorkoutPlan.cs
│   │   │   │       │   ├── UpdateWorkoutPlan.cs
│   │   │   │       │   ├── AddExerciseToPlan.cs
│   │   │   │       │   ├── RemoveExerciseFromPlan.cs
│   │   │   │       │   ├── ReorderExercises.cs
│   │   │   │       │   ├── AssignPlanToClient.cs
│   │   │   │       │   └── UnassignPlanFromClient.cs
│   │   │   │       ├── Validators/
│   │   │   │       │   ├── CreateWorkoutPlanValidator.cs
│   │   │   │       │   ├── AddExerciseToPlanValidator.cs
│   │   │   │       │   └── ReorderExercisesValidator.cs
│   │   │   │       └── WorkoutPlansFeature.cs
│   │   │   └── Common/           # Shared infrastructure
│   │   │       ├── Database/
│   │   │       │   ├── ApplicationDbContext.cs
│   │   │       │   ├── Migrations/
│   │   │       │   └── Seeding/
│   │   │       │       └── AdminUserSeeder.cs  # [NEW] Pre-configured admin
│   │   │       └── Persistence/
│   │   │           └── VogenEfCoreConverters.cs
│   │   ├── Domain/               # Domain entities and logic [EXISTING - using as-is]
│   │   │   ├── Users/
│   │   │   │   ├── User.cs       # [EXISTING] User aggregate
│   │   │   │   ├── UserRole.cs   # [EXISTING] Admin/Trainer/Client enum
│   │   │   │   └── UserErrors.cs # [EXISTING] Domain errors
│   │   │   ├── Exercises/
│   │   │   │   ├── Exercise.cs   # [EXISTING] Exercise aggregate
│   │   │   │   ├── ExerciseType.cs  # [EXISTING] RepsAndWeight/TimeBased
│   │   │   │   ├── MuscleGroup.cs   # [EXISTING] Muscle group enum
│   │   │   │   └── ExerciseErrors.cs
│   │   │   ├── WorkoutPlans/
│   │   │   │   ├── WorkoutPlan.cs      # [EXISTING] WorkoutPlan aggregate
│   │   │   │   ├── PlannedExercise.cs  # [EXISTING] Exercise in plan
│   │   │   │   └── WorkoutPlanErrors.cs
│   │   │   └── Common/
│   │   │       ├── Duration.cs   # [EXISTING] Value object
│   │   │       └── Weight.cs     # [EXISTING] Value object
│   │   └── ServiceDefaults/      # Aspire configuration [EXISTING]
│   ├── tests/                    # All test projects (when tests are needed)
│   │   ├── Domain.UnitTests/     # [EXISTING]
│   │   ├── WebApi.UnitTests/     # [EXISTING]
│   │   ├── WebApi.IntegrationTests/  # [EXISTING]
│   │   └── WebApi.ArchitectureTests/ # [EXISTING]
│   └── tools/                    # Aspire AppHost and migrations [EXISTING]
│       ├── AppHost/
│       └── MigrationService/
│
└── frontend/                     # Frontend Next.js PWA [NEW - entire directory]
    ├── src/
    │   ├── app/                  # Next.js App Router
    │   │   ├── layout.tsx        # Root layout with PWA manifest
    │   │   ├── page.tsx          # User selection screen (no auth)
    │   │   ├── admin/            # Admin routes
    │   │   │   ├── layout.tsx
    │   │   │   ├── page.tsx      # Admin dashboard
    │   │   │   └── trainers/     # Trainer management
    │   │   │       ├── page.tsx
    │   │   │       ├── new/
    │   │   │       └── [id]/
    │   │   └── trainer/          # Trainer routes
    │   │       ├── layout.tsx
    │   │       ├── page.tsx      # Trainer dashboard
    │   │       ├── clients/      # Client management
    │   │       │   ├── page.tsx
    │   │       │   ├── new/
    │   │       │   └── [id]/
    │   │       ├── exercises/    # Exercise library
    │   │       │   ├── page.tsx
    │   │       │   ├── new/
    │   │       │   └── [id]/
    │   │       └── workout-plans/  # Workout plans
    │   │           ├── page.tsx
    │   │           ├── new/
    │   │           └── [id]/
    │   ├── features/             # Feature slices (VSA)
    │   │   ├── admin/
    │   │   │   ├── components/
    │   │   │   │   ├── TrainerList.tsx
    │   │   │   │   ├── TrainerForm.tsx
    │   │   │   │   └── AdminDashboard.tsx
    │   │   │   ├── hooks/
    │   │   │   │   ├── useTrainers.ts
    │   │   │   │   └── useCreateTrainer.ts
    │   │   │   └── types.ts
    │   │   ├── trainer/
    │   │   │   ├── components/
    │   │   │   │   ├── ClientList.tsx
    │   │   │   │   ├── ClientForm.tsx
    │   │   │   │   └── TrainerDashboard.tsx
    │   │   │   ├── hooks/
    │   │   │   │   ├── useClients.ts
    │   │   │   │   └── useCreateClient.ts
    │   │   │   └── types.ts
    │   │   ├── exercise-library/
    │   │   │   ├── components/
    │   │   │   │   ├── ExerciseList.tsx
    │   │   │   │   ├── ExerciseForm.tsx
    │   │   │   │   ├── ExerciseFilter.tsx
    │   │   │   │   └── MuscleGroupBadge.tsx
    │   │   │   ├── hooks/
    │   │   │   │   ├── useExercises.ts
    │   │   │   │   └── useCreateExercise.ts
    │   │   │   └── types.ts
    │   │   └── workout-plans/
    │   │       ├── components/
    │   │       │   ├── WorkoutPlanList.tsx
    │   │       │   ├── WorkoutPlanForm.tsx
    │   │       │   ├── PlanExerciseList.tsx
    │   │       │   ├── AddExerciseModal.tsx
    │   │       │   └── ClientAssignmentList.tsx
    │   │       ├── hooks/
    │   │       │   ├── useWorkoutPlans.ts
    │   │       │   ├── useCreateWorkoutPlan.ts
    │   │       │   └── useAssignPlan.ts
    │   │       └── types.ts
    │   ├── components/           # Shared UI components
    │   │   ├── ui/               # Shadcn/ui components
    │   │   │   ├── button.tsx
    │   │   │   ├── input.tsx
    │   │   │   ├── form.tsx
    │   │   │   ├── card.tsx
    │   │   │   ├── select.tsx
    │   │   │   ├── badge.tsx
    │   │   │   ├── toast.tsx
    │   │   │   └── ...
    │   │   ├── ErrorBoundary.tsx
    │   │   ├── LoadingSpinner.tsx
    │   │   ├── UserSelector.tsx   # No-auth user selection
    │   │   └── Navigation.tsx     # Role-based navigation
    │   └── lib/                  # Utilities and helpers
    │       ├── api/              # API client
    │       │   ├── client.ts     # Base API client with TanStack Query
    │       │   ├── users.ts      # User endpoints
    │       │   ├── exercises.ts  # Exercise endpoints
    │       │   └── workout-plans.ts
    │       ├── stores/           # Zustand stores
    │       │   └── user-store.ts # Current user state
    │       ├── validation/       # Zod schemas
    │       │   ├── user.ts
    │       │   ├── exercise.ts
    │       │   └── workout-plan.ts
    │       └── utils/
    │           ├── format-duration.ts
    │           ├── format-weight.ts
    │           └── cn.ts         # Tailwind class merging
    ├── public/                   # Static assets and PWA manifest
    │   ├── manifest.json         # PWA manifest
    │   ├── icons/                # App icons for PWA
    │   └── sw.js                 # Service worker (generated by next-pwa)
    ├── next.config.js            # Next.js config with PWA plugin
    ├── tailwind.config.js        # Tailwind configuration
    ├── tsconfig.json             # TypeScript configuration
    └── package.json              # Dependencies
```

**Structure Decision**: Web application (Option 2) - Frontend PWA + Backend API architecture. The backend follows existing VSA pattern with new feature slices for Users, Exercises, and WorkoutPlans. The frontend is a new Next.js 16 PWA organized by feature using VSA principles, with shared UI components from Shadcn/ui and mobile-first responsive design.

## Complexity Tracking

> No constitution violations - this section is intentionally empty per template guidance.
