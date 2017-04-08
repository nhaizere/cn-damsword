using System;
using DamSword.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DamSword.Web
{
    public static class ControllerExtensions
    {
        public static void Require(this Controller self, UserPermissions permissions)
        {
            if (!CurrentPermissions.Has(permissions))
                throw new UnauthorizedAccessException();
        }
    }
}