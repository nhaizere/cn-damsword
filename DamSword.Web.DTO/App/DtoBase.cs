namespace DamSword.Web.DTO
{
    public abstract class DtoBase : IDto
    {
        public int ApiVersion { get; set; }
        public string Identifier { get; set; }
    }
}