
namespace presupuestoAPIEV2UribeInzunza.Response
{
    public class Resp
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Resp()
        {
            Success = false;
        }
    }
}
