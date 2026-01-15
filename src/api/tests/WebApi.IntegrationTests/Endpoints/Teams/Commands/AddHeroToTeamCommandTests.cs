using Ardalis.Specification.EntityFrameworkCore;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Teams;
using GymBuddy.Api.Features.Teams.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Teams.Commands;

public class AddHeroToTeamCommandTests : IntegrationTestBase
{
    [Test]
    public async Task Command_ShouldAddHeroToTeam()
    {
        // Arrange
        var hero = HeroFactory.Generate();
        var team = TeamFactory.Generate();
        await AddAsync(team);
        await AddAsync(hero);
        var cmd = new AddHeroToTeamRequest(team.Id.Value, hero.Id.Value);
        var client = GetAnonymousClient();
        
        // Act
        var result = await client.POSTAsync<AddHeroToTeamEndpoint, AddHeroToTeamRequest>(cmd);

        // Assert
        var updatedTeam = await GetQueryable<Team>()
            .WithSpecification(new TeamByIdSpec(team.Id))
            .FirstOrDefaultAsync(CancellationToken);

        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await Assert.That(updatedTeam).IsNotNull();
        await Assert.That(updatedTeam!.Heroes).Count().IsEqualTo(1);
        await Assert.That(updatedTeam.Heroes.First().Id).IsEqualTo(hero.Id);
        await Assert.That(updatedTeam.TotalPowerLevel).IsEqualTo(hero.PowerLevel);
    }
}
