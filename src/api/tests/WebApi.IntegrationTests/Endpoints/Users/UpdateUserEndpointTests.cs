using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.Users.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Users;

public class UpdateUserEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task UpdateUser_WithValidRequest_ShouldUpdateUser()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var request = new UpdateUserRequest(
            Id: user.Id.Value,
            Name: "Updated Name",
            Email: "updated.email@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsEqualTo(user.Id.Value);
        await Assert.That(result.Result.Name).IsEqualTo(request.Name);
        await Assert.That(result.Result.Email).IsEqualTo(request.Email);
        await Assert.That(result.Result.Roles).Contains(UserRole.Trainer);

        // Verify the user was updated in the database
        DetachAllEntities();
        var userId = UserId.From(user.Id.Value);
        var updatedUser = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(updatedUser).IsNotNull();
        await Assert.That(updatedUser!.Name).IsEqualTo(request.Name);
        await Assert.That(updatedUser.Email).IsEqualTo(request.Email);
    }

    [Test]
    public async Task UpdateUser_WithClient_ShouldPreserveTrainerAssignment()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, client });

        var request = new UpdateUserRequest(
            Id: client.Id.Value,
            Name: "Updated Client Name",
            Email: "updated.client@example.com");
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Name).IsEqualTo(request.Name);
        await Assert.That(result.Result.Email).IsEqualTo(request.Email);
        await Assert.That(result.Result.TrainerId).IsEqualTo(trainer.Id.Value); // Trainer preserved

        // Verify in database
        DetachAllEntities();
        var userId = UserId.From(client.Id.Value);
        var updatedUser = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(updatedUser!.TrainerId).IsNotNull();
        await Assert.That(updatedUser.TrainerId!.Value.Value).IsEqualTo(trainer.Id.Value);
    }

    [Test]
    public async Task UpdateUser_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateUserRequest(
            Id: nonExistentId,
            Name: "Updated Name",
            Email: "updated.email@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateUser_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var request = new UpdateUserRequest(
            Id: user.Id.Value,
            Name: "",
            Email: "updated.email@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify the user was not updated
        DetachAllEntities();
        var userId = UserId.From(user.Id.Value);
        var unchangedUser = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(unchangedUser!.Name).IsEqualTo(user.Name);
        await Assert.That(unchangedUser.Email).IsEqualTo(user.Email);
    }

    [Test]
    public async Task UpdateUser_WithEmptyEmail_ShouldReturnValidationError()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var request = new UpdateUserRequest(
            Id: user.Id.Value,
            Name: "Updated Name",
            Email: "");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify the user was not updated
        DetachAllEntities();
        var userId = UserId.From(user.Id.Value);
        var unchangedUser = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(unchangedUser!.Name).IsEqualTo(user.Name);
        await Assert.That(unchangedUser.Email).IsEqualTo(user.Email);
    }

    [Test]
    public async Task UpdateUser_WithInvalidEmail_ShouldReturnValidationError()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var request = new UpdateUserRequest(
            Id: user.Id.Value,
            Name: "Updated Name",
            Email: "not-an-email");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify the user was not updated
        DetachAllEntities();
        var userId = UserId.From(user.Id.Value);
        var unchangedUser = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(unchangedUser!.Email).IsEqualTo(user.Email);
    }

    [Test]
    public async Task UpdateUser_WithNameTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var request = new UpdateUserRequest(
            Id: user.Id.Value,
            Name: new string('A', User.NameMaxLength + 1),
            Email: "updated.email@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify the user was not updated
        DetachAllEntities();
        var userId = UserId.From(user.Id.Value);
        var unchangedUser = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(unchangedUser!.Name).IsEqualTo(user.Name);
    }

    [Test]
    public async Task UpdateUser_WithEmailTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var longEmail = new string('a', User.EmailMaxLength) + "@example.com"; // Exceeds max length
        var request = new UpdateUserRequest(
            Id: user.Id.Value,
            Name: "Updated Name",
            Email: longEmail);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify the user was not updated
        DetachAllEntities();
        var userId = UserId.From(user.Id.Value);
        var unchangedUser = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(unchangedUser!.Email).IsEqualTo(user.Email);
    }

    [Test]
    public async Task UpdateUser_WithEmptyId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new UpdateUserRequest(
            Id: Guid.Empty,
            Name: "Updated Name",
            Email: "updated.email@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateUser_WithMaxLengthValues_ShouldSucceed()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var maxLengthName = new string('A', User.NameMaxLength);
        var request = new UpdateUserRequest(
            Id: user.Id.Value,
            Name: maxLengthName,
            Email: "updated@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateUserEndpoint, UpdateUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result!.Name).IsEqualTo(maxLengthName);
    }
}
