# Phase 1: Data Model

**Feature**: Gym Workout Tracking PWA  
**Date**: Sat Jan 17 2026  
**Status**: Complete

## Overview

This document describes the data model for the gym workout tracking application. The backend uses the existing Domain entities from `src/api/src/Domain/`, and the frontend uses TypeScript interfaces that mirror these entities for type safety.

---

## Backend Domain Model (Existing)

The following entities already exist in the Domain layer and will be used as-is. No new domain entities need to be created.

### 1. User Aggregate

**Location**: `src/api/src/Domain/Users/User.cs`

**Aggregate Root**: Yes  
**Strongly Typed ID**: `UserId` (Guid-based, using Vogen)

**Properties**:
- `Id: UserId` - Unique identifier
- `Name: string` - User's name (max 100 characters)
- `Email: string` - User's email address (max 256 characters)
- `Roles: IReadOnlyList<UserRole>` - Collection of roles (Admin, Trainer, Client)
- `TrainerId: UserId?` - Reference to trainer (nullable, only for clients)
- `AssignedWorkoutPlanIds: IReadOnlyList<WorkoutPlanId>` - Workout plans assigned to this client

**Business Rules** (enforced by domain):
- Name is required and must not exceed 100 characters
- Email is required, must be valid format, and must not exceed 256 characters
- Users can have multiple roles
- Clients must have a trainer assigned
- Clients cannot assign themselves as their own trainer
- Only clients can have workout plans assigned

**Domain Methods**:
- `User.Create(name, email)` - Factory method
- `AddRole(role)` - Add a role to user
- `RemoveRole(role)` - Remove a role from user
- `AssignTrainer(trainerId)` - Assign trainer to client
- `UnassignTrainer()` - Remove trainer assignment
- `AssignWorkoutPlan(workoutPlanId)` - Assign plan to client
- `UnassignWorkoutPlan(workoutPlanId)` - Unassign plan from client
- `HasRole(role)` - Check if user has specific role

**Domain Errors** (`UserErrors.cs`):
- `AlreadyHasRole` - User already has the specified role
- `DoesNotHaveRole` - User does not have the specified role
- `NotAClient` - Operation requires client role
- `CannotAssignSelfAsTrainer` - Cannot assign user as their own trainer
- `WorkoutPlanAlreadyAssigned` - Workout plan already assigned
- `WorkoutPlanNotAssigned` - Workout plan not assigned

### 2. UserRole Enum

**Location**: `src/api/src/Domain/Users/UserRole.cs`

**Values**:
- `Admin = 1` - Administrator with full system access
- `Trainer = 2` - Trainer who manages clients and creates workouts
- `Client = 3` - Client who follows workout plans

### 3. Exercise Aggregate

**Location**: `src/api/src/Domain/Exercises/Exercise.cs`

**Aggregate Root**: Yes  
**Strongly Typed ID**: `ExerciseId` (Guid-based, using Vogen)

**Properties**:
- `Id: ExerciseId` - Unique identifier
- `Name: string` - Exercise name (max 100 characters)
- `Description: string?` - Optional description (max 500 characters)
- `Type: ExerciseType` - Exercise type (RepsAndWeight or TimeBased)
- `MuscleGroups: IReadOnlyList<MuscleGroup>` - Muscle groups targeted

**Business Rules** (enforced by domain):
- Name is required and must not exceed 100 characters
- Description is optional, max 500 characters if provided
- Exercise must have at least one muscle group
- Exercise type determines what parameters are required in workout plans

**Domain Methods**:
- `Exercise.Create(name, type, muscleGroups, description?)` - Factory method (returns ErrorOr<Exercise>)

**Domain Errors** (`ExerciseErrors.cs`):
- `NoMuscleGroups` - Exercise must have at least one muscle group

### 4. ExerciseType Enum

**Location**: `src/api/src/Domain/Exercises/ExerciseType.cs`

**Values**:
- `RepsAndWeight = 1` - Exercise measured by repetitions and optional weight (e.g., bench press)
- `TimeBased = 2` - Exercise measured by duration (e.g., plank, running)

### 5. MuscleGroup Enum

**Location**: `src/api/src/Domain/Exercises/MuscleGroup.cs`

**Values**:
- `Chest = 1`
- `Back = 2`
- `Shoulders = 3`
- `Biceps = 4`
- `Triceps = 5`
- `Forearms = 6`
- `Quadriceps = 7`
- `Hamstrings = 8`
- `Glutes = 9`
- `Calves = 10`
- `Abs = 11`
- `Obliques = 12`

### 6. WorkoutPlan Aggregate

**Location**: `src/api/src/Domain/WorkoutPlans/WorkoutPlan.cs`

**Aggregate Root**: Yes  
**Strongly Typed ID**: `WorkoutPlanId` (Guid-based, using Vogen)

**Properties**:
- `Id: WorkoutPlanId` - Unique identifier
- `Name: string` - Workout plan name (max 100 characters)
- `Description: string?` - Optional description (max 500 characters)
- `TrainerId: UserId` - Trainer who created this plan
- `Exercises: IReadOnlyList<PlannedExercise>` - Ordered list of exercises in plan

**Business Rules** (enforced by domain):
- Name is required and must not exceed 100 characters
- Description is optional, max 500 characters if provided
- Plan must belong to a trainer
- Exercises maintain order via Order property
- Cannot add duplicate exercises to plan

**Domain Methods**:
- `WorkoutPlan.Create(name, trainerId, description?)` - Factory method
- `AddExercise(exercise, sets, reps?, weight?, duration?)` - Add exercise to plan (returns ErrorOr<Success>)
- `RemoveExercise(exerciseId)` - Remove exercise from plan
- `UpdateExercise(exerciseId, sets, reps?, weight?, duration?)` - Update exercise parameters
- `ReorderExercises(newOrder)` - Reorder exercises in plan

**Domain Errors** (`WorkoutPlanErrors.cs`):
- `ExerciseAlreadyExists` - Exercise already in plan
- `ExerciseNotFound` - Exercise not found in plan
- `InvalidSets` - Sets must be >= 1
- `InvalidReps` - Reps must be >= 1 for RepsAndWeight exercises
- `InvalidDuration` - Duration must be provided for TimeBased exercises

### 7. PlannedExercise Entity

**Location**: `src/api/src/Domain/WorkoutPlans/PlannedExercise.cs`

**Entity** (not aggregate root, owned by WorkoutPlan)  
**Strongly Typed ID**: `PlannedExerciseId` (Guid-based, using Vogen)

**Properties**:
- `Id: PlannedExerciseId` - Unique identifier
- `ExerciseId: ExerciseId` - Reference to exercise
- `ExerciseName: string` - Snapshot of exercise name (max 100 characters)
- `ExerciseType: ExerciseType` - Snapshot of exercise type
- `Sets: int` - Number of sets (minimum 1)
- `Reps: int?` - Repetitions per set (required for RepsAndWeight)
- `Weight: Weight?` - Weight to use (optional for RepsAndWeight)
- `Duration: Duration?` - Duration (required for TimeBased)
- `Order: int` - Position in workout plan

**Business Rules** (enforced by domain):
- Sets must be >= 1
- For RepsAndWeight exercises: Reps required and must be >= 1
- For TimeBased exercises: Duration required
- Order determines sequence in workout plan

**Domain Methods** (internal - only called by WorkoutPlan):
- `PlannedExercise.Create(exerciseId, exerciseName, exerciseType, sets, order, reps?, weight?, duration?)` - Factory method
- `Update(sets, reps?, weight?, duration?)` - Update parameters
- `UpdateOrder(order)` - Update order position

### 8. Weight Value Object

**Location**: `src/api/src/Domain/Common/Weight.cs`

**Value Object**: Yes (immutable)

**Properties**:
- `Kilograms: decimal` - Weight in kilograms

**Business Rules**:
- Weight must be positive
- Immutable value object

**Note**: Frontend can display in pounds with conversion (1 kg = 2.20462 lbs)

### 9. Duration Value Object

**Location**: `src/api/src/Domain/Common/Duration.cs`

**Value Object**: Yes (immutable)

**Properties**:
- `Seconds: int` - Duration in seconds

**Business Rules**:
- Duration must be positive
- Immutable value object

**Note**: Frontend can display as minutes:seconds format

---

## Frontend TypeScript Models

The frontend uses TypeScript interfaces that mirror the backend domain entities for type safety. These are located in `src/frontend/src/lib/types/`.

### User Model

```typescript
// src/frontend/src/lib/types/user.ts

export enum UserRole {
  Admin = 1,
  Trainer = 2,
  Client = 3,
}

export interface User {
  id: string; // UserId.Value (Guid as string)
  name: string;
  email: string;
  roles: UserRole[];
  trainerId?: string; // UserId.Value (Guid as string)
  assignedWorkoutPlanIds: string[]; // WorkoutPlanId.Value[]
}

export interface CreateTrainerRequest {
  name: string;
  email: string;
}

export interface CreateClientRequest {
  name: string;
  email: string;
}

export interface UpdateUserRequest {
  name: string;
  email: string;
}

export interface CreateTrainerResponse {
  id: string;
}

export interface CreateClientResponse {
  id: string;
}
```

### Exercise Model

```typescript
// src/frontend/src/lib/types/exercise.ts

export enum ExerciseType {
  RepsAndWeight = 1,
  TimeBased = 2,
}

export enum MuscleGroup {
  Chest = 1,
  Back = 2,
  Shoulders = 3,
  Biceps = 4,
  Triceps = 5,
  Forearms = 6,
  Quadriceps = 7,
  Hamstrings = 8,
  Glutes = 9,
  Calves = 10,
  Abs = 11,
  Obliques = 12,
}

export interface Exercise {
  id: string; // ExerciseId.Value
  name: string;
  description?: string;
  type: ExerciseType;
  muscleGroups: MuscleGroup[];
}

export interface CreateExerciseRequest {
  name: string;
  type: ExerciseType;
  muscleGroups: MuscleGroup[];
  description?: string;
}

export interface UpdateExerciseRequest {
  name: string;
  description?: string;
  muscleGroups: MuscleGroup[];
}

export interface CreateExerciseResponse {
  id: string;
}
```

### Workout Plan Model

```typescript
// src/frontend/src/lib/types/workout-plan.ts

export interface Weight {
  kilograms: number;
}

export interface Duration {
  seconds: number;
}

export interface PlannedExercise {
  id: string; // PlannedExerciseId.Value
  exerciseId: string;
  exerciseName: string;
  exerciseType: ExerciseType;
  sets: number;
  reps?: number;
  weight?: Weight;
  duration?: Duration;
  order: number;
}

export interface WorkoutPlan {
  id: string; // WorkoutPlanId.Value
  name: string;
  description?: string;
  trainerId: string;
  exercises: PlannedExercise[];
}

export interface CreateWorkoutPlanRequest {
  name: string;
  description?: string;
}

export interface AddExerciseToPlanRequest {
  exerciseId: string;
  sets: number;
  reps?: number;
  weight?: Weight;
  duration?: Duration;
}

export interface UpdatePlannedExerciseRequest {
  sets: number;
  reps?: number;
  weight?: Weight;
  duration?: Duration;
}

export interface ReorderExercisesRequest {
  exerciseIds: string[]; // New order of exercise IDs
}

export interface AssignPlanToClientRequest {
  clientId: string;
  workoutPlanId: string;
}

export interface CreateWorkoutPlanResponse {
  id: string;
}
```

### API Error Models

```typescript
// src/frontend/src/lib/types/errors.ts

export interface ValidationError {
  [field: string]: string[]; // FluentValidation format
}

export interface DomainError {
  code: string;
  description: string;
  type: 'Validation' | 'NotFound' | 'Conflict' | 'Failure';
}

export interface ApiErrorResponse {
  errors?: ValidationError | DomainError[];
}

export class ApiError extends Error {
  constructor(
    public response: ApiErrorResponse,
    public statusCode: number
  ) {
    super('API Error');
  }

  isValidationError(): boolean {
    return this.response.errors && !Array.isArray(this.response.errors);
  }

  isDomainError(): boolean {
    return Array.isArray(this.response.errors);
  }

  getValidationErrors(): ValidationError | null {
    if (this.isValidationError()) {
      return this.response.errors as ValidationError;
    }
    return null;
  }

  getDomainErrors(): DomainError[] | null {
    if (this.isDomainError()) {
      return this.response.errors as DomainError[];
    }
    return null;
  }
}
```

---

## Database Schema (EF Core)

The database schema is managed by Entity Framework Core migrations. The following tables exist (already created):

### Users Table

```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    TrainerId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIMEOFFSET NOT NULL,
    UpdatedAt DATETIMEOFFSET NOT NULL,
    FOREIGN KEY (TrainerId) REFERENCES Users(Id)
);

-- Roles stored in separate table (many-to-many via owned collection)
CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    Role INT NOT NULL,
    PRIMARY KEY (UserId, Role),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Assigned workout plans (many-to-many)
CREATE TABLE UserWorkoutPlans (
    UserId UNIQUEIDENTIFIER NOT NULL,
    WorkoutPlanId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (UserId, WorkoutPlanId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (WorkoutPlanId) REFERENCES WorkoutPlans(Id) ON DELETE CASCADE
);
```

### Exercises Table

```sql
CREATE TABLE Exercises (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Type INT NOT NULL, -- ExerciseType enum
    CreatedAt DATETIMEOFFSET NOT NULL,
    UpdatedAt DATETIMEOFFSET NOT NULL
);

-- Muscle groups (many-to-many via owned collection)
CREATE TABLE ExerciseMuscleGroups (
    ExerciseId UNIQUEIDENTIFIER NOT NULL,
    MuscleGroup INT NOT NULL,
    PRIMARY KEY (ExerciseId, MuscleGroup),
    FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id) ON DELETE CASCADE
);
```

### WorkoutPlans Table

```sql
CREATE TABLE WorkoutPlans (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    TrainerId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIMEOFFSET NOT NULL,
    UpdatedAt DATETIMEOFFSET NOT NULL,
    FOREIGN KEY (TrainerId) REFERENCES Users(Id)
);

-- Planned exercises (owned by WorkoutPlan)
CREATE TABLE PlannedExercises (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    WorkoutPlanId UNIQUEIDENTIFIER NOT NULL,
    ExerciseId UNIQUEIDENTIFIER NOT NULL,
    ExerciseName NVARCHAR(100) NOT NULL,
    ExerciseType INT NOT NULL,
    Sets INT NOT NULL,
    Reps INT NULL,
    WeightKilograms DECIMAL(10, 2) NULL,
    DurationSeconds INT NULL,
    [Order] INT NOT NULL,
    FOREIGN KEY (WorkoutPlanId) REFERENCES WorkoutPlans(Id) ON DELETE CASCADE,
    FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id)
);
```

---

## Validation Rules

### Backend Validation (FluentValidation)

**CreateTrainerValidator**:
- Name: Required, max 100 characters
- Email: Required, valid email format, max 256 characters

**CreateClientValidator**:
- Name: Required, max 100 characters
- Email: Required, valid email format, max 256 characters

**UpdateUserValidator**:
- Name: Required, max 100 characters
- Email: Required, valid email format, max 256 characters

**CreateExerciseValidator**:
- Name: Required, max 100 characters
- Type: Required, must be valid ExerciseType
- MuscleGroups: Required, at least one, all must be valid MuscleGroup values
- Description: Optional, max 500 characters if provided

**UpdateExerciseValidator**:
- Name: Required, max 100 characters
- MuscleGroups: Required, at least one, all must be valid MuscleGroup values
- Description: Optional, max 500 characters if provided

**CreateWorkoutPlanValidator**:
- Name: Required, max 100 characters
- Description: Optional, max 500 characters if provided

**AddExerciseToPlanValidator**:
- ExerciseId: Required, must be valid GUID
- Sets: Required, must be >= 1
- Reps: Required if exercise type is RepsAndWeight, must be >= 1
- Weight: Optional
- Duration: Required if exercise type is TimeBased, must be > 0 seconds

**ReorderExercisesValidator**:
- ExerciseIds: Required, must not be empty, all must be valid GUIDs

### Frontend Validation (Zod)

Frontend validation mirrors backend validation to provide immediate feedback before API calls.

```typescript
// Example: src/frontend/src/lib/validation/user.ts
import { z } from 'zod';

export const createTrainerSchema = z.object({
  name: z.string().min(1, 'Name is required').max(100, 'Name must be 100 characters or less'),
  email: z.string().email('Invalid email format').max(256, 'Email must be 256 characters or less'),
});

export const createClientSchema = z.object({
  name: z.string().min(1, 'Name is required').max(100, 'Name must be 100 characters or less'),
  email: z.string().email('Invalid email format').max(256, 'Email must be 256 characters or less'),
});
```

---

## Relationships Diagram

```
User (Aggregate Root)
├── Has many Roles (UserRole enum)
├── Optional TrainerId reference (for clients)
└── Has many AssignedWorkoutPlanIds (for clients)

Exercise (Aggregate Root)
├── Has Type (ExerciseType enum)
└── Has many MuscleGroups (MuscleGroup enum)

WorkoutPlan (Aggregate Root)
├── Has TrainerId reference
└── Contains many PlannedExercises (owned entities)
    └── PlannedExercise references ExerciseId
```

**Key Relationships**:
- User ↔ User: Trainer-Client relationship (one trainer to many clients)
- User ↔ WorkoutPlan: Many-to-many (clients can have multiple plans, plans can be assigned to multiple clients)
- WorkoutPlan → User: One trainer per plan
- WorkoutPlan ↔ PlannedExercise: One-to-many (plan owns exercises)
- PlannedExercise → Exercise: Reference (snapshot exercise details)

---

## Status

✅ **Data model complete** - All entities exist in Domain layer, TypeScript models defined, validation rules specified.

**Next**: API Contracts (OpenAPI specifications)
