using Ardalis.Specification.EntityFrameworkCore;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Heroes;
using GymBuddy.Domain.Teams;
using GymBuddy.Api.Features.Heroes.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using GymBuddy.Api.IntegrationTests.Common.Utilities;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Teams.Events;

public class UpdatePowerLevelEventTests : IntegrationTestBase
{
    [Test]
    public async Task Command_UpdatePowerOnTeam()
    {
        // Arrange
        var hero = HeroFactory.Generate();
        var team = TeamFactory.Generate();
        List<Power> powers = [new Power("Strength", 10)];
        hero.UpdatePowers(powers);
        team.AddHero(hero);
        await AddAsync(team);
        powers.Add(new Power("Speed", 5));
        var powerDtos = powers.Select(p => new UpdateHeroRequest.HeroPowerDto(p.Name, p.PowerLevel));
        var cmd = new UpdateHeroRequest(hero.Name, hero.Alias, hero.Id.Value, powerDtos);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateHeroEndpoint, UpdateHeroRequest>(cmd);

        // Assert
        await Wait.ForEventualConsistency();
        var updatedTeam = await GetQueryable<Team>()
            .WithSpecification(new TeamByIdSpec(team.Id))
            .FirstOrDefaultAsync(CancellationToken);

        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await Assert.That(updatedTeam!.TotalPowerLevel).IsEqualTo(15);
    }
}
