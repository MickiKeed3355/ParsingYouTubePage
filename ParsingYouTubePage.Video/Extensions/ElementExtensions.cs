using AngleSharp.Dom;

namespace ParsingYouTubePage.Video.Extensions
{
    public static class ElementExtensions
    {
        public static string GetHref(this IElement element) =>
            element.GetAttribute("href") ?? string.Empty;
        public static string GetContent(this IElement element) =>
            element.GetAttribute("content") ?? string.Empty;
    }
}
