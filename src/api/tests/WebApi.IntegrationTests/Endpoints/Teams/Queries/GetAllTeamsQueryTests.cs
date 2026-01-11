using FastEndpoints;
using GymBuddy.Api.Features.Teams.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Teams.Queries;

public class GetAllTeamsQueryTests : IntegrationTestBase
{
    [Test]
    public async Task Query_ShouldReturnAllTeams()
    {
        // Arrange
        const int entityCount = 10;
        var entities = TeamFactory.Generate(entityCount);
        await AddRangeAsync(entities);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetAllTeamsEndpoint, GetAllTeamsResponse>();

        // Assert
        await Assert.That(result.Response.IsSuccessStatusCode).IsTrue();
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Teams).HasCount().EqualTo(entityCount);

        var firstTeam = result.Result.Teams.First();
        await Assert.That(firstTeam.Id).IsNotEqualTo(Guid.Empty);
        await Assert.That(firstTeam.Name).IsNotNull().And.IsNotEmpty();
    }
}
