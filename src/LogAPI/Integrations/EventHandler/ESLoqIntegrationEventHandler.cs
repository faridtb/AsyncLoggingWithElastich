using EventBus.Base.Abstraction;
using LogAPI.Integrations.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;
using System.Text.Json;
using System.Diagnostics.Eventing.Reader;

namespace LogAPI.Integrations.EventHandler
{
    public class ESLoqIntegrationEventHandler : IIntegrationEventHandler<ESLoqIntegrationEvent>
    {
        private ILogger<ESLoqIntegrationEventHandler> _logger;

        public ESLoqIntegrationEventHandler(Microsoft.Extensions.Logging.ILogger<ESLoqIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ESLoqIntegrationEvent @event)
        {
            var data = (JObject)JsonConvert.DeserializeObject(@event.Payload);
            switch (data["Level"].Value<string>() ?? "Debug")
            {
                case "Trace":
                    _logger.LogTrace("TRACELI");
                    break;
                case "Debug":
                    _logger.LogDebug("DEBUGLU");
                    break;
                case "Information":
                    _logger.LogInformation("INFOLADIM");
                    break;
                case "Warning":
                    _logger.LogWarning("WARNINGLEDIM!");
                    break;
                case "Fatal":
                    _logger.LogCritical("FAATALITY!");
                    break;
                case "Error":
                    _logger.LogError("ERRORLADIM!");
                    break;
            }

            return Task.CompletedTask;
        }

        
    }
}
