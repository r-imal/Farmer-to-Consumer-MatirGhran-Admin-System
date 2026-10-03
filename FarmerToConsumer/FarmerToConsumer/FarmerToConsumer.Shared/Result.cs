namespace FarmerToConsumer.Shared
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; } = "";
    }
}
