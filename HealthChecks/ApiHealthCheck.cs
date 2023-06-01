using HealthChecks.Uris;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace NHME_Apps.HealthChecks
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly UriHealthCheckOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly string _urlStr = string.Empty;
        //public string url { get; set; } = string.Empty;

        /// <summary>
        /// Test API availability
        /// </summary>
        /// <param name="options"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="urlStr"></param>
        public ApiHealthCheck(IOptions<UriHealthCheckOptions> options, IHttpClientFactory httpClientFactory, string urlStr)
        {
            _options = options.Value;

            _httpClientFactory = httpClientFactory;

            _urlStr = urlStr;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.GetAsync(_urlStr);
                if (response.IsSuccessStatusCode)
                {
                    return await Task.FromResult(new HealthCheckResult(status: HealthStatus.Healthy, description: "The API is up and running."));
                }
                return await Task.FromResult(new HealthCheckResult(status: HealthStatus.Unhealthy, description: "The API is down."));
            }
        }
    }
}
