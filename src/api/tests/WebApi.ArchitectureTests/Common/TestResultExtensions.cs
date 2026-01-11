using TestResult = NetArchTest.Rules.TestResult;

namespace GymBuddy.Api.ArchitectureTests.Common;

public static class TestResultExtensions
{
    public static void DumpFailingTypes(this TestResult result, TextWriter outputWriter)
    {
        if (result.IsSuccessful)
            return;

        outputWriter.WriteLine("Failing Types:");

        foreach (var type in result.FailingTypes)
        {
            if (type.FullName is null)
                continue;

            outputWriter.WriteLine(type.FullName);
        }
    }
}
