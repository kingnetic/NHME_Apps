using HealthChecks.Uris;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace NHME_Apps.HealthChecks
{
    public class UrlHealthCheck : IHealthCheck
    {
        private readonly UriHealthCheckOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private Uri _url;

        public Uri url
        {
            get { return _url; }
            set { _url = value; }
        }
        /// <summary>
        /// Test Url availability
        /// </summary>
        /// <param name="options"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="urlStr"></param>
        public UrlHealthCheck(IOptions<UriHealthCheckOptions> options, IHttpClientFactory httpClientFactory, string urlStr)
        {
            _options = options.Value;

            _httpClientFactory = httpClientFactory;

            _url = new Uri(urlStr);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_url.ToString());

                var requestMessage = new HttpRequestMessage(HttpMethod.Head, _url);

                var response = await client.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy();
                else
                    return HealthCheckResult.Degraded();

            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, ex.Message);
            }
        }

    }
}
