namespace ServerApp.Domain.ValueObjects.Admin;

public record AdminCreatedAt
{
    public DateTime Value { get; }

    public AdminCreatedAt(DateTime value)
    {
        Value = value;
    }

    public static implicit operator DateTime(AdminCreatedAt createdAt) => createdAt.Value;

    public static implicit operator AdminCreatedAt(DateTime value) => new(value);
}
