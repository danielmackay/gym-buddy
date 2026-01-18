using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.Users.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Users;

public class CreateTrainerEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task CreateTrainer_WithValidRequest_ShouldCreateTrainer()
    {
        // Arrange
        var request = new CreateTrainerRequest(
            Name: "John Doe",
            Email: "john.doe@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTrainerEndpoint, CreateTrainerRequest, CreateTrainerResponse>(request);

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
        await Assert.That(user.Roles).Contains(UserRole.Trainer);
        await Assert.That(user.Roles).Count().IsEqualTo(1);
        await Assert.That(user.TrainerId).IsNull();
    }

    [Test]
    public async Task CreateTrainer_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateTrainerRequest(
            Name: "",
            Email: "john.doe@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTrainerEndpoint, CreateTrainerRequest, CreateTrainerResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify no user was created
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateTrainer_WithEmptyEmail_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateTrainerRequest(
            Name: "John Doe",
            Email: "");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTrainerEndpoint, CreateTrainerRequest, CreateTrainerResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify no user was created
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateTrainer_WithInvalidEmail_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateTrainerRequest(
            Name: "John Doe",
            Email: "not-an-email");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTrainerEndpoint, CreateTrainerRequest, CreateTrainerResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify no user was created
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateTrainer_WithNameTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateTrainerRequest(
            Name: new string('A', User.NameMaxLength + 1),
            Email: "john.doe@example.com");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTrainerEndpoint, CreateTrainerRequest, CreateTrainerResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify no user was created
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateTrainer_WithEmailTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var longEmail = new string('a', User.EmailMaxLength) + "@example.com"; // Exceeds max length
        var request = new CreateTrainerRequest(
            Name: "John Doe",
            Email: longEmail);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTrainerEndpoint, CreateTrainerRequest, CreateTrainerResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        // Verify no user was created
        var userCount = await GetQueryable<User>().CountAsync(CancellationToken);
        await Assert.That(userCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateTrainer_WithMaxLengthValues_ShouldSucceed()
    {
        // Arrange
        var maxLengthName = new string('A', User.NameMaxLength);
        var maxLengthEmail = new string('a', User.EmailMaxLength - 12) + "@example.com"; // Within limit
        var request = new CreateTrainerRequest(
            Name: maxLengthName,
            Email: "a" + "@example.com"); // Valid email at max length
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTrainerEndpoint, CreateTrainerRequest, CreateTrainerResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        
        // Verify the user was created
        var userId = UserId.From(result.Result!.Id);
        var user = await GetQueryable<User>().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken);
        await Assert.That(user).IsNotNull();
    }
}
