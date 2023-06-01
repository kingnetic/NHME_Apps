using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHME_Apps.HealthChecks
{
    public class UrlHealthCheck_v1 : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _url;
        public string url { get; set; } = string.Empty;
        public UrlHealthCheck_v1(IHttpClientFactory httpClientFactory, string url)
        {
            _httpClientFactory = httpClientFactory;
            _url = url;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using var httpClient = _httpClientFactory.CreateClient("");

            var response = await httpClient.GetAsync(_url);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(new HealthCheckResult(status: HealthStatus.Healthy, description: "The API is up and running."));
            }
            return await Task.FromResult(new HealthCheckResult(status: HealthStatus.Unhealthy, description: "The API is down."));
        }
    }
}
