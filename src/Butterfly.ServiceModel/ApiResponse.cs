namespace Butterfly.ServiceModel
{
    public class ApiResponse
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public Paging Paging { get; set; }
    }
}