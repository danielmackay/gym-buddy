using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Heroes;
using GymBuddy.Api.Features.Heroes.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Heroes.Commands;

public class UpdateHeroCommandTests : IntegrationTestBase
{
    [Test]
    public async Task Command_ShouldUpdateHero()
    {
        // Arrange
        var heroName = "2021-01-01T00:00:00Z";
        var heroAlias = "2021-01-01T00:00:00Z-alias";
        var hero = HeroFactory.Generate();
        await AddAsync(hero);
        (string Name, int PowerLevel)[] powers =
        [
            ("Heat vision", 7),
            ("Super-strength", 10),
            ("Flight", 8),
        ];
        var cmd = new UpdateHeroRequest(
            heroName,
            heroAlias,
            hero.Id.Value,
            powers.Select(p => new UpdateHeroRequest.HeroPowerDto(p.Name, p.PowerLevel)));
        var client = GetAnonymousClient();
        var createdTimeStamp = DateTime.Now;

        // Act
        var result = await client.PUTAsync<UpdateHeroEndpoint, UpdateHeroRequest>(cmd);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        var item = await GetQueryable<Hero>().FirstAsync(dbHero => dbHero.Id == hero.Id, CancellationToken);

        await Assert.That(item).IsNotNull();
        await Assert.That(item.Name).IsEqualTo(cmd.Name);
        await Assert.That(item.Alias).IsEqualTo(cmd.Alias);
        await Assert.That(item.PowerLevel).IsEqualTo(25);
        await Assert.That(item.Powers).Count().IsEqualTo(3);
        await Assert.That(item.UpdatedAt).IsNotNull();
        await Assert.That(item.UpdatedAt!.Value).IsBetween(new DateTimeOffset(createdTimeStamp.AddSeconds(-10)), new DateTimeOffset(createdTimeStamp.AddSeconds(10)));
    }

    [Test]
    public async Task Command_WhenHeroDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var heroId = HeroId.From(Guid.NewGuid());
        var cmd = new UpdateHeroRequest(
            "foo",
            "bar",
            heroId.Value,
            [new UpdateHeroRequest.HeroPowerDto("Heat vision", 7)]);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateHeroEndpoint, UpdateHeroRequest>(cmd);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        var item = await GetQueryable<Hero>().FirstOrDefaultAsync(dbHero => dbHero.Id == heroId, CancellationToken);

        await Assert.That(item).IsNull();
    }
}
