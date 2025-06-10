using WebApplication1.Repositorio.Interfaces;

namespace WebApplication1.Helpers
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRefreshTokensRepository refreshTokenRepo)
        {
            string? authHeader = context.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                var refreshToken = refreshTokenRepo.ObtenerPorToken(token);

                if (refreshToken == null || refreshToken.Revoked || refreshToken.ExpiresAt < DateTime.Now)
                {
                    context.Session.Clear(); // Limpia la sesión
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Sesión expirada o token inválido.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
