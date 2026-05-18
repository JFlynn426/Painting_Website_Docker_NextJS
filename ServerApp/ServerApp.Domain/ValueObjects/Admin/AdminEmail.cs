namespace ServerApp.Domain.ValueObjects.Admin;

using ServerApp.Shared.Domain;

public record AdminEmail : StringValueObject
{
    public const int MaxLength = 256;

    public AdminEmail() : base()
    {
    }

    public AdminEmail(string value) : base(value, MaxLength)
    {
        if (!value.Contains("@"))
        {
            throw new ArgumentException($"Invalid email format: {value}", nameof(value));
        }
    }

    public static implicit operator AdminEmail(string email) => new(email);
}
