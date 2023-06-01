using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHME_Apps.HealthChecks
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _testQuery;

        private const string DefaultTestQuery = "Select 1";

        public string? ConnectionString { get; }

        public string? TestQuery { get; }

        public DbHealthCheck(string connectionString, string testQuery)
        {
            _connectionString = connectionString;
            _testQuery = testQuery ?? DefaultTestQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync(cancellationToken);

                var cmd = connection.CreateCommand();
                cmd.CommandText = _testQuery;
                await cmd.ExecuteNonQueryAsync(cancellationToken);

                return HealthCheckResult.Healthy("The database is up and running.");
            }
            catch (Exception ex)
            {
                //return new HealthCheckResult(HealthStatus.Unhealthy, ex.Message);
                return new HealthCheckResult(context.Registration.FailureStatus, "The database is down.");
            }
        }
    }
}
