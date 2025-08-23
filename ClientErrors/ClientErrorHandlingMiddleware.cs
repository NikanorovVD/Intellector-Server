using Microsoft.AspNetCore.Http;


namespace ClientErrors
{
    public class ClientErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ClientSideException ex)
            {
                switch (ex)
                {
                    case ClientError clientError:
                        await HandleClientException(context, clientError.Error);
                        break;
                    case ValidationError validatonErr:
                        await HandleValidationException(context, validatonErr.Errors);
                        break;
                }
            }
        }

        private async Task HandleClientException(HttpContext context, string errorMessage)
        {
            var response = new ClientErrorDto { StatusCode = 400, Error = errorMessage };
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleValidationException(HttpContext context, IDictionary<string, string> errors)
        {
            var response = new ValidationErrorDto { StatusCode = 400, Errors = errors };
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
