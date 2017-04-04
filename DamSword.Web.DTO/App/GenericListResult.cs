using System.Collections.Generic;

namespace DamSword.Web.DTO
{
    public class GenericListResult<TItem> : ListDataBase
    {
        public IEnumerable<TItem> Items { get; set; }

        public GenericListResult()
        {
        }

        public GenericListResult(IEnumerable<TItem> items)
        {
            Items = items;
        }
    }
}