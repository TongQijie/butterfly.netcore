namespace Butterfly.ServiceModel
{
    public class ApiRequest<T> : ApiRequest
    {
        public T Body { get; set; }
    }
}