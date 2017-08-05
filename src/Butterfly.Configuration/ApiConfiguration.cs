using Guru.DependencyInjection.Attributes;
using Guru.Formatter.Json;

namespace Butterfly.Configuration
{
    [StaticFile(typeof(IApiConfiguration), "./Configuration/api.json", Format = "json")]
    public class ApiConfiguration : IApiConfiguration
    {
        [JsonProperty(Alias = "apiKey")]
        public string ApiKey { get; set; }
    }
}