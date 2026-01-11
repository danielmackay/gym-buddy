using GymBuddy.Domain.Heroes;

namespace GymBuddy.Api.UnitTests.Features.Heroes;

public class HeroTests
{
    [Test]
    [Arguments("c8ad9974-ca93-44a5-9215-2f4d9e866c7a", "cc3431a8-4a31-4f76-af64-e8198279d7a4", false)]
    [Arguments("c8ad9974-ca93-44a5-9215-2f4d9e866c7a", "c8ad9974-ca93-44a5-9215-2f4d9e866c7a", true)]
    public async Task HeroId_ShouldBeComparable(string stringGuid1, string stringGuid2, bool isEqual)
    {
        // Arrange
        Guid guid1 = Guid.Parse(stringGuid1);
        Guid guid2 = Guid.Parse(stringGuid2);
        HeroId id1 = HeroId.From(guid1);
        HeroId id2 = HeroId.From(guid2);

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
        var alias = "alias";

        // Act
        var hero = Hero.Create(name, alias);

        // Assert
        await Assert.That(hero).IsNotNull();
        await Assert.That(hero.Name).IsEqualTo(name);
        await Assert.That(hero.Alias).IsEqualTo(alias);
    }

    [Test]
    public void Create_WithSameNameAndAlias_ShouldSucceed()
    {
        // Arrange
        var name = "name";
        var alias = "name";

        // Act
        Hero.Create(name, alias);
    }

    [Test]
    [Arguments(null, "alias")]
    [Arguments("name", null)]
    [Arguments(null, null)]
    public async Task Create_WithNullTitleOrAlias_ShouldThrow(string? name, string? alias)
    {
        // Arrange

        // Act
        Hero Act() => Hero.Create(name!, alias!);

        // Assert
        await Assert.That(Act).ThrowsException()
            .WithMessageMatching("*Value cannot be null*");
    }

    [Test]
    public async Task AddPower_ShouldUpdateHeroPowerLevel()
    {
        // Act
        var hero = Hero.Create("name", "alias");
        var powers = new List<Power> { new("Super-strength", 10), new("Super-speed", 5) };
        hero.UpdatePowers(powers);

        // Assert
        await Assert.That(hero.PowerLevel).IsEqualTo(15);
        await Assert.That(hero.Powers).HasCount().EqualTo(2);
    }

    [Test]
    public async Task RemovePower_ShouldUpdateHeroPowerLevel()
    {
        // Act
        var hero = Hero.Create("name", "alias");
        var powers = new List<Power> { new("Super-strength", 10), new("Super-speed", 5) };
        hero.UpdatePowers(powers);

        // Act
        hero.UpdatePowers([new("Super-strength", 5)]);

        // Assert
        await Assert.That(hero.PowerLevel).IsEqualTo(5);
        await Assert.That(hero.Powers).HasCount().EqualTo(1);
    }

    [Test]
    public async Task AddPower_ShouldRaisePowerLevelUpdatedEvent()
    {
        // Act
        var hero = Hero.Create("name", "alias");
        hero.Id = HeroId.From(Guid.NewGuid());
        hero.UpdatePowers([new Power("Super-strength", 10)]);

        // Assert
        var domainEvents = hero.PopDomainEvents();
        await Assert.That(domainEvents).IsNotNull();
        await Assert.That(domainEvents).HasCount().EqualTo(1);
        
        var firstEvent = domainEvents.First();
        await Assert.That(firstEvent).IsTypeOf<PowerLevelUpdatedEvent>();
        
        var powerLevelEvent = (PowerLevelUpdatedEvent)firstEvent;
        await Assert.That(powerLevelEvent.Hero.PowerLevel).IsEqualTo(10);
        await Assert.That(powerLevelEvent.Hero.Id).IsEqualTo(hero.Id);
        await Assert.That(powerLevelEvent.Hero.Name).IsEqualTo(hero.Name);
        
        await Assert.That(hero.Powers.Select(p => p.Name)).Contains("Super-strength");
    }

    [Test]
    public async Task RemovePower_ShouldRaisePowerLevelUpdatedEvent()
    {
        // Act
        var hero = Hero.Create("name", "alias");
        var power = new Power("Super-strength", 10);
        hero.UpdatePowers([power]);

        // Assert
        var domainEvents = hero.PopDomainEvents();
        await Assert.That(domainEvents).IsNotNull();
        await Assert.That(domainEvents).HasCount().EqualTo(1);
        
        var lastEvent = domainEvents.Last();
        await Assert.That(lastEvent).IsTypeOf<PowerLevelUpdatedEvent>();
        
        var powerLevelEvent = (PowerLevelUpdatedEvent)lastEvent;
        await Assert.That(powerLevelEvent.Hero.PowerLevel).IsEqualTo(10);
        
        await Assert.That(hero.Powers).HasCount().EqualTo(1);
    }
}