using Microsoft.EntityFrameworkCore;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Api.IntegrationTests.Persistence;

public class UserPersistenceTests : IntegrationTestBase
{
    [Test]
    public async Task CreateAndSave_ShouldPersistUser()
    {
        // Arrange
        var user = UserFactory.Generate();
        UserId userId = user.Id;

        // Act
        await AddAsync(user);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo(user.Name);
        await Assert.That(retrieved.Email).IsEqualTo(user.Email);
        await Assert.That(retrieved.CreatedAt).IsBetween(DateTimeOffset.UtcNow.AddSeconds(-10), DateTimeOffset.UtcNow.AddSeconds(10));
    }

    [Test]
    public async Task CreateAndSave_WithRoles_ShouldPersistUserWithRoles()
    {
        // Arrange
        var user = UserFactory.Generate();
        user.AddRole(UserRole.Trainer);
        user.AddRole(UserRole.Client);
        UserId userId = user.Id;

        // Act
        await AddAsync(user);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Roles).HasCount().EqualTo(2);
        await Assert.That(retrieved.Roles).Contains(UserRole.Trainer);
        await Assert.That(retrieved.Roles).Contains(UserRole.Client);
    }

    [Test]
    public async Task CreateAndSave_WithTrainerRelationship_ShouldPersistTrainerAssignment()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClientWithTrainer(trainerId);
        UserId clientId = client.Id;

        // Act
        await AddAsync(client);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == clientId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.TrainerId).IsNotNull();
        await Assert.That(retrieved.TrainerId).IsEqualTo(trainerId);
    }

    [Test]
    public async Task CreateAndSave_WithAssignedWorkoutPlan_ShouldPersistWorkoutPlanAssignment()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        WorkoutPlanId workoutPlanId = workoutPlan.Id;
        await AddAsync(workoutPlan);

        var client = UserFactory.GenerateClient();
        client.AssignWorkoutPlan(workoutPlanId);
        UserId clientId = client.Id;

        // Act
        await AddAsync(client);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == clientId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.AssignedWorkoutPlanIds).HasCount().EqualTo(1);
        await Assert.That(retrieved.AssignedWorkoutPlanIds).Contains(workoutPlanId);
    }

    [Test]
    public async Task Update_ShouldPersistChanges()
    {
        // Arrange
        var user = UserFactory.Generate();
        UserId userId = user.Id;
        await AddAsync(user);

        // Act
        var userToUpdate = await GetBySpecAsync(new UserByIdSpec(userId));
        userToUpdate!.Name = "Updated Name";
        userToUpdate.Email = "updated@email.com";
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo("Updated Name");
        await Assert.That(retrieved.Email).IsEqualTo("updated@email.com");
        await Assert.That(retrieved.UpdatedAt).IsNotNull();
    }

    [Test]
    public async Task Update_AddRole_ShouldPersistNewRole()
    {
        // Arrange
        var user = UserFactory.Generate();
        UserId userId = user.Id;
        await AddAsync(user);

        // Act
        var userToUpdate = await GetBySpecAsync(new UserByIdSpec(userId));
        userToUpdate!.AddRole(UserRole.Admin);
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Roles).HasCount().EqualTo(1);
        await Assert.That(retrieved.Roles).Contains(UserRole.Admin);
    }

    [Test]
    public async Task Update_RemoveRole_ShouldPersistRoleRemoval()
    {
        // Arrange
        var user = UserFactory.Generate();
        user.AddRole(UserRole.Trainer);
        user.AddRole(UserRole.Admin);
        UserId userId = user.Id;
        await AddAsync(user);

        // Act
        var userToUpdate = await GetBySpecAsync(new UserByIdSpec(userId));
        userToUpdate!.RemoveRole(UserRole.Trainer);
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Roles).HasCount().EqualTo(1);
        await Assert.That(retrieved.Roles).Contains(UserRole.Admin);
        await Assert.That(retrieved.Roles).DoesNotContain(UserRole.Trainer);
    }

    [Test]
    public async Task Update_UnassignTrainer_ShouldPersistTrainerRemoval()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClientWithTrainer(trainerId);
        UserId clientId = client.Id;
        await AddAsync(client);

        // Act
        var clientToUpdate = await GetBySpecAsync(new UserByIdSpec(clientId));
        clientToUpdate!.UnassignTrainer();
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == clientId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.TrainerId).IsNull();
    }

    [Test]
    public async Task Delete_ShouldRemoveUser()
    {
        // Arrange
        var user = UserFactory.Generate();
        UserId userId = user.Id;
        await AddAsync(user);

        // Act
        var userToDelete = await GetBySpecAsync(new UserByIdSpec(userId));
        await RemoveAsync(userToDelete!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(retrieved).IsNull();
    }

    [Test]
    public async Task Delete_UserWithRoles_ShouldRemoveUserAndRoles()
    {
        // Arrange
        var user = UserFactory.Generate();
        user.AddRole(UserRole.Trainer);
        user.AddRole(UserRole.Client);
        UserId userId = user.Id;
        await AddAsync(user);

        // Act
        var userToDelete = await GetBySpecAsync(new UserByIdSpec(userId));
        await RemoveAsync(userToDelete!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(retrieved).IsNull();
    }
}
