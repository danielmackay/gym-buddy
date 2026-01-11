using GymBuddy.Domain.Common;

namespace GymBuddy.Domain.UnitTests.Common;

public class WeightTests
{
    [Test]
    public async Task Create_WithValidKilograms_ShouldSucceed()
    {
        // Arrange & Act
        var weight = new Weight(75.5m, WeightUnit.Kilograms);

        // Assert
        await Assert.That(weight.Value).IsEqualTo(75.5m);
        await Assert.That(weight.Unit).IsEqualTo(WeightUnit.Kilograms);
    }

    [Test]
    public async Task Create_WithValidPounds_ShouldSucceed()
    {
        // Arrange & Act
        var weight = new Weight(165.0m, WeightUnit.Pounds);

        // Assert
        await Assert.That(weight.Value).IsEqualTo(165.0m);
        await Assert.That(weight.Unit).IsEqualTo(WeightUnit.Pounds);
    }

    [Test]
    public async Task Create_WithDefaultUnit_ShouldDefaultToKilograms()
    {
        // Arrange & Act
        var weight = new Weight(75.5m);

        // Assert
        await Assert.That(weight.Unit).IsEqualTo(WeightUnit.Kilograms);
    }

    [Test]
    public async Task Create_WithNegativeValue_ShouldThrow()
    {
        // Arrange, Act & Assert
        await Assert.That(() => new Weight(-10m, WeightUnit.Kilograms))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task Create_WithTooManyDecimals_ShouldRoundToTwoPlaces()
    {
        // Arrange & Act
        var weight = new Weight(75.567m, WeightUnit.Kilograms);

        // Assert
        await Assert.That(weight.Value).IsEqualTo(75.57m);
    }

    [Test]
    public async Task Create_WithZeroValue_ShouldSucceed()
    {
        // Arrange & Act
        var weight = new Weight(0m, WeightUnit.Kilograms);

        // Assert
        await Assert.That(weight.Value).IsEqualTo(0m);
        await Assert.That(weight.Unit).IsEqualTo(WeightUnit.Kilograms);
    }

    [Test]
    public async Task Create_WithLargeValue_ShouldSucceed()
    {
        // Arrange & Act
        var weight = new Weight(999.99m, WeightUnit.Pounds);

        // Assert
        await Assert.That(weight.Value).IsEqualTo(999.99m);
        await Assert.That(weight.Unit).IsEqualTo(WeightUnit.Pounds);
    }

    [Test]
    public async Task Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var weight1 = new Weight(75.5m, WeightUnit.Kilograms);
        var weight2 = new Weight(75.5m, WeightUnit.Kilograms);

        // Act & Assert
        await Assert.That(weight1).IsEqualTo(weight2);
    }

    [Test]
    public async Task Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var weight1 = new Weight(75.5m, WeightUnit.Kilograms);
        var weight2 = new Weight(75.6m, WeightUnit.Kilograms);

        // Act & Assert
        await Assert.That(weight1).IsNotEqualTo(weight2);
    }

    [Test]
    public async Task Equality_WithDifferentUnits_ShouldNotBeEqual()
    {
        // Arrange
        var weight1 = new Weight(75.5m, WeightUnit.Kilograms);
        var weight2 = new Weight(75.5m, WeightUnit.Pounds);

        // Act & Assert
        await Assert.That(weight1).IsNotEqualTo(weight2);
    }
}
