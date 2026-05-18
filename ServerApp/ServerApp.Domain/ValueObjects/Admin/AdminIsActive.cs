namespace ServerApp.Domain.ValueObjects.Admin;

public record AdminIsActive
{
    public bool Value { get; }

    public AdminIsActive(bool value)
    {
        Value = value;
    }

    public static implicit operator bool(AdminIsActive isActive) => isActive.Value;

    public static implicit operator AdminIsActive(bool isActive) => new(isActive);
}
