namespace ServerApp.Application.Services;

/// <summary>
/// Provides state storage and validation for OAuth flows.
/// State parameters are stored server-side with expiration to prevent CSRF attacks.
/// </summary>
public interface IStateStore
{
    /// <summary>
    /// Stores an OAuth state parameter with a 10-minute expiration.
    /// </summary>
    /// <param name="state">The state value to store.</param>
    void StoreState(string state);

    /// <summary>
    /// Validates and removes an OAuth state parameter.
    /// Returns true if the state exists and is valid; false otherwise.
    /// The state is removed after validation to prevent replay attacks.
    /// </summary>
    /// <param name="state">The state value to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    bool ValidateAndRemoveState(string state);
}
