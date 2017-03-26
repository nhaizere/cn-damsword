using System;
using DamSword.Common;

namespace DamSword.Watch.Vk.Providers
{
    public interface IVkMetaProvider : IService
    {
        VkMetaSnapshot FetchDataSnaphot(string reference, DateTime? until);
    }
}