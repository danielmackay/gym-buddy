using System.Text;
using TestResult = NetArchTest.Rules.TestResult;

namespace GymBuddy.Api.ArchitectureTests.Common;

/// <summary>
/// TUnit-compatible assertion helper for NetArchTest TestResult
/// </summary>
public static class TestResultAssertions
{
    /// <summary>
    /// Asserts that the TestResult is successful, otherwise fails with detailed type information
    /// </summary>
    public static async Task ShouldBeSuccessful(this TestResult result, string? because = null)
    {
        if (result.IsSuccessful)
        {
            await Assert.That(true).IsTrue();
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine(because ?? "The architecture test failed.");
        sb.AppendLine("The following types failed the test:");

        foreach (var name in result.FailingTypeNames ?? [])
        {
            sb.AppendLine($"  - {name}");
        }

        await Assert.That(result.IsSuccessful)
            .IsTrue()
            .Because(sb.ToString());
    }
}
