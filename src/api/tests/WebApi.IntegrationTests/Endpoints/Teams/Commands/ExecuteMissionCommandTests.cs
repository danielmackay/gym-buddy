using Ardalis.Specification.EntityFrameworkCore;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Teams;
using GymBuddy.Api.Features.Teams.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Teams.Commands;

public class ExecuteMissionCommandTests : IntegrationTestBase
{
    [Test]
    public async Task Command_ShouldExecuteMission()
    {
        // Arrange
        var hero = HeroFactory.Generate();
        var team = TeamFactory.Generate();
        team.AddHero(hero);
        await AddAsync(team);
        var cmd = new ExecuteMissionRequest(team.Id.Value, "Save the world");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<ExecuteMissionEndpoint, ExecuteMissionRequest>(cmd);

        // Assert
        var updatedTeam = await GetQueryable<Team>()
            .WithSpecification(new TeamByIdSpec(team.Id))
            .FirstOrDefaultAsync(CancellationToken);
        var mission = updatedTeam!.Missions.First();

        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await Assert.That(updatedTeam!.Missions).Count().IsEqualTo(1);
        await Assert.That(updatedTeam.Status).IsEqualTo(TeamStatus.OnMission);
        await Assert.That(mission.Status).IsEqualTo(MissionStatus.InProgress);
    }
}
