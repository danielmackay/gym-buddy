using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.Users.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Users;

public class CreateClientEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task CreateClient_WithValidRequest_ShouldCreateClientAndAssignTrainer()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateClientRequest(
            Name: "Jane Smith",
            Email: "jane.smith@example.com",
            TrainerId: trainer.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsNotEqualTo(Guid.Empty);

        // Verify the user was created in the database
        var userId = UserId.From(result.Result.Id);
        var user = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);

        await Assert.That(user).IsNotNull();
        await Assert.That(user!.Name).IsEqualTo(request.Name);
        await Assert.That(user.Email).IsEqualTo(request.Email);
        await Assert.That(user.Roles).Contains(UserRole.Client);
        await Assert.That(user.Roles).HasCount().EqualTo(1);
        await Assert.That(user.TrainerId).IsNotNull();
        await Assert.That(user.TrainerId!.Value.Value).IsEqualTo(trainer.Id.Value);
    }

    [Test]
    public async Task CreateClient_WithNonExistentTrainer_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentTrainerId = Guid.NewGuid();
        var request = new CreateClientRequest(
            Name: "Jane Smith",
            Email: "jane.smith@example.com",
            TrainerId: nonExistentTrainerId);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        
        // Verify no user was created
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateClient_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateClientRequest(
            Name: "",
            Email: "jane.smith@example.com",
            TrainerId: trainer.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify only the trainer exists
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateClient_WithEmptyEmail_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateClientRequest(
            Name: "Jane Smith",
            Email: "",
            TrainerId: trainer.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify only the trainer exists
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateClient_WithInvalidEmail_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateClientRequest(
            Name: "Jane Smith",
            Email: "not-an-email",
            TrainerId: trainer.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify only the trainer exists
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateClient_WithEmptyTrainerId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateClientRequest(
            Name: "Jane Smith",
            Email: "jane.smith@example.com",
            TrainerId: Guid.Empty);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify no user was created
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateClient_WithNameTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateClientRequest(
            Name: new string('A', User.NameMaxLength + 1),
            Email: "jane.smith@example.com",
            TrainerId: trainer.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify only the trainer exists
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateClient_WithEmailTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var longEmail = new string('a', User.EmailMaxLength) + "@example.com"; // Exceeds max length
        var request = new CreateClientRequest(
            Name: "Jane Smith",
            Email: longEmail,
            TrainerId: trainer.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify only the trainer exists
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateClient_WithMultipleTrainers_ShouldCreateClientForCorrectTrainer()
    {
        // Arrange
        var trainer1 = UserFactory.GenerateTrainer();
        var trainer2 = UserFactory.GenerateTrainer();
        await AddRangeAsync(new[] { trainer1, trainer2 });

        var request = new CreateClientRequest(
            Name: "Jane Smith",
            Email: "jane.smith@example.com",
            TrainerId: trainer1.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateClientEndpoint, CreateClientRequest, CreateClientResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);

        // Verify the client is assigned to the correct trainer
        var userId = UserId.From(result.Result!.Id);
        var user = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(user!.TrainerId!.Value.Value).IsEqualTo(trainer1.Id.Value);
        await Assert.That(user.TrainerId!.Value.Value).IsNotEqualTo(trainer2.Id.Value);
    }
}
