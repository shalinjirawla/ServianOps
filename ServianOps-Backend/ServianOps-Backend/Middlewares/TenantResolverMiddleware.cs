using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServianOps_Backend.Middlewares
{
    public class TenantResolverMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only enforce for authenticated requests
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var tenantIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "tenant_id");
                var roleClaim = context.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);
                
                if (tenantIdClaim == null || string.IsNullOrWhiteSpace(tenantIdClaim.Value))
                {
                    // SuperAdmin doesn't have a TenantId
                    if (roleClaim != null && roleClaim.Value == "SuperAdmin")
                    {
                        // Allowed
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"error\": \"Invalid Tenant Context. Token is missing tenant_id claim.\"}");
                        return;
                    }
                }
            }

            // Continue down the pipeline
            await _next(context);
        }
    }
}
