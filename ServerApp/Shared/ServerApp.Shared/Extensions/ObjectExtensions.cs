namespace ServerApp.Shared.Extensions;

/// <summary>
/// Extension methods for working with objects, particularly for setting protected properties
/// in domain entities when mapping from EF Core models.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Sets a protected property on an object using reflection.
    /// This is commonly used in repository implementations to map EF Core models
    /// to domain entities with protected setters.
    /// </summary>
    /// <param name="obj">The object whose property will be set</param>
    /// <param name="propertyName">The name of the property to set</param>
    /// <param name="value">The value to set</param>
    public static void SetProtectedProperty(this object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        property?.SetValue(obj, value);
    }
}