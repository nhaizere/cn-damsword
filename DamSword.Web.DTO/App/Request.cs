namespace DamSword.Web.DTO
{
    public class Request<TData> : EmptyRequest
    {
        public TData Data { get; set; }
    }
}