namespace ServerApp.Shared.Domain;

public abstract record GuidValueObject
{
    public Guid Value { get; init; }

    protected GuidValueObject()
    {
        Value = Guid.NewGuid();
    }

    protected GuidValueObject(Guid value)
    {
        Value = value;
    }

    public static implicit operator Guid(GuidValueObject valueObject) => valueObject.Value;
}
