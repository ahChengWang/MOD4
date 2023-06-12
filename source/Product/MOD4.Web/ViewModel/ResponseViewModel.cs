namespace MOD4.Web.ViewModel
{
    public class ResponseViewModel<T>
    {
        public bool IsSuccess { get; set; } = true;

        public T Data { get; set; }

        public string Msg { get; set; }
    }
}
