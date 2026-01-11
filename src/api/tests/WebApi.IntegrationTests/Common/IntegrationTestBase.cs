using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GymBuddy.Api.Common.Persistence;

namespace GymBuddy.Api.IntegrationTests.Common;

/// <summary>
/// Integration tests inherit from this to access helper classes.
/// Uses TUnit hooks for setup/teardown.
/// NotInParallel ensures tests run sequentially since they share a database
/// and Respawn resets state before each test.
/// </summary>
[NotInParallel]
public abstract class IntegrationTestBase
{
    private IServiceScope _scope = null!;
    private ApplicationDbContext _dbContext = null!;

    /// <summary>
    /// Setup for each test - resets the database and creates a new scope
    /// </summary>
    [Before(Test)]
    public async Task SetupAsync()
    {
        await TestingDatabaseFixture.ResetDatabaseAsync();
        _scope = TestingDatabaseFixture.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    /// <summary>
    /// Cleanup for each test - disposes the scope
    /// </summary>
    [After(Test)]
    public Task TeardownAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    protected IQueryable<T> GetQueryable<T>() where T : class => _dbContext.Set<T>().AsNoTracking();

    protected async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        await _dbContext.AddAsync(entity, CancellationToken);
        await _dbContext.SaveChangesAsync(CancellationToken);
    }

    protected async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class
    {
        await _dbContext.AddRangeAsync(entities, CancellationToken);
        await _dbContext.SaveChangesAsync(CancellationToken);
    }

    protected async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync(CancellationToken);
    }

    protected HttpClient GetAnonymousClient() => TestingDatabaseFixture.CreateAnonymousClient();

    protected CancellationToken CancellationToken => TestContext.Current!.CancellationToken;
}
