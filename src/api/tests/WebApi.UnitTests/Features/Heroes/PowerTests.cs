using GymBuddy.Domain.Heroes;

namespace GymBuddy.Api.UnitTests.Features.Heroes;

public class PowerTests
{
    [Test]
    public async Task Power_ShouldBeCreatable()
    {
        // Arrange
        var name = "PowerName";
        var powerLevel = 5;

        // Act
        var power = new Power(name, powerLevel);

        // Assert
        await Assert.That(power).IsNotNull();
        await Assert.That(power.Name).IsEqualTo(name);
        await Assert.That(power.PowerLevel).IsEqualTo(powerLevel);
    }

    [Test]
    public async Task Power_ShouldBeComparable()
    {
        // Arrange
        var name = "PowerName";
        var powerLevel = 5;
        var power1 = new Power(name, powerLevel);
        var power2 = new Power(name, powerLevel);

        // Act
        var areEqual = power1 == power2;

        // Assert
        await Assert.That(areEqual).IsTrue();
    }

    [Test]
    [Arguments(-1, true)]
    [Arguments(0, true)]
    [Arguments(1, false)]
    [Arguments(9, false)]
    [Arguments(10, false)]
    [Arguments(11, true)]
    public async Task Power_WithInvalidPowerLevel_ShouldThrow(int powerLevel, bool shouldThrow)
    {
        // Arrange
        var name = "PowerName";

        // Act
        var act = () => new Power(name, powerLevel);

        // Assert
        if (shouldThrow)
        {
            await Assert.That(act).ThrowsExactly<ArgumentOutOfRangeException>();
        }
        else
        {
            await Assert.That(act).ThrowsNothing();
        }
    }
}
