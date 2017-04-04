using System;
using System.Collections.Generic;
using System.Net;

namespace DamSword.Web.Exceptions
{
    public class RestObjectValidationException : RequestException
    {
        public IEnumerable<string> MemberNames { get; }

        public RestObjectValidationException(IEnumerable<string> memberNames, string message)
            : base(HttpStatusCode.BadRequest, message ?? throw new ArgumentNullException(nameof(message)))
        {
            MemberNames = memberNames ?? throw new ArgumentNullException(nameof(message));
        }
    }
}