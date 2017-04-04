using System;

namespace DamSword.Web.DTO
{
    public class EmptyRespone : DtoBase, IResponse, IUnixTimeStampDto
    {
        public bool IsSuccessful { get; set; }
        public long UnixTimeStamp { get; set; }

        public EmptyRespone()
        {
            this.SetTimeStamp(DateTime.UtcNow);
        }

        public EmptyRespone(IRequest request, bool isSuccessful = true)
            : this()
        {
            ApiVersion = 1;
            Identifier = request.Identifier;
            IsSuccessful = isSuccessful;
        }
    }
}