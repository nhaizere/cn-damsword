namespace DamSword.Web.DTO
{
    public abstract class ListDataBase : IUnixTimeStampDto
    {
        public long UnixTimeStamp { get; set; }
        public int? Offset { get; set; }
        public int? Count { get; set; }
    }
}