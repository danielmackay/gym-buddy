using FastEndpoints;
using GymBuddy.Api.ArchitectureTests.Common;

namespace GymBuddy.Api.ArchitectureTests;

public class ApplicationTests : TestBase
{
    [Test]
    public async Task CommandHandlers_Should_HaveCorrectSuffix()
    {
        // Arrange
        var output = TestContext.Current!.OutputWriter;
        var commandTypes = Types
            .InAssembly(WebApiAssembly)
            .That()
            .ResideInNamespaceContaining(CommandsAssemblyName)
            .GetTypes()
            .WithNestedTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            ))
            .ToList();
        
        commandTypes.Dump(output);
        
        // Act
        var invalidNames = commandTypes
            .Where(t => t.Name != "Handler")
            .ToList();
        
        // Assert
        await Assert.That(invalidNames).IsEmpty();
    }

    [Test]
    public async Task QueryHandlers_Should_HaveCorrectSuffix()
    {
        // Arrange
        var output = TestContext.Current!.OutputWriter;
        var commandTypes = Types
            .InAssembly(WebApiAssembly)
            .That()
            .ResideInNamespaceContaining(QueriesAssemblyName)
            .GetTypes()
            .WithNestedTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            ))
            .ToList();
        
        commandTypes.Dump(output);
        
        // Act
        var invalidNames = commandTypes
            .Where(t => t.Name != "Handler")
            .ToList();
        
        // Assert
        await Assert.That(invalidNames).IsEmpty();
    }
}
