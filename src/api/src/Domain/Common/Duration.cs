using GymBuddy.Domain.Base.Interfaces;

namespace GymBuddy.Domain.Common;

public record Duration : IValueObject
{
    private int _seconds;

    // Private setters needed for EF Core
    public int Seconds
    {
        get => _seconds;
        private set
        {
            ThrowIfLessThan(value, 1, nameof(Seconds));
            _seconds = value;
        }
    }

    public Duration(int seconds)
    {
        Seconds = seconds;
    }

    // Private parameterless constructor for EF Core
    private Duration() { }

    // Convenience factory methods
    public static Duration FromSeconds(int seconds) => new(seconds);
    public static Duration FromMinutes(int minutes) => new(minutes * 60);
    public static Duration FromMinutesAndSeconds(int minutes, int seconds) => new((minutes * 60) + seconds);

    // Convenience properties for display
    public int TotalMinutes => Seconds / 60;
    public int RemainingSeconds => Seconds % 60;
}
