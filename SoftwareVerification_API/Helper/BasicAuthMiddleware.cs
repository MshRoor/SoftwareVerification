using System.Text;

namespace SoftwareVerification_API.Helper;
public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _username = "admin"; // Set your desired username
    private readonly string _password = "Xcel@1234"; // Set your desired password

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var credentials = decodedCredentials.Split(':');

            if (credentials.Length == 2 && credentials[0] == _username && credentials[1] == _password)
            {
                await _next(context);
                return;
            }
        }

        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        context.Response.StatusCode = 401; // Unauthorized
    }
}