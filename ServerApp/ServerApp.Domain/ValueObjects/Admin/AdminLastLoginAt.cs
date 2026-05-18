namespace ServerApp.Domain.ValueObjects.Admin;

public record AdminLastLoginAt
{
    public DateTime Value { get; }

    public AdminLastLoginAt(DateTime value)
    {
        Value = value;
    }

    public static implicit operator DateTime(AdminLastLoginAt lastLoginAt) => lastLoginAt.Value;

    public static implicit operator AdminLastLoginAt(DateTime value) => new(value);
}
