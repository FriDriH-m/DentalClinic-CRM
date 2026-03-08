using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Web_API.Extensions;

namespace Web_API.Middlewares
{
    public class RlsConfigMiddleware
    {
        private readonly RequestDelegate _next;
        public RlsConfigMiddleware(RequestDelegate next) 
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext appDbContext)
        {
            var employeeId = context.GetCurrentEmployeeId();
            var role = context.GetCurrentRole();

            if (employeeId == null || role == null)
            {
                await _next(context);
                return;
            }

            using var transaction = await appDbContext.Database.BeginTransactionAsync();

            try
            {
                await appDbContext.Database.ExecuteSqlRawAsync($"SET ROLE \"{role}\"");
                await appDbContext.Database.ExecuteSqlRawAsync($"SET LOCAL app.current_employee_id = '{employeeId}'");

                await _next(context);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await appDbContext.Database.ExecuteSqlRawAsync("RESET ROLE");
            }
        }
    }
}
