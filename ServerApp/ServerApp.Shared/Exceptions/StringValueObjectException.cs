namespace ServerApp.Shared.Exceptions;

public class StringValueObjectException : ServerAppException
{
    protected StringValueObjectException(string valueObjectName, string message) : base(message)
    {
        ValueObjectName = valueObjectName;
    }

    public string ValueObjectName { get; }

    public static StringValueObjectException CreateEmptyException(string valueObjectName)
    {
        return new StringValueObjectException(valueObjectName, $"{valueObjectName} cannot be null or whitespace.");
    }

    public static StringValueObjectException CreateTooLongException(string valueObjectName, int maxLength)
    {
        return new StringValueObjectException(valueObjectName, $"{valueObjectName} cannot exceed {maxLength} characters.");
    }
}