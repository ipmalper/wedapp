using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;

namespace WebApplication1.Services
{
    public class ClienteLockCleanerService : BackgroundService
    {
        private readonly ILogger<ClienteLockCleanerService> _logger;
        private readonly IConfiguration _configuration;

        public ClienteLockCleanerService(ILogger<ClienteLockCleanerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var connection = new SqlConnection(_configuration.GetConnectionString("ConexionSQLAzure"));

                    var afectados = await connection.ExecuteAsync(@"
                    UPDATE Users
                    SET 
                        LockedByUserName = NULL,
                        LockedAt = NULL,
                        LockExpiresAt = NULL
                    WHERE LockExpiresAt IS NOT NULL AND LockExpiresAt < GETDATE();
                ");

                    if (afectados > 0)
                    {
                        _logger.LogInformation($"Se liberaron {afectados} bloqueos expirados.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al limpiar bloqueos expirados.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }


}
