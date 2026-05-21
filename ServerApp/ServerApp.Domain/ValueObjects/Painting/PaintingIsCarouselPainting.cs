namespace ServerApp.Domain.ValueObjects.Painting;

public record PaintingIsCarouselPainting
{
    public bool Value { get; }

    public PaintingIsCarouselPainting(bool value)
    {
        Value = value;
    }

    public static implicit operator PaintingIsCarouselPainting(bool isCarouselPainting) => new(isCarouselPainting);

    public static implicit operator bool(PaintingIsCarouselPainting isCarouselPainting) => isCarouselPainting.Value;
}
