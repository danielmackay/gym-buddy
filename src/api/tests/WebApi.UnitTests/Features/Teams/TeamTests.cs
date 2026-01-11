using GymBuddy.Domain.Heroes;
using GymBuddy.Domain.Teams;

namespace GymBuddy.Api.UnitTests.Features.Teams;

public class TeamTests
{
    [Test]
    [Arguments("c8ad9974-ca93-44a5-9215-2f4d9e866c7a", "cc3431a8-4a31-4f76-af64-e8198279d7a4", false)]
    [Arguments("c8ad9974-ca93-44a5-9215-2f4d9e866c7a", "c8ad9974-ca93-44a5-9215-2f4d9e866c7a", true)]
    public async Task TeamId_ShouldBeComparable(string stringGuid1, string stringGuid2, bool isEqual)
    {
        // Arrange
        Guid guid1 = Guid.Parse(stringGuid1);
        Guid guid2 = Guid.Parse(stringGuid2);
        TeamId id1 = TeamId.From(guid1);
        TeamId id2 = TeamId.From(guid2);

        // Act
        var areEqual = id1 == id2;

        // Assert
        await Assert.That(areEqual).IsEqualTo(isEqual);
        await Assert.That(id1.Value).IsEqualTo(guid1);
        await Assert.That(id2.Value).IsEqualTo(guid2);
    }

    [Test]
    public async Task Create_WithValidNameAndAlias_ShouldSucceed()
    {
        // Arrange
        var name = "name";

        // Act
        var team = Team.Create(name);

        // Assert
        await Assert.That(team).IsNotNull();
        await Assert.That(team.Name).IsEqualTo(name);
    }

    [Test]
    public async Task Create_WithNullNameAndAlias_ShouldThrow()
    {
        // Arrange
        string? name = null;

        // Act
        Action act = () => Team.Create(name!);

        // Assert
        await Assert.That(act).ThrowsException()
            .WithMessage("Value cannot be null. (Parameter 'Name')");
    }

    [Test]
    public async Task AddHero_ShouldUpdateTeamPowerLevel()
    {
        // Arrange
        var hero1 = Hero.Create("hero1", "alias1");
        var hero2 = Hero.Create("hero2", "alias2");
        var power1 = new Power("Foo", 10);
        var power2 = new Power("Bar", 4);
        hero1.UpdatePowers([power1]);
        hero2.UpdatePowers([power2]);
        var team = Team.Create("name");

        // Act
        team.AddHero(hero1);
        team.AddHero(hero2);

        // Assert
        await Assert.That(team.TotalPowerLevel).IsEqualTo(14);
    }

    [Test]
    public async Task RemoveHero_ShouldUpdateTeamPowerLevel()
    {
        // Arrange
        var hero1 = Hero.Create("hero1", "alias1");
        var hero2 = Hero.Create("hero2", "alias2");
        var power1 = new Power("Foo", 10);
        var power2 = new Power("Bar", 4);
        hero1.UpdatePowers([power1]);
        hero2.UpdatePowers([power2]);
        var team = Team.Create("name");
        team.AddHero(hero1);
        team.AddHero(hero2);

        // Act
        team.RemoveHero(hero1);

        // Assert
        await Assert.That(team.TotalPowerLevel).IsEqualTo(4);
    }

    [Test]
    public async Task ExecuteMission_ShouldUpdateTeamStatus()
    {
        // Arrange
        var team = Team.Create("name");
        team.AddHero(Hero.Create("hero1", "alias1"));

        // Act
        team.ExecuteMission("Mission");

        // Assert
        await Assert.That(team.Status).IsEqualTo(TeamStatus.OnMission);
        await Assert.That(team.Missions).HasCount().EqualTo(1);
        await Assert.That(team.Missions.Any(x => x.Description == "Mission")).IsTrue();
    }

    [Test]
    public async Task ExecuteMission_WhenTeamNotAvailable_ShouldError()
    {
        // Arrange
        var team = Team.Create("name");
        team.AddHero(Hero.Create("hero1", "alias1"));
        team.ExecuteMission("Mission1");

        // Act
        var result = team.ExecuteMission("Mission2");

        // Assert
        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError).IsEqualTo(TeamErrors.NotAvailable);
    }

    [Test]
    public async Task CompleteCurrentMission_ShouldUpdateTeamStatus()
    {
        // Arrange
        var team = Team.Create("name");
        team.ExecuteMission("Mission");

        // Act
        team.CompleteCurrentMission();

        // Assert
        await Assert.That(team.Status).IsEqualTo(TeamStatus.Available);
    }

    [Test]
    public async Task CompleteCurrentMission_WhenNoMissionHasBeenExecuted_ShouldThrow()
    {
        // Arrange
        var team = Team.Create("name");

        // Act
        var result = team.CompleteCurrentMission();

        // Assert
        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError).IsEqualTo(TeamErrors.NotOnMission);
    }

    [Test]
    public async Task CompleteCurrentMission_WhenNotOnMission_ShouldError()
    {
        // Arrange
        var team = Team.Create("name");
        team.ExecuteMission("Mission1");
        team.CompleteCurrentMission();

        // Act
        var result = team.CompleteCurrentMission();

        // Assert
        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError).IsEqualTo(TeamErrors.NotOnMission);
    }

    [Test]
    public async Task ExecuteMission_WhenNoHeroes_ShouldError()
    {
        // Arrange
        var team = Team.Create("name");

        // Act
        var result = team.ExecuteMission("Mission");

        // Assert
        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError).IsEqualTo(TeamErrors.NoHeroes);
    }

    [Test]
    public async Task ExecuteMission_AfterAddingHero_ShouldSucceed()
    {
        // Arrange
        var team = Team.Create("name");
        var hero = Hero.Create("hero1", "alias1");
        var power1 = new Power("Foo", 10);
        hero.UpdatePowers([power1]);
        team.AddHero(hero);

        // Act
        var result = team.ExecuteMission("Mission");

        // Assert
        await Assert.That(result.IsError).IsFalse();
    }
}
