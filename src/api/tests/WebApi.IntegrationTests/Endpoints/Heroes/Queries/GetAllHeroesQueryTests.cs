using FastEndpoints;
using GymBuddy.Api.Features.Heroes.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Heroes.Queries;

public class GetAllHeroesQueryTests : IntegrationTestBase
{
    [Test]
    public async Task Query_ShouldReturnAllHeroes()
    {
        // Arrange
        const int entityCount = 10;
        var entities = HeroFactory.Generate(entityCount);
        await AddRangeAsync(entities);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetAllHeroesEndpoint, GetAllHeroesResponse>();

        // Assert
        await Assert.That(result.Response.IsSuccessStatusCode).IsTrue();
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Heroes).Count().IsEqualTo(entityCount);
    }
}
