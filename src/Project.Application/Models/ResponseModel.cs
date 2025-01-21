namespace Project.Application.Models
{
    public class ResponseModel<T>
    {
        public T Data { get; set; }
        public bool Status { get; set; }

        public ResponseModel(T data, bool status)
        {
            Data = data;
            Status = status;
        }
    }
}
