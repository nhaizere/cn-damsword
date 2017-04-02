using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DamSword.Web.TagHelpers
{
    public class ActiveUrlInfo
    {
        public bool IsActive { get; set; }
    }

    [HtmlTargetElement("a", ParentTag = "li")]
    public class NavigationAnchorTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public NavigationAnchorTagHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var href = output.Attributes["href"]?.Value?.ToString();
            if (href == null)
                return;

            var hrefUri = new Uri(href, UriKind.RelativeOrAbsolute);
            var contextUri = new Uri(_contextAccessor.HttpContext.Request.Path, UriKind.RelativeOrAbsolute);
            if (Uri.Compare(hrefUri, contextUri, UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var result = (ActiveUrlInfo)context.Items["bs-navigation-active"];
                result.IsActive = true;
            }
        }
    }

    [HtmlTargetElement("li", Attributes = "bs-navigation"), RestrictChildren("a")]
    public class NavigationItemTagHelper : TagHelper
    {
        [HtmlAttributeName("bs-navigation")]
        public bool EnableNavigation { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var result = new ActiveUrlInfo();
            context.Items["bs-navigation-active"] = result;
            output.Content.SetHtmlContent(await output.GetChildContentAsync());

            if (!result.IsActive)
                return;

            var cssClass = output.Attributes["class"]?.Value?.ToString() ?? string.Empty;
            output.Attributes.SetAttribute("class", cssClass + " active");
        }
    }
}