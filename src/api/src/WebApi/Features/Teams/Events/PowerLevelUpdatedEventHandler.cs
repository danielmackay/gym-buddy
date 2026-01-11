using Ardalis.Specification.EntityFrameworkCore;
using GymBuddy.Api.Common.Domain.Base.EventualConsistency;
using GymBuddy.Api.Common.Domain.Heroes;
using GymBuddy.Api.Common.Domain.Teams;

namespace GymBuddy.Api.Features.Teams.Events;

public class PowerLevelUpdatedEventHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<PowerLevelUpdatedEventHandler> logger)
    : IEventHandler<PowerLevelUpdatedEvent>
{
    public async Task HandleAsync(PowerLevelUpdatedEvent eventModel, CancellationToken ct)
    {
        logger.LogInformation("PowerLevelUpdatedEventHandler: {HeroName} power updated to {PowerLevel}",
        eventModel.Hero.Name, eventModel.Hero.PowerLevel);

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.Resolve<ApplicationDbContext>();


        var hero = await dbContext.Heroes.FirstAsync(h => h.Id == eventModel.Hero.Id,
            cancellationToken: ct);

        if (hero.TeamId is null)
        {
            logger.LogInformation("Hero {HeroName} is not on a team - nothing to do", eventModel.Hero.Name);
            return;
        }

        var team = dbContext.Teams
            .WithSpecification(new TeamByIdSpec(hero.TeamId.Value))
            .FirstOrDefault();

        if (team is null)
            throw new EventualConsistencyException(PowerLevelUpdatedEvent.TeamNotFound);

        team.ReCalculatePowerLevel();
        await dbContext.SaveChangesAsync(ct);
    }
}