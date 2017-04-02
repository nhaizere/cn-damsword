using System;

namespace DamSword.Web.Models.Error
{
    public class DetailsModel
    {
        public int StatusCode { get; set; }
        public Exception Exception { get; set; }
    }
}