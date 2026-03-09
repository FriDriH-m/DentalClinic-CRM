using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Web_API.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetCurrentEmployeeId(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Employee-Id", out var headerValue))
            {
                return headerValue.ToString();
            }
            return null;
        }
        public static string GetCurrentRole(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Employee-Role", out var headerValue))
            {
                return headerValue.ToString();
            }
            return null;
        }
    }    
}
