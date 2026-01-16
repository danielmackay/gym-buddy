# Implementation Tasks: Gym Workout Tracking PWA

**Feature Branch**: `001-gym-workout-pwa`  
**Date**: Sat Jan 17 2026  
**Status**: Ready for Implementation

This document provides a comprehensive, ordered list of implementation tasks broken down by phase and user story. Tasks are designed to be independently completable where possible, with clear dependencies marked.

---

## Task Format

Each task follows the format: `- [ ] T### [Flags] Description with file path`

**Flags**:
- `[P]` = Parallelizable (can be done in parallel with adjacent P tasks)
- `[US#]` = User Story number (1-5 from spec.md)

**Dependencies**: Tasks should be completed in order within each phase unless marked `[P]` for parallelization.

---

## Phase 1: Project Setup & Infrastructure

**Goal**: Initialize frontend project, configure PWA, setup core infrastructure

### Frontend Project Initialization

- [ ] T001 [P] Initialize Next.js 16 project at `src/frontend/` with TypeScript, App Router, and Tailwind CSS
- [ ] T002 [P] Install PWA dependencies: `next-pwa@latest` in `src/frontend/package.json`
- [ ] T003 [P] Install UI dependencies: `@radix-ui/react-*`, `class-variance-authority`, `clsx`, `tailwind-merge`, `lucide-react` in `src/frontend/package.json`
- [ ] T004 [P] Install state management: `zustand`, `@tanstack/react-query` in `src/frontend/package.json`
- [ ] T005 [P] Install form handling: `react-hook-form`, `@hookform/resolvers`, `zod` in `src/frontend/package.json`
- [ ] T006 [P] Install utilities: `date-fns` in `src/frontend/package.json`
- [ ] T007 [P] Install drag-and-drop: `@dnd-kit/core`, `@dnd-kit/sortable`, `@dnd-kit/utilities` in `src/frontend/package.json`
- [ ] T008 Configure `next-pwa` in `src/frontend/next.config.js` with service worker, cache strategies, and offline support
- [ ] T009 Create PWA manifest in `src/frontend/public/manifest.json` with app metadata, icons, and theme colors
- [ ] T010 Create app icons (192x192, 512x512) in `src/frontend/public/icons/` for PWA installation
- [ ] T011 Initialize Shadcn/ui in `src/frontend/` with `npx shadcn-ui@latest init`
- [ ] T012 Configure Tailwind CSS in `src/frontend/tailwind.config.js` with mobile-first breakpoints and custom theme

### Frontend Directory Structure

- [ ] T013 Create feature slice directories: `src/frontend/src/features/{admin,trainer,exercise-library,workout-plans}/`
- [ ] T014 Create shared component directory: `src/frontend/src/components/`
- [ ] T015 Create lib directories: `src/frontend/src/lib/{api,stores,validation,utils}/`
- [ ] T016 Create TypeScript types directory: `src/frontend/src/lib/types/`

### Backend Infrastructure

- [ ] T017 Create AdminUserSeeder in `src/api/tools/MigrationService/Seeders/AdminUserSeeder.cs`
- [ ] T018 Update MigrationService Program.cs to call AdminUserSeeder after migrations in `src/api/tools/MigrationService/Program.cs`
- [ ] T020 Run migration service locally to seed admin user (verify with Aspire dashboard)

---

## Phase 2: Shared Frontend Foundation

**Goal**: Build reusable infrastructure used by all features

### Core API Client

- [ ] T020 Create base API client with error handling in `src/frontend/src/lib/api/client.ts`
- [ ] T021 Create API error classes for FluentValidation and Domain errors in `src/frontend/src/lib/types/errors.ts`
- [ ] T022 Setup TanStack Query provider in `src/frontend/src/app/providers.tsx`
- [ ] T023 Create environment configuration for API URL in `src/frontend/.env.local` and `src/frontend/.env.production`

### TypeScript Type Definitions

- [ ] T024 [P] Create User types (User, UserRole, CreateTrainerRequest, CreateClientRequest, UpdateUserRequest) in `src/frontend/src/lib/types/user.ts`
- [ ] T025 [P] Create Exercise types (Exercise, ExerciseType, MuscleGroup, CreateExerciseRequest, UpdateExerciseRequest) in `src/frontend/src/lib/types/exercise.ts`
- [ ] T026 [P] Create WorkoutPlan types (WorkoutPlan, PlannedExercise, Weight, Duration, CreateWorkoutPlanRequest, AddExerciseToPlanRequest) in `src/frontend/src/lib/types/workout-plan.ts`

### Validation Schemas

- [ ] T027 [P] Create user validation schemas (createTrainerSchema, createClientSchema, updateUserSchema) in `src/frontend/src/lib/validation/user.ts`
- [ ] T028 [P] Create exercise validation schemas (createExerciseSchema, updateExerciseSchema) in `src/frontend/src/lib/validation/exercise.ts`
- [ ] T029 [P] Create workout plan validation schemas (createWorkoutPlanSchema, addExerciseToPlanSchema, reorderExercisesSchema) in `src/frontend/src/lib/validation/workout-plan.ts`

### Zustand Stores

- [ ] T030 Create user selection store in `src/frontend/src/lib/stores/user-store.ts` with currentUser state and localStorage persistence

### Shared UI Components (Shadcn/ui)

- [ ] T031 [P] Add Shadcn/ui Button component to `src/frontend/src/components/ui/button.tsx`
- [ ] T032 [P] Add Shadcn/ui Input component to `src/frontend/src/components/ui/input.tsx`
- [ ] T033 [P] Add Shadcn/ui Form components to `src/frontend/src/components/ui/form.tsx`
- [ ] T034 [P] Add Shadcn/ui Card component to `src/frontend/src/components/ui/card.tsx`
- [ ] T035 [P] Add Shadcn/ui Select component to `src/frontend/src/components/ui/select.tsx`
- [ ] T036 [P] Add Shadcn/ui Badge component to `src/frontend/src/components/ui/badge.tsx`
- [ ] T037 [P] Add Shadcn/ui Toast/Sonner component to `src/frontend/src/components/ui/sonner.tsx`
- [ ] T038 [P] Add Shadcn/ui Dialog component to `src/frontend/src/components/ui/dialog.tsx`
- [ ] T039 [P] Add Shadcn/ui Sheet component (mobile drawer) to `src/frontend/src/components/ui/sheet.tsx`
- [ ] T040 [P] Add Shadcn/ui Label component to `src/frontend/src/components/ui/label.tsx`

### Custom Shared Components

- [ ] T041 Create ErrorBoundary component in `src/frontend/src/components/ErrorBoundary.tsx`
- [ ] T042 Create LoadingSpinner component in `src/frontend/src/components/LoadingSpinner.tsx`
- [ ] T043 Create UserSelector component (no-auth user selection) in `src/frontend/src/components/UserSelector.tsx`
- [ ] T044 Create Navigation component with role-based menu in `src/frontend/src/components/Navigation.tsx`

### Utility Functions

- [ ] T045 [P] Create format-duration utility in `src/frontend/src/lib/utils/format-duration.ts`
- [ ] T046 [P] Create format-weight utility (kg/lbs conversion) in `src/frontend/src/lib/utils/format-weight.ts`
- [ ] T047 [P] Create cn utility for Tailwind class merging in `src/frontend/src/lib/utils/cn.ts`

### Root App Layout

- [ ] T048 Create root layout with PWA metadata in `src/frontend/src/app/layout.tsx`
- [ ] T049 Create root page (user selection screen) in `src/frontend/src/app/page.tsx`
- [ ] T050 Add global styles with mobile-first CSS in `src/frontend/src/app/globals.css`

---

## Phase 3: User Story 1 - Admin Creates Trainers (P1)

**Goal**: Enable admin user to create and manage trainers

### Backend - Users Feature Slice [US1]

- [ ] T051 [US1] Create UsersFeature.cs group registration in `src/api/src/WebApi/Features/Users/UsersFeature.cs`
- [ ] T052 [US1] Create CreateTrainer endpoint in `src/api/src/WebApi/Features/Users/Endpoints/CreateTrainer.cs`
- [ ] T053 [US1] Create CreateTrainerValidator in `src/api/src/WebApi/Features/Users/Validators/CreateTrainerValidator.cs`
- [ ] T054 [US1] Create ListTrainers endpoint in `src/api/src/WebApi/Features/Users/Endpoints/ListTrainers.cs`
- [ ] T055 [US1] Create GetUser endpoint in `src/api/src/WebApi/Features/Users/Endpoints/GetUser.cs`
- [ ] T056 [US1] Create UpdateUser endpoint in `src/api/src/WebApi/Features/Users/Endpoints/UpdateUser.cs`
- [ ] T057 [US1] Create UpdateUserValidator in `src/api/src/WebApi/Features/Users/Validators/UpdateUserValidator.cs`
- [ ] T058 [US1] Run EF Core migration to ensure User tables exist: `dotnet ef migrations add AddUsersFeature`
- [ ] T059 [US1] Test all Users endpoints with Aspire dashboard/Swagger (create trainer, list, get, update)

### Frontend - Users API Service [US1]

- [ ] T060 [US1] Create users API service with all endpoint methods in `src/frontend/src/lib/api/users.ts`

### Frontend - Admin Feature Slice [US1]

- [ ] T061 [US1] Create admin layout with navigation in `src/frontend/src/app/admin/layout.tsx`
- [ ] T062 [US1] Create admin dashboard page in `src/frontend/src/app/admin/page.tsx`
- [ ] T063 [US1] Create trainer list page in `src/frontend/src/app/admin/trainers/page.tsx`
- [ ] T064 [US1] Create new trainer page in `src/frontend/src/app/admin/trainers/new/page.tsx`
- [ ] T065 [US1] Create trainer detail page in `src/frontend/src/app/admin/trainers/[id]/page.tsx`

### Frontend - Admin Components [US1]

- [ ] T066 [US1] Create AdminDashboard component in `src/frontend/src/features/admin/components/AdminDashboard.tsx`
- [ ] T067 [US1] Create TrainerList component in `src/frontend/src/features/admin/components/TrainerList.tsx`
- [ ] T068 [US1] Create TrainerForm component with validation in `src/frontend/src/features/admin/components/TrainerForm.tsx`

### Frontend - Admin Hooks [US1]

- [ ] T069 [US1] [P] Create useTrainers hook with TanStack Query in `src/frontend/src/features/admin/hooks/useTrainers.ts`
- [ ] T070 [US1] [P] Create useCreateTrainer mutation hook in `src/frontend/src/features/admin/hooks/useCreateTrainer.ts`
- [ ] T071 [US1] [P] Create useUpdateTrainer mutation hook in `src/frontend/src/features/admin/hooks/useUpdateTrainer.ts`
- [ ] T072 [US1] [P] Create useTrainer hook (get single) in `src/frontend/src/features/admin/hooks/useTrainer.ts`

### Testing [US1]

- [ ] T073 [US1] Manually test User Story 1 acceptance scenarios (create trainer, invalid email, list trainers, update trainer)

---

## Phase 4: User Story 2 - Trainer Creates Clients (P1)

**Goal**: Enable trainers to create and manage their clients

### Backend - Users Feature Slice (Clients) [US2]

- [ ] T074 [US2] Create CreateClient endpoint in `src/api/src/WebApi/Features/Users/Endpoints/CreateClient.cs`
- [ ] T075 [US2] Create CreateClientValidator in `src/api/src/WebApi/Features/Users/Validators/CreateClientValidator.cs`
- [ ] T076 [US2] Create ListClients endpoint (filtered by trainerId) in `src/api/src/WebApi/Features/Users/Endpoints/ListClients.cs`
- [ ] T077 [US2] Test client endpoints with Aspire dashboard/Swagger (create client, list for trainer)

### Frontend - Trainer Feature Slice [US2]

- [ ] T078 [US2] Create trainer layout with navigation in `src/frontend/src/app/trainer/layout.tsx`
- [ ] T079 [US2] Create trainer dashboard page in `src/frontend/src/app/trainer/page.tsx`
- [ ] T080 [US2] Create client list page in `src/frontend/src/app/trainer/clients/page.tsx`
- [ ] T081 [US2] Create new client page in `src/frontend/src/app/trainer/clients/new/page.tsx`
- [ ] T082 [US2] Create client detail page in `src/frontend/src/app/trainer/clients/[id]/page.tsx`

### Frontend - Trainer Components [US2]

- [ ] T083 [US2] Create TrainerDashboard component in `src/frontend/src/features/trainer/components/TrainerDashboard.tsx`
- [ ] T084 [US2] Create ClientList component in `src/frontend/src/features/trainer/components/ClientList.tsx`
- [ ] T085 [US2] Create ClientForm component with validation in `src/frontend/src/features/trainer/components/ClientForm.tsx`

### Frontend - Trainer Hooks [US2]

- [ ] T086 [US2] [P] Create useClients hook with TanStack Query in `src/frontend/src/features/trainer/hooks/useClients.ts`
- [ ] T087 [US2] [P] Create useCreateClient mutation hook in `src/frontend/src/features/trainer/hooks/useCreateClient.ts`
- [ ] T088 [US2] [P] Create useUpdateClient mutation hook in `src/frontend/src/features/trainer/hooks/useUpdateClient.ts`
- [ ] T089 [US2] [P] Create useClient hook (get single) in `src/frontend/src/features/trainer/hooks/useClient.ts`

### Testing [US2]

- [ ] T090 [US2] Manually test User Story 2 acceptance scenarios (create client, invalid email, list clients, update client, trainer assignment)

---

## Phase 5: User Story 3 - Trainer Manages Exercise Library (P2)

**Goal**: Enable trainers to create and manage exercises

### Backend - Exercises Feature Slice [US3]

- [ ] T091 [US3] Create ExercisesFeature.cs group registration in `src/api/src/WebApi/Features/Exercises/ExercisesFeature.cs`
- [ ] T092 [US3] Create CreateExercise endpoint in `src/api/src/WebApi/Features/Exercises/Endpoints/CreateExercise.cs`
- [ ] T093 [US3] Create CreateExerciseValidator in `src/api/src/WebApi/Features/Exercises/Validators/CreateExerciseValidator.cs`
- [ ] T094 [US3] Create ListExercises endpoint with muscle group/type filtering in `src/api/src/WebApi/Features/Exercises/Endpoints/ListExercises.cs`
- [ ] T095 [US3] Create GetExercise endpoint in `src/api/src/WebApi/Features/Exercises/Endpoints/GetExercise.cs`
- [ ] T096 [US3] Create UpdateExercise endpoint in `src/api/src/WebApi/Features/Exercises/Endpoints/UpdateExercise.cs`
- [ ] T097 [US3] Create UpdateExerciseValidator in `src/api/src/WebApi/Features/Exercises/Validators/UpdateExerciseValidator.cs`
- [ ] T098 [US3] Run EF Core migration to ensure Exercise tables exist: `dotnet ef migrations add AddExercisesFeature`
- [ ] T099 [US3] Test all Exercises endpoints with Aspire dashboard/Swagger (create, list, filter, get, update)

### Frontend - Exercises API Service [US3]

- [ ] T100 [US3] Create exercises API service with all endpoint methods in `src/frontend/src/lib/api/exercises.ts`

### Frontend - Exercise Library Feature Slice [US3]

- [ ] T101 [US3] Create exercise list page in `src/frontend/src/app/trainer/exercises/page.tsx`
- [ ] T102 [US3] Create new exercise page in `src/frontend/src/app/trainer/exercises/new/page.tsx`
- [ ] T103 [US3] Create exercise detail page in `src/frontend/src/app/trainer/exercises/[id]/page.tsx`

### Frontend - Exercise Library Components [US3]

- [ ] T104 [US3] Create ExerciseList component with filtering in `src/frontend/src/features/exercise-library/components/ExerciseList.tsx`
- [ ] T105 [US3] Create ExerciseForm component with type/muscle group selection in `src/frontend/src/features/exercise-library/components/ExerciseForm.tsx`
- [ ] T106 [US3] Create ExerciseFilter component (muscle group, type) in `src/frontend/src/features/exercise-library/components/ExerciseFilter.tsx`
- [ ] T107 [US3] Create MuscleGroupBadge component in `src/frontend/src/features/exercise-library/components/MuscleGroupBadge.tsx`

### Frontend - Exercise Library Hooks [US3]

- [ ] T108 [US3] [P] Create useExercises hook with filtering support in `src/frontend/src/features/exercise-library/hooks/useExercises.ts`
- [ ] T109 [US3] [P] Create useCreateExercise mutation hook in `src/frontend/src/features/exercise-library/hooks/useCreateExercise.ts`
- [ ] T110 [US3] [P] Create useUpdateExercise mutation hook in `src/frontend/src/features/exercise-library/hooks/useUpdateExercise.ts`
- [ ] T111 [US3] [P] Create useExercise hook (get single) in `src/frontend/src/features/exercise-library/hooks/useExercise.ts`

### Testing [US3]

- [ ] T112 [US3] Manually test User Story 3 acceptance scenarios (create reps/weight exercise, create time-based exercise, list, filter, update)

---

## Phase 6: User Story 4 - Trainer Creates Workout Plans (P2)

**Goal**: Enable trainers to create and manage workout plans with exercises

### Backend - WorkoutPlans Feature Slice [US4]

- [ ] T113 [US4] Create WorkoutPlansFeature.cs group registration in `src/api/src/WebApi/Features/WorkoutPlans/WorkoutPlansFeature.cs`
- [ ] T114 [US4] Create CreateWorkoutPlan endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/CreateWorkoutPlan.cs`
- [ ] T115 [US4] Create CreateWorkoutPlanValidator in `src/api/src/WebApi/Features/WorkoutPlans/Validators/CreateWorkoutPlanValidator.cs`
- [ ] T116 [US4] Create ListWorkoutPlans endpoint (filtered by trainerId) in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/ListWorkoutPlans.cs`
- [ ] T117 [US4] Create GetWorkoutPlan endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/GetWorkoutPlan.cs`
- [ ] T118 [US4] Create UpdateWorkoutPlan endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/UpdateWorkoutPlan.cs`
- [ ] T119 [US4] Create AddExerciseToPlan endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/AddExerciseToPlan.cs`
- [ ] T120 [US4] Create AddExerciseToPlanValidator in `src/api/src/WebApi/Features/WorkoutPlans/Validators/AddExerciseToPlanValidator.cs`
- [ ] T121 [US4] Create RemoveExerciseFromPlan endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/RemoveExerciseFromPlan.cs`
- [ ] T122 [US4] Create ReorderExercises endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/ReorderExercises.cs`
- [ ] T123 [US4] Create ReorderExercisesValidator in `src/api/src/WebApi/Features/WorkoutPlans/Validators/ReorderExercisesValidator.cs`
- [ ] T124 [US4] Run EF Core migration to ensure WorkoutPlan/PlannedExercise tables exist: `dotnet ef migrations add AddWorkoutPlansFeature`
- [ ] T125 [US4] Test all WorkoutPlans endpoints with Aspire dashboard/Swagger (create plan, add exercises, reorder, remove)

### Frontend - Workout Plans API Service [US4]

- [ ] T126 [US4] Create workout-plans API service with all endpoint methods in `src/frontend/src/lib/api/workout-plans.ts`

### Frontend - Workout Plans Feature Slice [US4]

- [ ] T127 [US4] Create workout plan list page in `src/frontend/src/app/trainer/workout-plans/page.tsx`
- [ ] T128 [US4] Create new workout plan page in `src/frontend/src/app/trainer/workout-plans/new/page.tsx`
- [ ] T129 [US4] Create workout plan detail/edit page in `src/frontend/src/app/trainer/workout-plans/[id]/page.tsx`

### Frontend - Workout Plans Components [US4]

- [ ] T130 [US4] Create WorkoutPlanList component in `src/frontend/src/features/workout-plans/components/WorkoutPlanList.tsx`
- [ ] T131 [US4] Create WorkoutPlanForm component in `src/frontend/src/features/workout-plans/components/WorkoutPlanForm.tsx`
- [ ] T132 [US4] Create PlanExerciseList component with drag-and-drop reordering using @dnd-kit/sortable in `src/frontend/src/features/workout-plans/components/PlanExerciseList.tsx`
- [ ] T133 [US4] Create AddExerciseModal component with exercise selection in `src/frontend/src/features/workout-plans/components/AddExerciseModal.tsx`

### Frontend - Workout Plans Hooks [US4]

- [ ] T134 [US4] [P] Create useWorkoutPlans hook in `src/frontend/src/features/workout-plans/hooks/useWorkoutPlans.ts`
- [ ] T135 [US4] [P] Create useWorkoutPlan hook (get single) in `src/frontend/src/features/workout-plans/hooks/useWorkoutPlan.ts`
- [ ] T136 [US4] [P] Create useCreateWorkoutPlan mutation hook in `src/frontend/src/features/workout-plans/hooks/useCreateWorkoutPlan.ts`
- [ ] T137 [US4] [P] Create useUpdateWorkoutPlan mutation hook in `src/frontend/src/features/workout-plans/hooks/useUpdateWorkoutPlan.ts`
- [ ] T138 [US4] [P] Create useAddExercise mutation hook in `src/frontend/src/features/workout-plans/hooks/useAddExercise.ts`
- [ ] T139 [US4] [P] Create useRemoveExercise mutation hook in `src/frontend/src/features/workout-plans/hooks/useRemoveExercise.ts`
- [ ] T140 [US4] [P] Create useReorderExercises mutation hook in `src/frontend/src/features/workout-plans/hooks/useReorderExercises.ts`

### Testing [US4]

- [ ] T141 [US4] Manually test User Story 4 acceptance scenarios (create plan, add reps/weight exercise, add time-based exercise, reorder, remove, view)

---

## Phase 7: User Story 5 - Trainer Assigns Plans to Clients (P2)

**Goal**: Enable trainers to assign/unassign workout plans to clients

### Backend - WorkoutPlans Feature Slice (Assignment) [US5]

- [ ] T142 [US5] Create AssignPlanToClient endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/AssignPlanToClient.cs`
- [ ] T143 [US5] Create UnassignPlanFromClient endpoint in `src/api/src/WebApi/Features/WorkoutPlans/Endpoints/UnassignPlanFromClient.cs`
- [ ] T144 [US5] Test assignment endpoints with Aspire dashboard/Swagger (assign, unassign, verify client workout plans)

### Frontend - Workout Plans Components (Assignment) [US5]

- [ ] T145 [US5] Create ClientAssignmentList component in `src/frontend/src/features/workout-plans/components/ClientAssignmentList.tsx`
- [ ] T146 [US5] Update workout plan detail page to show assigned clients in `src/frontend/src/app/trainer/workout-plans/[id]/page.tsx`
- [ ] T147 [US5] Update client detail page to show assigned plans in `src/frontend/src/app/trainer/clients/[id]/page.tsx`

### Frontend - Workout Plans Hooks (Assignment) [US5]

- [ ] T148 [US5] [P] Create useAssignPlan mutation hook in `src/frontend/src/features/workout-plans/hooks/useAssignPlan.ts`
- [ ] T149 [US5] [P] Create useUnassignPlan mutation hook in `src/frontend/src/features/workout-plans/hooks/useUnassignPlan.ts`

### Testing [US5]

- [ ] T150 [US5] Manually test User Story 5 acceptance scenarios (assign plan to client, view assigned plans, unassign plan)

---

## Phase 8: Polish & Cross-Cutting Concerns

**Goal**: Add final touches, error handling, loading states, and mobile optimization

### Error Handling & User Feedback

- [ ] T151 Add toast notifications for all mutation success/error states across all features
- [ ] T152 Add error boundaries to all main pages (admin, trainer, client pages)
- [ ] T153 Add loading spinners to all data-fetching pages
- [ ] T154 Implement optimistic updates for mutations where appropriate (e.g., adding exercises to plan)
- [ ] T155 Test FluentValidation error display in all forms
- [ ] T156 Test Domain error display for all error scenarios
- [ ] T157 Test generic error fallback for unexpected errors

### Mobile Responsiveness

- [ ] T158 Test all pages on 320px width (minimum requirement)
- [ ] T159 Verify all touch targets are minimum 44x44px
- [ ] T160 Test navigation on mobile devices (bottom nav vs sidebar)
- [ ] T161 Test form inputs on mobile devices (appropriate keyboards, validation)
- [ ] T162 Test drag-and-drop exercise reordering on touch devices

### PWA Functionality

- [ ] T163 Test service worker registration and caching
- [ ] T164 Test offline mode (view cached data)
- [ ] T165 Test "Add to Home Screen" functionality on iOS and Android
- [ ] T166 Run Lighthouse PWA audit (target 90+ score)
- [ ] T167 Verify PWA manifest icons display correctly
- [ ] T168 Test performance on 3G network (target <3s initial load)

### Performance Optimization

- [ ] T169 Audit bundle size (target <200KB gzipped initial bundle)
- [ ] T170 Implement code splitting for feature pages
- [ ] T171 Optimize images and icons
- [ ] T172 Test Time to Interactive on mobile (target <3s on 3G)
- [ ] T173 Run Lighthouse Performance audit (target 90+ score)

### Validation & Integration Testing

- [ ] T174 Test full user journey: Admin creates trainer → Trainer creates client → Trainer creates exercises → Trainer creates workout plan → Trainer assigns plan to client
- [ ] T175 Verify all 22 functional requirements from spec.md are implemented
- [ ] T176 Verify all 10 success criteria from spec.md are met
- [ ] T177 Follow quickstart.md validation steps
- [ ] T178 Test edge cases from spec.md (duplicate email, concurrent edits, etc.)

### Documentation

- [ ] T179 Add inline code comments for complex logic
- [ ] T180 Update README.md with setup instructions (if needed)
- [ ] T181 Document known limitations (offline editing, no auth, etc.)

---

## Task Summary

**Total Tasks**: 181

**By Phase**:
- Phase 1 (Setup): 19 tasks
- Phase 2 (Foundation): 31 tasks
- Phase 3 (US1 - Admin/Trainers): 23 tasks
- Phase 4 (US2 - Clients): 17 tasks
- Phase 5 (US3 - Exercises): 22 tasks
- Phase 6 (US4 - Workout Plans): 29 tasks
- Phase 7 (US5 - Assignments): 9 tasks
- Phase 8 (Polish): 31 tasks

**By User Story**:
- US1 (Admin Creates Trainers): 23 tasks
- US2 (Trainer Creates Clients): 17 tasks
- US3 (Trainer Manages Exercise Library): 22 tasks
- US4 (Trainer Creates Workout Plans): 29 tasks
- US5 (Trainer Assigns Plans to Clients): 9 tasks
- Infrastructure/Shared: 81 tasks

**Priority Distribution**:
- P1 (US1 + US2): 40 tasks ⭐ **MVP Critical**
- P2 (US3 + US4 + US5): 60 tasks
- Infrastructure/Polish: 81 tasks

---

## Implementation Notes

### Parallelization Strategy

Tasks marked `[P]` can be executed in parallel with adjacent `[P]` tasks within the same section. For example, T001-T007 can all run concurrently during dependency installation.

### Testing Strategy

Per Constitution Principle III, tests are created **on demand**. Manual testing is included after each user story implementation. If automated tests are requested, they should be added as new tasks.

### Dependencies

- **Backend tasks** must complete before frontend pages that consume those APIs
- **Shared foundation tasks** (Phase 2) must complete before feature-specific tasks (Phases 3-7)
- **Infrastructure tasks** (Phase 1) must complete before all other phases

### Estimated Effort

- **Phase 1-2 (Setup + Foundation)**: 2-3 days
- **Phase 3-4 (P1 User Stories)**: 3-4 days ⭐ **MVP Delivery**
- **Phase 5-7 (P2 User Stories)**: 4-5 days
- **Phase 8 (Polish)**: 2-3 days
- **Total**: ~12-15 days for one developer

### Success Milestones

1. ✅ **Milestone 1**: Admin can create trainers (End of Phase 3)
2. ✅ **Milestone 2**: Trainer can create clients (End of Phase 4) - **MVP Complete**
3. ✅ **Milestone 3**: Trainer can manage exercise library (End of Phase 5)
4. ✅ **Milestone 4**: Trainer can create workout plans (End of Phase 6)
5. ✅ **Milestone 5**: Trainer can assign plans to clients (End of Phase 7)
6. ✅ **Milestone 6**: Production-ready PWA (End of Phase 8)

---

**Status**: ✅ Ready for implementation

**Next Steps**: Begin Phase 1 tasks starting with T001
