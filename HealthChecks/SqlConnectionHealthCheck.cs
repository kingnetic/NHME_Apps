using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.Common;

namespace NHME_Apps.HealthChecks
{
    // Sample SQL Connection Health Check
    public class SqlConnectionHealthCheck : IHealthCheck
    {
        private const string DefaultTestQuery = "Select 1";
        private HealthStatus _unhealthyStatus = HealthStatus.Unhealthy;

        public string ConnectionString { get; }

        public string? TestQuery { get; }

        public HealthStatus healthStatus
        {
            get { return _unhealthyStatus; }
            set { _unhealthyStatus = value; }
        }

        /// <summary>
        /// Testing DB connection passing the connectionstring and health status
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="unhealthyStatus"></param>
        public SqlConnectionHealthCheck(string connectionString, HealthStatus unhealthyStatus = HealthStatus.Unhealthy)
            : this(connectionString: connectionString, testQuery: DefaultTestQuery)
        {
            _unhealthyStatus = unhealthyStatus;
        }

        /// <summary>
        /// Testing DB connection passing the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlConnectionHealthCheck(string connectionString)
            : this(connectionString: connectionString, testQuery: DefaultTestQuery)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Testing DB connection passing the connectionstring and a test query
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="testQuery"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlConnectionHealthCheck(string connectionString, string testQuery)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            TestQuery = testQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    if (TestQuery != null)
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = TestQuery;

                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (DbException ex)
                {
                    return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
                }
            }

            return HealthCheckResult.Healthy();
        }
    }
}
