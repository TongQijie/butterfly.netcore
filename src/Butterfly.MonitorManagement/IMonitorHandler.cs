using System.Threading.Tasks;
using Butterfly.ServiceModel;

namespace Butterfly.MonitorManagement
{
    public interface IMonitorHandler
    {
        Task<ApiResponse> GetDownloadLinks();
    }
}