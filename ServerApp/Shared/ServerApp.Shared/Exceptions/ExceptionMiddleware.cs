using Microsoft.AspNetCore.Http;
using ServerApp.Shared.Abstractions.Exceptions;

namespace ServerApp.Shared.Exceptions;

internal sealed class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ServerAppException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", ""));
            var json = System.Text.Json.JsonSerializer.Serialize(new { ErrorCode = errorCode, ex.Message });

            await context.Response.WriteAsync(json);
        }
    }

    private static string ToUnderscoreCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var result = new System.Text.StringBuilder();
        bool previousWasUpper = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (char.IsUpper(c))
            {
                if (i > 0 && !previousWasUpper)
                {
                    result.Append('_');
                }
                result.Append(char.ToLower(c));
                previousWasUpper = true;
            }
            else if (char.IsWhiteSpace(c))
            {
                result.Append('_');
                previousWasUpper = false;
            }
            else
            {
                result.Append(char.ToLower(c));
                previousWasUpper = false;
            }
        }

        return result.ToString();
    }
}
