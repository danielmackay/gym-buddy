using GymBuddy.Domain.Common;

namespace GymBuddy.Domain.UnitTests.Common;

public class DurationTests
{
    [Test]
    public async Task Create_WithValidSeconds_ShouldSucceed()
    {
        // Arrange & Act
        var duration = new Duration(90);

        // Assert
        await Assert.That(duration.Seconds).IsEqualTo(90);
    }

    [Test]
    public async Task FromSeconds_ShouldCreateDuration()
    {
        // Arrange & Act
        var duration = Duration.FromSeconds(120);

        // Assert
        await Assert.That(duration.Seconds).IsEqualTo(120);
    }

    [Test]
    public async Task FromMinutes_ShouldConvertToSeconds()
    {
        // Arrange & Act
        var duration = Duration.FromMinutes(2);

        // Assert
        await Assert.That(duration.Seconds).IsEqualTo(120);
    }

    [Test]
    public async Task FromMinutesAndSeconds_ShouldCombineCorrectly()
    {
        // Arrange & Act
        var duration = Duration.FromMinutesAndSeconds(2, 30);

        // Assert
        await Assert.That(duration.Seconds).IsEqualTo(150);
    }

    [Test]
    public async Task TotalMinutes_ShouldCalculateCorrectly()
    {
        // Arrange
        var duration = new Duration(150);

        // Act & Assert
        await Assert.That(duration.TotalMinutes).IsEqualTo(2);
    }

    [Test]
    public async Task RemainingSeconds_ShouldCalculateCorrectly()
    {
        // Arrange
        var duration = new Duration(150);

        // Act & Assert
        await Assert.That(duration.RemainingSeconds).IsEqualTo(30);
    }

    [Test]
    public async Task Create_WithZeroSeconds_ShouldThrow()
    {
        // Arrange, Act & Assert
        await Assert.That(() => new Duration(0))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task Create_WithNegativeSeconds_ShouldThrow()
    {
        // Arrange, Act & Assert
        await Assert.That(() => new Duration(-10))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task Create_WithOneSecond_ShouldSucceed()
    {
        // Arrange & Act
        var duration = new Duration(1);

        // Assert
        await Assert.That(duration.Seconds).IsEqualTo(1);
    }

    [Test]
    public async Task Create_WithLargeValue_ShouldSucceed()
    {
        // Arrange & Act
        var duration = new Duration(3600); // 1 hour

        // Assert
        await Assert.That(duration.Seconds).IsEqualTo(3600);
        await Assert.That(duration.TotalMinutes).IsEqualTo(60);
        await Assert.That(duration.RemainingSeconds).IsEqualTo(0);
    }

    [Test]
    public async Task Equality_WithSameSeconds_ShouldBeEqual()
    {
        // Arrange
        var duration1 = new Duration(60);
        var duration2 = new Duration(60);

        // Act & Assert
        await Assert.That(duration1).IsEqualTo(duration2);
    }

    [Test]
    public async Task Equality_WithDifferentSeconds_ShouldNotBeEqual()
    {
        // Arrange
        var duration1 = new Duration(60);
        var duration2 = new Duration(61);

        // Act & Assert
        await Assert.That(duration1).IsNotEqualTo(duration2);
    }

    [Test]
    public async Task FromMinutesAndSeconds_WithJustSeconds_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var duration = Duration.FromMinutesAndSeconds(0, 45);

        // Assert
        await Assert.That(duration.Seconds).IsEqualTo(45);
        await Assert.That(duration.TotalMinutes).IsEqualTo(0);
        await Assert.That(duration.RemainingSeconds).IsEqualTo(45);
    }
}
