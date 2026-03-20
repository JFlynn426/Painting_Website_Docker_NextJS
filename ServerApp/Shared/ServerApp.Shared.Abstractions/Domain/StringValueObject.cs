namespace ServerApp.Shared.Abstractions.Domain;

using ServerApp.Shared.Abstractions.Exceptions;

public abstract record StringValueObject
{
    public string Value { get; init; } = string.Empty;

    protected StringValueObject()
    {
    }

    protected StringValueObject(string value, int maxLength, bool allowEmpty = false, bool enforceMaxLength = true) : this()
    {
        if (!allowEmpty && string.IsNullOrWhiteSpace(value))
        {
            throw StringValueObjectException.CreateEmptyException(GetTypeName());
        }

        if (enforceMaxLength && value.Length > maxLength)
        {
            throw StringValueObjectException.CreateTooLongException(GetTypeName(), maxLength);
        }

        Value = value;
    }

    protected virtual string GetTypeName() => GetType().Name;

    public static implicit operator string(StringValueObject valueObject) => valueObject.Value;
}