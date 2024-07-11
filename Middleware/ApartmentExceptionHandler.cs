using Microsoft.AspNetCore.Diagnostics;
using System.Threading;

namespace Apartment_Marketplace_API.Middleware
{
    public class ApartmentExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellation)
        {
            var error = new { message = exception.Message };
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(error, cancellation);
            return true;
        }
    }
}
