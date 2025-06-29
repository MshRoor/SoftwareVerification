using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SoftwareVerification_API.Helper;
public class HangfireBasicAuthFilter : IDashboardAuthorizationFilter
{
    private readonly IConfiguration _config;
    private readonly string _username;
    private readonly string _password;
    public HangfireBasicAuthFilter(IConfiguration config)
    {
        _config = config;
        _username = _config.GetSection("Username").Value!;
        _password = _config.GetSection("Password").Value!;
    }
    public bool Authorize([NotNull] DashboardContext context)
    {

        var httpContext = context.GetHttpContext();
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();

        // Check if the Authorization header is present and is in Basic format
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            // Trigger a 401 Unauthorized response with Basic authentication challenge
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.StatusCode = 401;
            return false;
        }
        //if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        //    return false;

        var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        var credentials = decodedCredentials.Split(':');

        if (credentials.Length != 2)
            return false;

        if (credentials[0] == _username && credentials[1] == _password)
            return true;

        httpContext.Response.StatusCode = 401;
        return false;
    }
}