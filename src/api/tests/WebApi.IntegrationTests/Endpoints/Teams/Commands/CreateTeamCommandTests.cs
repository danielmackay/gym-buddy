using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Teams;
using GymBuddy.Api.Features.Teams.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Teams.Commands;

public class CreateTeamCommandTests : IntegrationTestBase
{
    [Test]
    public async Task Command_ShouldCreateTeam()
    {
        // Arrange
        var cmd = new CreateTeamRequest("Clark Kent");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateTeamEndpoint, CreateTeamRequest>(cmd);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        var item = await GetQueryable<Team>().FirstAsync(CancellationToken);

        await Assert.That(item).IsNotNull();
        await Assert.That(item.Name).IsEqualTo(cmd.Name);
        await Assert.That(item.CreatedAt).IsBetween(DateTime.Now.AddSeconds(-10), DateTime.Now.AddSeconds(10));
    }
}
