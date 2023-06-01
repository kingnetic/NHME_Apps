using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;

namespace NHME_Apps.HealthChecks
{
    public class DbHealthCheck_v1 : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                var builder = WebApplication.CreateBuilder();

                using (SqlConnection sqlconn = new(builder.Configuration.GetConnectionString("NHMEConnectionString")))
                {
                    if (sqlconn.State !=
                        System.Data.ConnectionState.Open)
                        sqlconn.Open();

                    if (sqlconn.State == System.Data.ConnectionState.Open)
                    {
                        sqlconn.Close();
                        return Task.FromResult(
                        HealthCheckResult.Healthy("The database is up and running."));
                    }
                }

                    return Task.FromResult(
                          new HealthCheckResult(
                          context.Registration.FailureStatus, "The database is down."));
                }
                catch (Exception)
                {
                    return Task.FromResult(
                        new HealthCheckResult(
                            context.Registration.FailureStatus, "The database is down."));
                }
            }
        }
    }