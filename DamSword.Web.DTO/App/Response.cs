namespace DamSword.Web.DTO
{
    public class Response<TData> : EmptyRespone
    {
        public TData Data { get; set; }

        public Response()
            : base()
        {
        }

        public Response(IRequest request, TData data, bool isSuccessful = true)
            : base(request, isSuccessful)
        {
            Data = data;
        }
    }
}