using HealthChecks.System;
using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NHME_Apps.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var connStr = builder.Configuration.GetConnectionString("TestConnectionString"); //?? throw new ArgumentNullException(nameof(connStr));

//builder.Configuration["ConnectionStrings:TestConnectionString"];

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddHttpClient();

builder.Services.AddHealthChecks()
    .AddDiskStorageHealthCheck(setup: delegate (DiskStorageOptions diskStorageOptions)
    {
        diskStorageOptions.AddDrive(@"C:\", minimumFreeMegabytes: 20000);
    }, name: "diskHealthCheck", HealthStatus.Unhealthy, tags: new string[] { "system", "process", "disk" })

    .AddProcessAllocatedMemoryHealthCheck(name: "memoryHealthCheck",
                                          maximumMegabytesAllocated: 100,
                                          tags: new string[] { "system", "process", "memory" })

    .AddCheck(name: "dbHealthCheck", new SqlConnectionHealthCheck(connectionString: connStr),
                                                            failureStatus: HealthStatus.Unhealthy,
                                                            tags: new string[] { "sql" })

    .AddTypeActivatedCheck<DbHealthCheck>(name: "sqlHealthCheck",
                                          failureStatus: HealthStatus.Degraded,
                                          tags: new string[] { "sql" },
                                          args: new object[] { connStr, "Select 1;" })

     .AddTypeActivatedCheck<UrlHealthCheck>(name: "sysAdministrativo",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "web", "url" },
                                            args: new string[] { "http://sysnhme.montespana.com.ni:8086" })

     .AddTypeActivatedCheck<UrlHealthCheck>(name: "sysEjecutivo",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "web", "url" },
                                            args: new string[] { "http://sysnhme.montespana.com.ni:8083" })

     .AddTypeActivatedCheck<UrlHealthCheck>(name: "sysRRHH",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "web", "url" },
                                            args: new string[] { "http://sysnhme.montespana.com.ni:8081" })

     .AddTypeActivatedCheck<UrlHealthCheck>(name: "sysServicios",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "web", "url" },
                                            args: new string[] { "http://sysnhme.montespana.com.ni:8084" })

     .AddTypeActivatedCheck<UrlHealthCheck>(name: "Receta en Línea",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "web", "url" },
                                            args: new string[] { "http://sysnhme.montespana.com.ni:8085" })

     .AddTypeActivatedCheck<UrlHealthCheck>(name: "Formato de Subsidios",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "web", "url" },
                                            args: new string[] { "http://sysnhme.montespana.com.ni:9083" })

     .AddTypeActivatedCheck<ApiHealthCheck>(name: "CIE 11",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "api" },
                                            args: new string[] { "http://localhost:6382/ct11" })

     .AddTypeActivatedCheck<ApiHealthCheck>(name: "Vademecum",
                                            failureStatus: HealthStatus.Degraded,
                                            tags: new string[] { "api" },
                                            args: new string[] { "http://localhost:5000/aemps-medicamentos/index.html" });

//.AddUrlGroup(new Uri("http://www.google.com"), "Google", tags: new string[] { "web", "url" }); //Control

builder.Services.AddHealthChecksUI(setupSettings: setup =>
{
    setup.AddHealthCheckEndpoint(name: "allEndpoint", uri: "/allHealthChecks");
    setup.AddHealthCheckEndpoint(name: "sysEndpoint", uri: "/sysHealthChecks");
    setup.AddHealthCheckEndpoint(name: "dbEndpoint", uri: "/dbHealthChecks");
    setup.AddHealthCheckEndpoint(name: "urlEndpoint", uri: "/urlHealthChecks");
    setup.AddHealthCheckEndpoint(name: "apiEndpoint", uri: "/apiHealthChecks");

    setup.SetHeaderText("NHME - Health Checks Status");
    setup.SetEvaluationTimeInSeconds(10);
    setup.MaximumHistoryEntriesPerEndpoint(60);
    setup.SetApiMaxActiveRequests(1); 

}).AddInMemoryStorage();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();


//.RequireHost($"*:{builder.Configuration["ManagementPort"]}")
#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapHealthChecks("/allHealthChecks", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    endpoints.MapHealthChecks("/sysHealthChecks", new HealthCheckOptions()
    {
        Predicate = (check) => check.Tags.Contains("system"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    endpoints.MapHealthChecks("/dbHealthChecks", new HealthCheckOptions()
    {
        Predicate = (check) => check.Tags.Contains("sql"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    endpoints.MapHealthChecks("/urlHealthChecks", new HealthCheckOptions()
    {
        Predicate = (check) => (check.Tags.Contains("web") || check.Tags.Contains("url")),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    endpoints.MapHealthChecks("/apiHealthChecks", new HealthCheckOptions()
    {
        Predicate = (check) => check.Tags.Contains("api"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

});
#pragma warning restore ASP0014 // Suggest using top level route registrations


app.UseHealthChecksUI(config =>
{
    config.AddCustomStylesheet(@"wwwroot\css\healthcheck-ui.css");
    config.UIPath = "/dashboard";
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();