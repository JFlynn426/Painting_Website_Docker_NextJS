namespace ServerApp.Infrastructure.Services;

using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using ServerApp.Domain.Services;

/// <summary>
/// HTML sanitizer implementation using AngleSharp to prevent XSS attacks.
/// Sanitizes HTML content by parsing and re-serializing, which removes scripts and dangerous attributes.
/// </summary>
public class HtmlSanitizer : IHtmlSanitizer
{
    private static readonly HtmlParser Parser = new();

    // Allowed tags list
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "p", "br", "strong", "em", "u", "s", "i", "b",
        "h1", "h2", "h3", "h4", "h5", "h6",
        "ul", "ol", "li",
        "blockquote", "code", "pre",
        "a", "img", "div", "span", "hr",
        "table", "thead", "tbody", "tr", "th", "td"
    };

    // Allowed attributes list
    private static readonly HashSet<string> AllowedAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "href", "target", "rel", "src", "alt", "title",
        "colspan", "rowspan", "style"
    };

    /// <summary>
    /// Sanitizes HTML content by removing potentially dangerous elements and attributes.
    /// </summary>
    /// <param name="html">The HTML content to sanitize.</param>
    /// <returns>The sanitized HTML content.</returns>
    public string Sanitize(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        // Parse the HTML
        var document = Parser.ParseDocument(html);

        // Remove dangerous elements
        RemoveDangerousElements(document.Body);

        // Remove dangerous attributes
        RemoveDangerousAttributes(document.Body);

        // Return the sanitized body inner HTML
        return document.Body.InnerHtml;
    }

    /// <summary>
    /// Recursively removes elements that are not in the allowed list.
    /// </summary>
    private void RemoveDangerousElements(IHtmlElement element)
    {
        // Get child nodes as a list first to avoid modification during iteration
        var childNodes = element.ChildNodes.ToList();

        foreach (var child in childNodes)
        {
            if (child is IHtmlElement childElement)
            {
                // Recursively process children first
                RemoveDangerousElements(childElement);

                // Remove the element if it's not allowed
                if (!AllowedTags.Contains(childElement.TagName))
                {
                    // Move child nodes before removing the element
                    var grandChildren = childElement.ChildNodes.ToList();
                    foreach (var grandChild in grandChildren)
                    {
                        element.AppendChild(grandChild);
                    }

                    childElement.Remove();
                }
            }
        }
    }

    /// <summary>
    /// Recursively removes attributes that are not in the allowed list.
    /// </summary>
    private void RemoveDangerousAttributes(IHtmlElement element)
    {
        // Get attributes as a list first to avoid modification during iteration
        var attributes = element.Attributes?.ToList() ?? new List<IAttr>();

        foreach (var attr in attributes)
        {
            if (!AllowedAttributes.Contains(attr.Name))
            {
                element.RemoveAttribute(attr.Name);
            }
        }

        // Process children
        var childNodes = element.ChildNodes.ToList();
        foreach (var child in childNodes)
        {
            if (child is IHtmlElement childElement)
            {
                RemoveDangerousAttributes(childElement);
            }
        }
    }
}