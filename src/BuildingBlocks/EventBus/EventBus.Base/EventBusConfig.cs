﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Base
{
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5; // eventbusa qoshulma zamani minimum 5 defe yoxla(birinci veya 5 e qeder problem olma ehtimalina gore) 
        public string DefaultTopicName { get; set; } = "AdrenalinEventBus"; // Default Topic adi bunun altinda queueler
        public string EventBusConnectionString { get; set; } = String.Empty; 
        public string SubscriberClientAppName { get; set; } = String.Empty;
        public string EventNamePrefix { get; set; } = String.Empty;
        public string EventNameSuffix { get; set; } = "IntegrationEvent";
        public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;
        public object Connection { get; set; }

        public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);
        public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);


        /// <summary>
        /// Reading parameters from appsettings json
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>EventBusConfig</returns>
        public EventBusConfig ReadFrom(IConfiguration configuration)
        {
           EventBusConfig eventBus = configuration.GetSection("EventBusConfig").Get<EventBusConfig>();
            return eventBus;
        }


    }

    public enum EventBusType
    {
        RabbitMQ =0,
        AzureServiceBus =1
    }
}
