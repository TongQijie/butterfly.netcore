using Guru.Formatter.Json;
using Guru.DependencyInjection;

namespace Butterfly.ServiceModel
{
    [FileDI(typeof(Article), "./data/*.json", Format = FileFormat.Json, Multiply = true)]
    public class Article
    {
        [JsonProperty(Alias = "id")]
        public string Id { get; set; }

        [JsonProperty(Alias = "creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty(Alias = "modifiedDate")]
        public string ModifiedDate { get; set; }

        [JsonProperty(Alias = "title")]
        public string Title { get; set; }

        [JsonProperty(Alias = "abstract")]
        public string Abstract { get; set; }

        [JsonProperty(Alias = "content")]
        public string Content { get; set; }

        [JsonProperty(Alias = "signature")]
        public string Signature { get; set; }
    }
}