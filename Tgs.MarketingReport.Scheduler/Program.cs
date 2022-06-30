using Hangfire;
using Hangfire.Console;
using Hangfire.Redis;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.MarketingReport.Scheduler;
using Tgs.MarketingReport.Scheduler.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack();
        });
    });

builder.Services
    .AddScheduler()
    .AddMarketingReportDailyAggregateClient((options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("Scheduler:MarketingReportDailyAggregateBaseUrl");
    });

builder.Services.AddControllers();

builder.Services
    .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
        sp => new ConfigureNamedOptions<OtlpExporterOptions>(
            Options.DefaultName,
            exporterOptions =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var nodeIp = config.GetValue<string>("NODE_IP");

                if (!string.IsNullOrWhiteSpace(nodeIp))
                    exporterOptions.Endpoint = new Uri($"http://{nodeIp}:4317");

                config.GetSection("Opentelemetry:Tracing").Bind(exporterOptions);
            }))
    .AddOpenTelemetryTracing(traceBuilder => traceBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .AddProfileViewer()
    .Configure<AspNetCoreInstrumentationOptions>(options =>
    {
        options.Filter = httpContext =>
        {
            return !httpContext.Request.Path.HasValue
                   || !httpContext.Request.Path.StartsWithSegments("/swagger")
                   && !httpContext.Request.Path.StartsWithSegments("/healthz")
                   && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                   && !new[] { "html", "ico", "png", "jpg" }.Any(
                       ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    });

builder.Services.AddHangfire((sp, config) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("Redis");

    var options = new RedisStorageOptions
    {
        Prefix = "hangfire:marketingreport:"
    };

    _ = config
        .UseRedisStorage(connectionString, options)
        .UseConsole();
});

builder.Services.AddHangfireServer();

builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions
    {
        Authorization = new[]
        {
            new IgnoreHangfireAuthorizationFilter()
        }
    });

app.MapControllers();
app.MapProfileViewer();
app.MapSwagger();
app.UseSwaggerUI();

app.UseJobRegister();

app.MapHealthChecks("/healthz");

app.Run();