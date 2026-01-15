using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Heroes;
using GymBuddy.Api.Features.Heroes.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Heroes.Commands;

public class CreateHeroCommandTests : IntegrationTestBase
{
    [Test]
    public async Task Command_ShouldCreateHero()
    {
        // Arrange
        (string Name, int PowerLevel)[] powers =
        [
            ("Heat vision", 7),
            ("Super-strength", 10),
            ("Flight", 8),
        ];
        var cmd = new CreateHeroRequest(
            "Clark Kent",
            "Superman",
            powers.Select(p => new CreateHeroRequest.HeroPowerDto(p.Name, p.PowerLevel)));
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateHeroEndpoint, CreateHeroRequest, CreateHeroResponse>(cmd);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var item = await GetQueryable<Hero>().FirstAsync(CancellationToken);

        await Assert.That(item).IsNotNull();
        await Assert.That(item.Name).IsEqualTo(cmd.Name);
        await Assert.That(item.Alias).IsEqualTo(cmd.Alias);
        await Assert.That(item.PowerLevel).IsEqualTo(25);
        await Assert.That(item.Powers).Count().IsEqualTo(3);
        await Assert.That(item.CreatedAt).IsBetween(DateTime.Now.AddSeconds(-10), DateTime.Now.AddSeconds(10));
    }
}
