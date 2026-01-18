# .NET Test Suite Guide

## Overview

The test suite consists of four test projects covering different aspects of the application:

1. **Domain.UnitTests** - Unit tests for domain entities and value objects
2. **WebApi.UnitTests** - Unit tests for API-specific logic
3. **WebApi.IntegrationTests** - Integration tests for HTTP endpoints and database interactions
4. **WebApi.ArchitectureTests** - Architecture tests enforcing VSA/DDD patterns

## Test Framework

All tests use **TUnit** (v1.9.68+) - a modern, source-generated test framework for .NET.

### TUnit Assertion Syntax

**IMPORTANT:** TUnit uses a different assertion syntax than xUnit/NUnit:

✅ **Correct:**
```csharp
await Assert.That(result).IsNotNull();
await Assert.That(collection).Count().IsEqualTo(5);
await Assert.That(value).IsEqualTo(expected);
await Assert.That(list).Contains(item);
await Assert.That(list).IsEmpty();
```

❌ **Incorrect (old syntax):**
```csharp
await Assert.That(collection).HasCount().EqualTo(5);  // WRONG!
```

**Key Pattern:** Use `.Count().IsEqualTo()` instead of `.HasCount().EqualTo()`

## Running Tests

### Run All Tests
```bash
# From repository root
cd src/api
dotnet run --project tests/WebApi.IntegrationTests/
dotnet run --project tests/Domain.UnitTests/
dotnet run --project tests/WebApi.UnitTests/
dotnet run --project tests/WebApi.ArchitectureTests/
```

### Build Tests
```bash
cd src/api
dotnet build tests/WebApi.IntegrationTests/
```

### Run Tests with Filter (TUnit Syntax)
```bash
cd src/api/tests/WebApi.IntegrationTests
dotnet run --no-build -- --treenode-filter "*/CreateTrainer*"
```

### .NET 10 Testing Platform

**Note:** The legacy `dotnet test` command is deprecated on .NET 10 SDK. Use `dotnet run` instead:

```bash
# ❌ Old way (doesn't work with .NET 10 + TUnit)
dotnet test tests/WebApi.IntegrationTests/

# ✅ New way (correct for .NET 10 + TUnit)
dotnet run --project tests/WebApi.IntegrationTests/
```

## Test Structure

### Domain.UnitTests
**Location:** `tests/Domain.UnitTests/`

Tests for domain entities, value objects, and business logic. These tests MUST be written for ALL domain code.

**Examples:**
- `Common/DurationTests.cs` - Tests for Duration value object
- `Common/WeightTests.cs` - Tests for Weight value object
- `Scenarios/CompleteWorkoutSessionScenarioTests.cs` - End-to-end domain scenarios

**Coverage Requirements:**
- ALL domain entities must have unit tests
- ALL value objects must have unit tests
- ALL business logic must have unit tests

### WebApi.IntegrationTests
**Location:** `tests/WebApi.IntegrationTests/`

Integration tests for HTTP endpoints using TestContainers with SQL Server.

**Test Base Class:** `IntegrationTestBase`
- Provides database setup/teardown
- Provides HTTP client factory
- Uses Respawn for database cleanup between tests

**Structure:**
```
Endpoints/
├── Users/              # User management endpoints
├── Exercises/          # Exercise CRUD endpoints
├── Heroes/             # Legacy hero endpoints
└── Teams/              # Legacy team endpoints
Persistence/            # Direct EF Core persistence tests
```

**Running Integration Tests:**
```bash
cd src/api
dotnet run --project tests/WebApi.IntegrationTests/
```

**Requirements:**
- Docker or Podman must be running (for SQL Server container)
- ALL endpoints must have integration tests
- Tests must cover happy path + validation errors

### WebApi.ArchitectureTests
**Location:** `tests/WebApi.ArchitectureTests/`

Architecture tests using NetArchTest to enforce:
- Vertical Slice Architecture patterns
- Domain-Driven Design rules
- Dependency rules
- Naming conventions

**Tests:**
- `DomainTests.cs` - Domain layer rules (entities, value objects, ErrorOr)
- `ApplicationTests.cs` - Application layer rules (endpoints, validators)

### WebApi.UnitTests
**Location:** `tests/WebApi.UnitTests/`

Unit tests for API-specific logic that doesn't require HTTP pipeline or database.

**Examples:**
- `Features/Heroes/HeroTests.cs`
- `Features/Teams/TeamTests.cs`

## Common Test Patterns

### Integration Test Pattern
```csharp
[Test]
public async Task EndpointName_WithValidRequest_ShouldReturnExpected()
{
    // Arrange
    var entity = EntityFactory.Generate();
    await AddAsync(entity);
    var request = new SomeRequest(...);
    var client = GetAnonymousClient();

    // Act
    var result = await client.POSTAsync<SomeEndpoint, SomeRequest, SomeResponse>(request);

    // Assert
    await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    await Assert.That(result.Result).IsNotNull();
    await Assert.That(result.Result!.Id).IsNotEqualTo(Guid.Empty);
    
    // Verify database state
    var dbEntity = await GetQueryable<Entity>().FirstOrDefaultAsync(...);
    await Assert.That(dbEntity).IsNotNull();
}
```

### Domain Unit Test Pattern
```csharp
[Test]
public async Task Create_WithValidInput_ShouldSucceed()
{
    // Arrange
    var name = "Test Name";
    var type = ExerciseType.RepsAndWeight;
    var muscleGroups = new[] { MuscleGroup.Chest };

    // Act
    var result = Exercise.Create(name, type, muscleGroups);

    // Assert
    await Assert.That(result.IsError).IsFalse();
    await Assert.That(result.Value.Name).IsEqualTo(name);
}
```

## Test Utilities

### Factories
**Location:** `tests/WebApi.IntegrationTests/Common/Factories/`

Generate test data using Bogus:
- `UserFactory.cs` - Generate Users (Trainers, Clients, Admins)
- `ExerciseFactory.cs` - Generate Exercises
- `WorkoutPlanFactory.cs` - Generate WorkoutPlans
- `WorkoutSessionFactory.cs` - Generate WorkoutSessions

**Example:**
```csharp
var trainer = UserFactory.GenerateTrainer();
var client = UserFactory.GenerateClientWithTrainer(trainer.Id);
var exercise = ExerciseFactory.GenerateRepsAndWeightExercise();
```

### IntegrationTestBase Helpers
```csharp
// Database operations
await AddAsync(entity);
await AddRangeAsync(entities);
await RemoveAsync(entity);
var queryable = GetQueryable<Entity>();

// HTTP clients
var client = GetAnonymousClient();
```

### TestContainers
SQL Server runs in a Docker container for integration tests. The container is:
- Automatically started before tests
- Shared across all test runs in a session
- Automatically cleaned up after tests

**Configuration:** `Common/Infrastructure/Database/SqlServerContainer.cs`

## TUnit-Specific Features

### Test Attributes
```csharp
[Test]                          // Marks a test method
[TestCase(1, 2, 3)]            // Parameterized test
[Before(Test)]                 // Setup method
[After(Test)]                  // Teardown method
[NotInParallel]                // Disable parallel execution
[Category("Integration")]      // Test categorization
```

### Assertion Methods
```csharp
// Equality
await Assert.That(value).IsEqualTo(expected);
await Assert.That(value).IsNotEqualTo(unexpected);

// Nullability
await Assert.That(value).IsNull();
await Assert.That(value).IsNotNull();

// Boolean
await Assert.That(condition).IsTrue();
await Assert.That(condition).IsFalse();

// Collections
await Assert.That(collection).Count().IsEqualTo(5);
await Assert.That(collection).Contains(item);
await Assert.That(collection).DoesNotContain(item);
await Assert.That(collection).IsEmpty();

// Strings
await Assert.That(str).Contains("substring");
await Assert.That(str).StartsWith("prefix");
```

## Common Issues & Solutions

### Issue: "Testing with VSTest target is no longer supported"
**Solution:** Use `dotnet run --project` instead of `dotnet test` on .NET 10 SDK.

### Issue: Tests not running with filter
**Solution:** Use `--treenode-filter` (not `--filter`) with TUnit:
```bash
dotnet run -- --treenode-filter "*/TestName*"
```

### Issue: SQL Server container fails to start
**Solution:** Ensure Docker/Podman is running and port 1433 is available.

### Issue: Tests fail with "Database does not exist"
**Solution:** MigrationService runs automatically in tests. Check that migrations are up to date.

## Test Coverage Requirements

### Mandatory Test Coverage
- ✅ **ALL domain entities** must have unit tests
- ✅ **ALL value objects** must have unit tests
- ✅ **ALL endpoints** must have integration tests
- ✅ **ALL business logic** must have unit tests

### Integration Test Requirements
Each endpoint must have tests for:
1. Happy path (valid request → success response)
2. Validation errors (invalid request → 400 Bad Request)
3. Not found errors (non-existent resource → 404 Not Found)
4. Database state verification (verify changes persisted)

### Example Coverage
For a `CreateExercise` endpoint:
- ✅ `CreateExercise_WithValidRequest_ShouldCreateExercise`
- ✅ `CreateExercise_WithEmptyName_ShouldReturnValidationError`
- ✅ `CreateExercise_WithTooLongName_ShouldReturnValidationError`
- ✅ `CreateExercise_WithNoMuscleGroups_ShouldReturnValidationError`

## Performance

### Parallel Execution
TUnit runs tests in parallel by default. Use `[NotInParallel]` for tests that cannot run concurrently:
```csharp
[Test]
[NotInParallel]
public async Task TestThatModifiesGlobalState() { ... }
```

### Database Cleanup
Respawn is used to clean the database between tests efficiently:
- Fast reset (truncates tables instead of dropping/recreating database)
- Preserves schema and migrations
- Automatic via `IntegrationTestBase`

## References

- **TUnit Documentation:** https://github.com/thomhurst/TUnit
- **TestContainers Documentation:** https://dotnet.testcontainers.org/
- **NetArchTest Documentation:** https://github.com/BenMorris/NetArchTest
- **Respawn Documentation:** https://github.com/jbogard/Respawn

## Recent Changes

- **2026-01-18:** Updated all assertions from `.HasCount().EqualTo()` to `.Count().IsEqualTo()` to align with TUnit syntax
- **2026-01-18:** Added `<EnableMSTestRunner>true</EnableMSTestRunner>` to project file for .NET 10 SDK compatibility
