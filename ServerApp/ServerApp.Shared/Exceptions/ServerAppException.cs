namespace ServerApp.Shared.Exceptions
{
    public class ServerAppException : Exception
    {
        protected ServerAppException(string message) : base(message)
        {
        }
    }
}