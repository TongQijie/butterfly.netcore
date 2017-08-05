using System.Threading.Tasks;
using Butterfly.MonitorManagement;
using Butterfly.ServiceModel;
using Guru.AspNetCore.Attributes;

namespace Butterfly.Api
{
    [ApiService("Monitor")]
    public class MonitorService
    {
        private readonly IMonitorHandler _MonitorHandler;

        public MonitorService(IMonitorHandler monitorHandler)
        {
            _MonitorHandler = monitorHandler;
        }

        [ApiMethod("GetDownloadLinks")]
        public async Task<ApiResponse> GetDownloadLinks()
        {
            return await _MonitorHandler.GetDownloadLinks();
        }
    }
}