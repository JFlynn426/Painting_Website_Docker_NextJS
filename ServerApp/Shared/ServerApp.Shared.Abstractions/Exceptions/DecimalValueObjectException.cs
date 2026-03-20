namespace ServerApp.Shared.Abstractions.Exceptions;

public class DecimalValueObjectException : ServerAppException
{
    public DecimalValueObjectException(string message) : base(message)
    {
    }

    public static DecimalValueObjectException CreateNegativeException(string typeName, decimal value)
    {
        return new DecimalValueObjectException($"The {typeName} cannot be negative. Provided: {value}");
    }

    public static DecimalValueObjectException CreateTooManyDecimalPlacesException(string typeName, decimal value, int maxDecimalPlaces)
    {
        return new DecimalValueObjectException($"The {typeName} cannot have more than {maxDecimalPlaces} decimal places. Provided: {value}");
    }
}