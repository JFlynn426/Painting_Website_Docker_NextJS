namespace ServerApp.Shared.Domain;

using ServerApp.Shared.Exceptions;

public abstract record StringValueObject
{
    public string Value { get; init; } = string.Empty;

    protected StringValueObject()
    {
    }

    protected StringValueObject(string value, int maxLength, bool allowEmpty = false, bool enforceMaxLength = true, int minLength = 0) : this()
    {
        if (!allowEmpty && string.IsNullOrWhiteSpace(value))
        {
            throw StringValueObjectException.CreateEmptyException(GetTypeName());
        }

        var trimmedValue = value.Trim();
        if (minLength > 0 && trimmedValue.Length < minLength)
        {
            throw StringValueObjectException.CreateTooShortException(GetTypeName(), minLength);
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