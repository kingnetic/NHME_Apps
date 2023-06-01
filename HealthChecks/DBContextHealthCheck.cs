using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHME_Apps.HealthChecks
{
    public class DbContextHealthCheck : IHealthCheck
    {
        private readonly DbContext _dbContext;


        public DbContextHealthCheck(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
          CancellationToken cancellationToken = new CancellationToken())
        {
            return await _dbContext.Database.CanConnectAsync(cancellationToken)
                    ? HealthCheckResult.Healthy()
                    : HealthCheckResult.Unhealthy();
        }
    }
}
