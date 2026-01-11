using FastEndpoints;
using System.Reflection;
using GymBuddy.Api.ArchitectureTests.Common;
using GymBuddy.Domain.Base;
using GymBuddy.Domain.Base.Interfaces;

namespace GymBuddy.Api.ArchitectureTests;

public class DomainTests : TestBase
{
    private static readonly Type AggregateRoot = typeof(AggregateRoot<>);
    private static readonly Type Entity = typeof(Entity<>);
    private static readonly Type DomainEvent = typeof(IEvent);
    private static readonly Type ValueObject = typeof(IValueObject);

    [Test]
    public async Task DomainModel_Should_InheritsBaseClasses()
    {
        // Arrange
        var output = TestContext.Current!.OutputWriter;
        var domainModels = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespaceContaining(DomainAssemblyName)
            .And().DoNotResideInNamespaceContaining("Base")
            .And().DoNotHaveNameMatching(".*Id.*")
            .And().DoNotHaveNameMatching(".*Vogen.*")
            .And().DoNotHaveName("ThrowHelper")
            .And().DoNotHaveNameEndingWith("Spec")
            .And().DoNotHaveNameEndingWith("Errors")
            .And().MeetCustomRule(new IsNotEnumRule());
        var types = domainModels.GetTypes().ToList();
        
        types.Dump(output);

        // Act
        var result = domainModels
            .Should()
            .Inherit(AggregateRoot)
            .Or().Inherit(Entity)
            .Or().ImplementInterface(DomainEvent)
            .Or().ImplementInterface(ValueObject)
            .GetResult();

        // Assert
        await Assert.That(types).IsNotEmpty();
        await result.ShouldBeSuccessful();
    }

    [Test]
    public async Task EntitiesAndAggregates_Should_HavePrivateParameterlessConstructor()
    {
        // Arrange
        var output = TestContext.Current!.OutputWriter;
        var entityTypes = Types
            .InAssembly(DomainAssembly)
            .That()
            .Inherit(Entity)
            .Or()
            .Inherit(AggregateRoot);
        var types = entityTypes.GetTypes().ToList();
        
        types.Dump(output);

        // Act
        var failingTypes = entityTypes
            .GetTypes()
            .Where(t => t != AggregateRoot && !t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            .ToList();

        // Assert
        await Assert.That(types).IsNotEmpty();
        await Assert.That(failingTypes).IsEmpty();
    }
}
