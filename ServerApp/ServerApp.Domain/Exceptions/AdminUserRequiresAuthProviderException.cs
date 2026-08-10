namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Exceptions;

public class AdminUserRequiresAuthProviderException : ServerAppException
{
    public AdminUserRequiresAuthProviderException()
        : base("At least one OAuth provider identifier (Google or Yahoo) must be provided when creating an admin user.")
    {
    }
}
