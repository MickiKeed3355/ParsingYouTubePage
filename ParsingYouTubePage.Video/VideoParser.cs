using AngleSharp;
using AngleSharp.Dom;

namespace ParsingYouTubePage.Video
{
    public class VideoParser
    {
        private static IConfiguration _config = Configuration.Default.WithDefaultLoader();

        private const string Div = "div";
        private const string MainCol = "watch-main-col";
        private const string Script = "SCRIPT";

        public static async Task<IEnumerable<IElement>> GetDataPageYouTube(string link)
        {
            using var document = await BrowsingContext.New(_config).OpenAsync(link);

            return document.QuerySelectorAll(Div)
                .Where(x => x.ClassName == MainCol)
                .SelectMany(x => x.Children)
                .Where(x => VideoInfo.Names.Any(n => x.OuterHtml.Contains(n)) && x.TagName != Script)
                .ToList();
        }
    }
}
