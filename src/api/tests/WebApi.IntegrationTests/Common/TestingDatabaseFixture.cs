using Microsoft.Extensions.DependencyInjection;
using GymBuddy.Api.IntegrationTests.Common.Infrastructure.Database;
using GymBuddy.Api.IntegrationTests.Common.Infrastructure.Web;

namespace GymBuddy.Api.IntegrationTests.Common;

/// <summary>
/// Global test infrastructure for integration tests using TUnit's assembly hooks.
/// Initializes and manages the database container lifecycle.
/// </summary>
public static class TestingDatabaseFixture
{
    private static readonly TestDatabase Database = new();
    private static WebApiTestFactory s_factory = null!;
    private static IServiceScopeFactory s_scopeFactory = null!;

    /// <summary>
    /// Global setup for all tests - runs once before any test in the assembly
    /// </summary>
    [Before(Assembly)]
    public static async Task InitializeAsync(AssemblyHookContext _)
    {
        await Database.InitializeAsync();
        s_factory = new WebApiTestFactory(Database.DbConnection);
        s_scopeFactory = s_factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    /// <summary>
    /// Global cleanup for all tests - runs once after all tests in the assembly
    /// </summary>
    [After(Assembly)]
    public static async Task DisposeAsync(AssemblyHookContext _)
    {
        await Database.DisposeAsync();
        await s_factory.DisposeAsync();
    }

    /// <summary>
    /// Resets the database before each test
    /// </summary>
    public static async Task ResetDatabaseAsync()
    {
        await Database.ResetAsync();
    }

    // NOTE: If you need an authenticated client, create a similar method that performs the authentication,
    // adds the appropriate headers and returns the authenticated client
    // For an example of this see https://github.com/SSWConsulting/Northwind365
    public static HttpClient CreateAnonymousClient() => s_factory.CreateClient();

    public static IServiceScope CreateScope() => s_scopeFactory.CreateScope();
}
