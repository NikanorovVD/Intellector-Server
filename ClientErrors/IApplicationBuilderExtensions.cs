using Microsoft.AspNetCore.Builder;

namespace ClientErrors
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseClientErrors(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ClientErrorHandlingMiddleware>();
        }
    }
}
