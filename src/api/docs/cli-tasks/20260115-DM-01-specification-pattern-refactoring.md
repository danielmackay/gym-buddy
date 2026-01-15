# Specification Pattern Refactoring - Integration Tests

**Date:** 2026-01-15  
**Author:** Daniel Mackay (DM)

## Summary

Successfully refactored all integration tests to use the **Specification Pattern** for entity loading instead of direct EF Core queries. This change enforces consistency across the codebase and ensures domain specifications are the single source of truth for query logic.

## Changes Made

### 1. Enhanced Domain Specifications with Eager Loading

Updated specifications to include navigation properties by default, following the pattern established by `TeamByIdSpec`:

#### `/src/Domain/WorkoutPlans/WorkoutPlanByIdSpec.cs`
```csharp
public sealed class WorkoutPlanByIdSpec : SingleResultSpecification<WorkoutPlan>
{
    public WorkoutPlanByIdSpec(WorkoutPlanId workoutPlanId)
    {
        Query.Where(wp => wp.Id == workoutPlanId)
            .Include(wp => wp.Exercises); // Added eager loading
    }
}
```

#### `/src/Domain/WorkoutSessions/WorkoutSessionByIdSpec.cs`
```csharp
public sealed class WorkoutSessionByIdSpec : SingleResultSpecification<WorkoutSession>
{
    public WorkoutSessionByIdSpec(WorkoutSessionId sessionId)
    {
        Query.Where(s => s.Id == sessionId)
            .Include(s => s.Exercises); // Added eager loading
    }
}
```

**Design Decision:** Following **Option C** - always include navigation properties in `ByIdSpec`. This aligns with DDD principles where loading an aggregate by ID should return the complete aggregate with its child entities.

### 2. Replaced Direct EF Queries with Specifications

Replaced 3 occurrences of `GetTrackedQueryable<T>().Include(...).FirstAsync(...)` with `GetBySpecAsync(new SpecByIdSpec(id))`:

#### `WorkoutPlanPersistenceTests.cs`
- **Line 198**: `Update_RemoveExercise_ShouldPersistExerciseRemoval` test
- **Line 236**: `Update_UpdateExercise_ShouldPersistExerciseChanges` test

#### `WorkoutSessionPersistenceTests.cs`
- **Line 168**: `Update_CompleteExercise_ShouldPersistActualValues` test

**Before:**
```csharp
var planToUpdate = await GetTrackedQueryable<WorkoutPlan>()
    .Include(w => w.Exercises)
    .FirstAsync(w => w.Id == workoutPlanId, CancellationToken);
```

**After:**
```csharp
var planToUpdate = await GetBySpecAsync(new WorkoutPlanByIdSpec(workoutPlanId));
```

### 3. Marked Legacy Method as Obsolete

Updated `IntegrationTestBase.GetTrackedQueryable<T>()`:
```csharp
[Obsolete("Use GetBySpecAsync with domain specifications instead. This ensures consistent query patterns and proper eager loading of navigation properties.")]
protected IQueryable<T> GetTrackedQueryable<T>() where T : class => _dbContext.Set<T>();
```

## Benefits

### 1. **Consistency**
All entity loading now goes through domain specifications, ensuring:
- Predictable eager loading behavior
- Single source of truth for queries
- No ad-hoc EF queries scattered through tests

### 2. **DDD Alignment**
Specifications represent domain queries, making the intent clear:
- `UserByIdSpec` - load a user aggregate
- `WorkoutPlanByIdSpec` - load a workout plan with exercises
- `WorkoutSessionByIdSpec` - load a session with exercises

### 3. **Maintainability**
- Changes to query logic (e.g., adding new includes) happen in one place
- Tests don't need to know about EF Core navigation properties
- Reduced coupling between tests and infrastructure

### 4. **Type Safety**
- Compiler ensures specifications exist for entities
- No runtime errors from forgetting to include navigation properties
- Clear API: `GetBySpecAsync(new WorkoutPlanByIdSpec(id))`

## Test Results

All **44 integration tests** pass:
- 10 User persistence tests
- 7 Exercise persistence tests
- 10 WorkoutPlan persistence tests
- 8 WorkoutSession persistence tests
- Plus endpoint integration tests

```
Test run summary: Passed!
  total: 44
  succeeded: 44
  duration: 11s 747ms
```

## Previous Work

This refactoring builds on previous work:

1. **Created comprehensive persistence tests** for 4 domain entities
2. **Fixed EF Core configuration** - changed from `ComplexProperty` to `OwnsOne` for nullable value objects
3. **Added specification support** to `IntegrationTestBase` via `GetBySpecAsync<T>()`
4. **Replaced 15 usages of `FindAsync`** with `GetBySpecAsync` and domain specifications

## Remaining Work

### Assertion Pattern
Tests still use `GetQueryable<T>().Include(...).FirstOrDefaultAsync(...)` in assertion sections (11 occurrences). This is acceptable because:
- Assertions need to verify actual database state
- Using `.AsNoTracking()` is appropriate for read-only verification
- These don't modify entities, so specifications for updates aren't needed

### Optional: Create Read-Only Specifications
Consider creating separate read-only specifications for assertions if the Include patterns become complex or are reused across tests.

## Architecture Notes

### Specification Pattern in This Codebase

Following [SSW Rules on Specification Pattern](https://www.ssw.com.au/rules/use-specification-pattern/):

- **Location**: `src/Domain/{Entity}/{Entity}Spec.cs`
- **Base Classes**: 
  - `SingleResultSpecification<T>` - for queries returning 0 or 1 results
  - `Specification<T>` - for queries returning multiple results
- **Usage**: Via Ardalis.Specification's `.WithSpecification(spec)` extension
- **Purpose**: Encapsulate query logic in reusable, testable, domain-focused classes

### Aggregate Loading Pattern

When loading an aggregate root by ID, always include child entities:
```csharp
Query.Where(entity => entity.Id == id)
    .Include(entity => entity.Children);
```

This ensures the aggregate is loaded in a consistent state, respecting DDD aggregate boundaries.

## Files Modified

### Domain Layer
- `src/Domain/WorkoutPlans/WorkoutPlanByIdSpec.cs` - Added `.Include(wp => wp.Exercises)`
- `src/Domain/WorkoutSessions/WorkoutSessionByIdSpec.cs` - Added `.Include(s => s.Exercises)`

### Test Layer
- `tests/WebApi.IntegrationTests/Common/IntegrationTestBase.cs` - Marked `GetTrackedQueryable` as obsolete
- `tests/WebApi.IntegrationTests/Persistence/WorkoutPlanPersistenceTests.cs` - Replaced 2 usages with specifications
- `tests/WebApi.IntegrationTests/Persistence/WorkoutSessionPersistenceTests.cs` - Replaced 1 usage with specification

## Commands Used

```bash
# Build integration tests
dotnet build tests/WebApi.IntegrationTests/WebApi.IntegrationTests.csproj

# Run all integration tests
dotnet exec tests/WebApi.IntegrationTests/bin/Debug/net10.0/WebApi.IntegrationTests.dll
```

## Key Takeaways

1. **Always use specifications** for loading entities in tests and production code
2. **`ByIdSpec` should include navigation properties** to load complete aggregates
3. **Follow established patterns** - `TeamByIdSpec` was the model for this refactoring
4. **Prefer domain patterns over infrastructure** - specifications over raw EF queries
5. **Mark legacy methods as obsolete** to guide developers toward better patterns
