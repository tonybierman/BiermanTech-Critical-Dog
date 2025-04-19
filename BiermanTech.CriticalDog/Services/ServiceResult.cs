namespace BiermanTech.CriticalDog.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public string AlertType { get; set; }
    }
}
