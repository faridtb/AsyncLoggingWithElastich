using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus.Base;
using EventBus.Factory;
using LogAPI.AutoFac;
using Serilog;
using System.Configuration;
using System.Reflection;
using System;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Elasticsearch;
using LogAPI.Integrations.EventHandler;
using EventBus.Base.Abstraction;
using LogAPI.Integrations.Events;
using Elasticsearch.Net;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new AutofacModule());
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ESLoqIntegrationEventHandler>();
builder.Services.AddHttpContextAccessor();


#region Elastic Serilog
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
        optional: true)
    .Build();

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Uri"]))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = $"ferid-elastic"
            })
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

builder.Host.UseSerilog();


#endregion

#region RabbitMQ
builder.Services.AddScoped<ESLoqIntegrationEventHandler>();
builder.Services.AddScoped(sp =>
{
    EventBusConfig config = new EventBusConfig
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "LogAPI",
        EventBusType = EventBusType.RabbitMQ,
        //Connection = new ConnectionFactory()
        //{
        //    HostName = "rabbitmq"
        //}
    };

    return EventBusFactory.Create(config, sp);
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<ESLoqIntegrationEvent, ESLoqIntegrationEventHandler>();

app.Run();
