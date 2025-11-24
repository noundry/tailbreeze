using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Tailbreeze.TagHelpers;

/// <summary>
/// Tag helper for adding Tailwind CSS link tag with automatic path resolution.
/// Usage: &lt;tailwind-link /&gt;
/// </summary>
[HtmlTargetElement("tailwind-link")]
public sealed class TailwindLinkTagHelper : TagHelper
{
    private readonly IOptions<TailbreezeOptions> _options;
    private readonly IHostEnvironment _environment;

    public TailwindLinkTagHelper(IOptions<TailbreezeOptions> options, IHostEnvironment environment)
    {
        _options = options;
        _environment = environment;
    }

    /// <summary>
    /// Gets or sets whether to add a CDN fallback script.
    /// </summary>
    [HtmlAttributeName("cdn-fallback")]
    public bool CdnFallback { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var opts = _options.Value;

        output.TagName = "link";
        output.TagMode = TagMode.SelfClosing;

        output.Attributes.SetAttribute("rel", "stylesheet");
        output.Attributes.SetAttribute("href", opts.ServePath);

        if (_environment.IsDevelopment())
        {
            output.Attributes.SetAttribute("data-tailbreeze", "true");
        }

        if (CdnFallback)
        {
            var version = TailwindVersion.Parse(opts.TailwindVersion);
            var cdnUrl = version.IsV4
                ? "https://cdn.tailwindcss.com/4.0.0-alpha.32"
                : "https://cdn.tailwindcss.com";

            output.PostElement.AppendHtml($@"
<script>
  // Tailbreeze CDN fallback
  (function() {{
    var link = document.querySelector('link[data-tailbreeze]');
    if (link) {{
      link.onerror = function() {{
        console.warn('Tailbreeze: Loading Tailwind CSS from CDN fallback');
        var cdn = document.createElement('script');
        cdn.src = '{cdnUrl}';
        document.head.appendChild(cdn);
      }};
    }}
  }})();
</script>");
        }
    }
}
