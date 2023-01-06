using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus.Base;
using EventBus.Factory;
using LogAPI.AutoFac;
using Serilog;
using LogAPI.Integrations.EventHandler;
using EventBus.Base.Abstraction;
using LogAPI.Integrations.Events;
using LogAPI;

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


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddScoped<ESLoqIntegrationEventHandler>();
builder.Services.AddScoped(sp => {
    return EventBusFactory.Create(new EventBusConfig().ReadFrom(builder.Configuration), sp);
});

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
