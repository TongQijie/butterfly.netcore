namespace Butterfly.ServiceModel
{
    public class ApiRequest
    {
        public KeyValueItem[] Items { get; set; }

        public Paging Paging { get; set; }
    }
}