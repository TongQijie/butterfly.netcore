using Guru.DependencyInjection;

namespace Butterfly.Configuration
{
    [FileDI(typeof(IApiConfiguration), "./configuration/api.json", Format = FileFormat.Json)]
    public class ApiConfiguration : IApiConfiguration
    {
        public string ApiKey { get; set; }
    }
}