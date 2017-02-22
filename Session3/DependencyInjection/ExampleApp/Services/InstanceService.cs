using System;
using Microsoft.Extensions.Logging;

namespace MvcApp.Services
{
    public interface IInstanceService 
    {
        Guid GetInstanceId();    
    }
    public class InstanceService : IInstanceService
    {
        private Guid _instanceId;
        private ILogger<InstanceService> _logger;

        public InstanceService(ILogger<InstanceService> logger)
        {
            _instanceId = Guid.NewGuid();
            _logger = logger;    
            _logger.LogInformation($"InstanceService Constructor Instance Id: {_instanceId}");
        }

        public Guid GetInstanceId()
        {
            _logger.LogInformation($"InstanceService.GetInstanceId: {_instanceId}");
            return _instanceId;
        }
    }
}