namespace ServerApp.Shared.Abstractions.Domain;

using ServerApp.Shared.Abstractions.Exceptions;

public abstract record DecimalValueObject
{
    public decimal Value { get; init; }

    protected DecimalValueObject()
    {
    }

    protected DecimalValueObject(decimal value, bool allowZero = false)
    {
        if (value < 0)
        {
            throw DecimalValueObjectException.CreateNegativeException(GetTypeName(), value);
        }

        if (!allowZero && value == 0)
        {
            throw DecimalValueObjectException.CreateNegativeException(GetTypeName(), value);
        }

        // Check for more than 2 decimal places
        var scaled = value * 100;
        if (scaled != Math.Round(scaled))
        {
            throw DecimalValueObjectException.CreateTooManyDecimalPlacesException(GetTypeName(), value, 2);
        }

        Value = value;
    }

    protected virtual string GetTypeName() => GetType().Name;

    public static implicit operator decimal(DecimalValueObject valueObject) => valueObject.Value;
}