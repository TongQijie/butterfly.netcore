using Guru.ExtensionMethod;
using Butterfly.ServiceModel;
using Guru.Formatter.Abstractions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace Butterfly.MonitorManagement
{
    [DI(typeof(IMonitorHandler), Lifetime = Lifetime.Singleton)]
    public class MonitorHandler : IMonitorHandler
    {
        private readonly IFormatter _Formatter;

        public MonitorHandler(IJsonFormatter jsonFormatter)
        {
            _Formatter = jsonFormatter;
        }

        public async Task<ApiResponse> GetDownloadLinks()
        {
            var downloadLinks = new List<DownloadLink>();

            var value = RedisClient.Current.Get("DownloadLinks.PrisonBreakS05");
            if (value.HasValue())
            {
                downloadLinks.AddRange(await _Formatter.ReadObjectAsync<List<DownloadLink>>(value, Encoding.UTF8));
            }

            return new ApiResponse()
            {
                Succeeded = true,
                Data = downloadLinks,
            };
        }
    }
}