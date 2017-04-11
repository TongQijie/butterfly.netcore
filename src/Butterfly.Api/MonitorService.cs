using System.Threading.Tasks;
using Butterfly.MonitorManagement;
using Butterfly.ServiceModel;
using Guru.Middleware.RESTfulService;

namespace Butterfly.Api
{    
    [Service("Monitor", Prefix = "Api")]
    public class MonitorService
    {
        private readonly IMonitorHandler _MonitorHandler;

        public MonitorService(IMonitorHandler monitorHandler)
        {
            _MonitorHandler = monitorHandler;
        }

        [Method(Name = "GetDownloadLinks", HttpVerb = HttpVerb.GET, Response = ContentType.Json)]
        public async Task<ApiResponse> GetDownloadLinks()
        {
            return await _MonitorHandler.GetDownloadLinks();
        }
    }
}