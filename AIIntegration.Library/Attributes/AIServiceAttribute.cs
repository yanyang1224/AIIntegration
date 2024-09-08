using AIIntegration.Library.Enums;
using System;

namespace AIIntegration.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AIServiceAttribute : Attribute
    {
        public AIServiceType ServiceType { get; }

        public AIServiceAttribute(AIServiceType serviceType)
        {
            ServiceType = serviceType;
        }
    }
}