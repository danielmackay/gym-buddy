using GymBuddy.Domain.Base.Interfaces;

namespace GymBuddy.Domain.Common;

public enum WeightUnit
{
    Kilograms = 1,
    Pounds = 2
}

public record Weight : IValueObject
{
    private decimal _value;

    // Private setters needed for EF Core
    public decimal Value
    {
        get => _value;
        private set
        {
            ThrowIfLessThan(value, 0, nameof(Value));
            _value = Math.Round(value, 2); // Enforce 2 decimal places
        }
    }

    public WeightUnit Unit { get; private set; }

    public Weight(decimal value, WeightUnit unit = WeightUnit.Kilograms)
    {
        Value = value;
        Unit = unit;
    }

    // Private parameterless constructor for EF Core
    private Weight() { }
}