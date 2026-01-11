namespace GymBuddy.Api.ArchitectureTests.Common;

public static class TypeExtensions
{
    public static void Dump(this IEnumerable<Type> types, TextWriter outputWriter)
    {
        if (!types.Any())
            outputWriter.WriteLine("No types found.");

        foreach (var type in types)
        {
            if (type.FullName is null)
                continue;

            outputWriter.WriteLine(type.FullName);
        }
    }
}
