using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;
using TestApi.AutoFac;
using TestApi.IntegrationEvents.Events;

using Serilog.Sinks.RabbitMQ.Sinks.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new AutofacModule());
    });


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()   
    .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) => {
        clientConfiguration.Username = "guest"; //"dev-user"
        clientConfiguration.Password = "guest"; //
        clientConfiguration.Exchange = "AdrenalinEventBus";
        clientConfiguration.ExchangeType = "direct";
        clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
        clientConfiguration.RouteKey = "ESLoq";
        clientConfiguration.Port = 5672; //32672        
        clientConfiguration.Hostnames.Add("127.0.0.1"); //"10.168.32.20"



        sinkConfiguration.TextFormatter = new JsonFormatter();
    }).MinimumLevel.Warning()
    .CreateLogger();

var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog(Log.Logger);


builder.Services.AddSingleton<ILoggerFactory>(loggerFactory);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();


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

app.Run();
