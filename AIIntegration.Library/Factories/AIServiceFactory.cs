using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using AIIntegration.Library.Enums;
using AIIntegration.Library.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace AIIntegration.Library.Factories
{
    public class AIServiceFactory
    {
        private readonly AIServiceConfig _config;

        public AIServiceFactory(AIServiceConfig config)
        {
            _config = config;
        }

        public IAIService CreateService(AIServiceType serviceType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var serviceTypeInfo = assembly.GetTypes()
                .FirstOrDefault(t => t.GetCustomAttribute<AIServiceAttribute>()?.ServiceType == serviceType);

            if (serviceTypeInfo == null)
            {
                throw new ArgumentException($"No service found for type: {serviceType}");
            }

            return (IAIService)Activator.CreateInstance(serviceTypeInfo, _config);
        }
    }
}