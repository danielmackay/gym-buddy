using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using GymBuddy.Api.Common.Interfaces;
using System.Data.Common;

namespace GymBuddy.Api.IntegrationTests.Common.Infrastructure.Web;

/// <summary>
/// Host builder (services, DI and configuration) for integration tests
/// </summary>
public class WebApiTestFactory : WebApplicationFactory<IWebApiMarker>
{
    private readonly DbConnection _dbConnection;

    public WebApiTestFactory(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Redirect application logging to test output using TUnit's TestContext
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddFilter(level => level >= LogLevel.Information);
            // TUnit captures console output automatically, so we can use console logging
            logging.AddConsole();
        });

        builder.UseSetting("ConnectionStrings:AppDb", _dbConnection.ConnectionString);
    }
}
