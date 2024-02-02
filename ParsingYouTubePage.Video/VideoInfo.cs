using AngleSharp.Dom;
using ParsingYouTubePage.Video.Extensions;

namespace ParsingYouTubePage.Video
{
    public class VideoInfo
    {
        private const string name = "name";
        private const string author = "author";
        public static string[] Names = new string[] { name, author };
        public static VideoInfo DefaultVideoInfo = new VideoInfo(string.Empty, string.Empty, string.Empty, string.Empty);
        public VideoInfo(string channelUrl, string channelName, string videoName, string videoUrl)
        {
            ChannelUrl = channelUrl;
            ChannelName = channelName;
            VideoName = videoName;
            VideoUrl = videoUrl;
        }
        public static VideoInfo SetData(IEnumerable<IElement> elements)
        {
            var video = elements.Where(x => x.OuterHtml.Contains(name)).FirstOrDefault();

            if (video == null)
                return DefaultVideoInfo;

            var videoName = video.GetContent();

            var videoUrl = video.BaseUri;

            var channel = elements
            .Where(x => x.OuterHtml.Contains(author))
            .Select(x => x.Children).FirstOrDefault();

            if (channel == null)
                return new VideoInfo(string.Empty, string.Empty, videoName, videoUrl);

            var channelUrl = channel
                .Where(x => !string.IsNullOrEmpty(x.GetHref()))
                .Select(x => x.GetHref())
                .FirstOrDefault() ?? string.Empty;

            var channelName = channel
              .Where(x => !string.IsNullOrEmpty(x.GetContent()))
              .Select(x => x.GetContent())
              .FirstOrDefault() ?? string.Empty;

            return new VideoInfo(channelUrl, channelName, videoName, videoUrl);
        }

        public string ChannelUrl { get; set; }
        public string ChannelName { get; set; }
        public string VideoName { get; set; }
        public string VideoUrl { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
