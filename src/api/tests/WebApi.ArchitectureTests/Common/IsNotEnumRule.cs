using Mono.Cecil;

namespace GymBuddy.Api.ArchitectureTests.Common;

public class IsNotEnumRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type) => !type.IsEnum;
}