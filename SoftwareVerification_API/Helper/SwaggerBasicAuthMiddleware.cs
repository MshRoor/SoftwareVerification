using System.Text;

namespace SoftwareVerification_API.Helper
{
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                var authHeader = context.Request.Headers["Authorization"];

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.ToString().StartsWith("Basic "))
                {
                    context.Response.Headers["WWW-Authenticate"] = "Basic";
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var encodedCredentials = authHeader.ToString().Substring("Basic ".Length).Trim();
                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var parts = decodedCredentials.Split(':', 2);

                var username = parts[0];
                var password = parts.Length > 1 ? parts[1] : "";

                var expectedUsername = _configuration["SwaggerAuth:Username"];
                var expectedPassword = _configuration["SwaggerAuth:Password"];

                if (username != expectedUsername || password != expectedPassword)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }

            await _next(context);
        }
    }
}
