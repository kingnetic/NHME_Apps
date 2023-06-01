using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace NHME_Apps.HealthChecks
{
    public class EndpointHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken =
         default)
        {

            //create a json string of parameters and send it to the endpoint
            var data = new
            {
                test = "Example",
            };
            string jsonString = JsonSerializer.Serialize(data);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.example.com/post");
            httpWebRequest.ContentType = "application/json"; //= Configuration["application/json"];
            httpWebRequest.Method = "POST"; //= Configuration["POST"];
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
            }
            //Get the endpoint result and use it to return the appropriate health check result
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (((int)httpResponse.StatusCode) >= 200 && ((int)httpResponse.StatusCode) < 300)
                return Task.FromResult(HealthCheckResult.Healthy());
            else
                return Task.FromResult(HealthCheckResult.Unhealthy());
        }
    }
}
