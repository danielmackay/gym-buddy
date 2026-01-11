using System.Reflection;
using GymBuddy.Domain.Base;

namespace GymBuddy.Api.ArchitectureTests.Common;

public abstract class TestBase
{
    protected const string DomainAssemblyName = "Domain";
    protected const string CommandsAssemblyName = "Commands";
    protected const string QueriesAssemblyName = "Queries";
    
    protected static readonly Assembly WebApiAssembly = typeof(GymBuddy.Api.Program).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Entity<>).Assembly;
}