namespace ServerApp.Application.Exceptions;

using ServerApp.Shared.Abstractions.Exceptions;

public class PaintingSlugAlreadyExistsException : ServerAppException
{
    public PaintingSlugAlreadyExistsException()
        : base("A painting with a similar title already exists. Please use a more unique title.")
    {
    }
}