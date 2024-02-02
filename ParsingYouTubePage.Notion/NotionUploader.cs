using Microsoft.Extensions.Options;
using Notion.Client;
using ParsingYouTubePage.Video;
using ParsingYouTubePage.Video.Options;

namespace ParsingYouTubePage.Notion
{
    public class NotionUploader
    {
        private readonly INotionClient _client;
        private readonly ProjectOptions _projectOptions;
        public NotionUploader(
            INotionClient client,
           IOptions<ProjectOptions> projectOptions)
        {
            _client = client;
            _projectOptions = projectOptions.Value;
        }

        public Task<Page> AddRow(VideoInfo videoInfo, string relationId)
        {
            var properties = new Dictionary<string, PropertyValue>()
            {
                 { "Название", CreateTitleProperty(videoInfo.VideoName) },
                 { "Ютубер", CreateRichTextProperty(videoInfo.ChannelName, videoInfo.ChannelUrl) },
                 { "Ссылка", CreateUrlProperty(videoInfo.VideoUrl) },
                 { "Тип видео", CreateMultiSelectProperty() },
                 { "❤️‍🔥 Blog", CreateRelationProperty(relationId) }
            };

            return _client.Pages.CreateAsync(new PagesCreateParameters()
            {
                Parent = new DatabaseParentInput()
                {
                    DatabaseId = _projectOptions.VideoPageId,
                },
                Properties = properties,

            });
        }

        private static TitlePropertyValue CreateTitleProperty(string title) =>
            new TitlePropertyValue()
            {
                Title = new List<RichTextBase>()
                {
                     new RichTextText()
                     {
                         Text = new Text() { Content = title },
                         PlainText = title,
                         Annotations = new Annotations() { IsBold = true },
                     }
                }
            };

        private static RichTextPropertyValue CreateRichTextProperty(string text, string url) =>
            new RichTextPropertyValue()
            {
                RichText = new List<RichTextBase>()
                {
                    new RichTextText()
                    {
                         PlainText = text,
                         Text = new Text() { Content = text, Link = new Link() { Url = url } },
                         Href = url,
                         Type = RichTextType.Text
                    }
                }
            };

        private static UrlPropertyValue CreateUrlProperty(string url) =>
            new UrlPropertyValue()
            {
                Url = url
            };

        private static MultiSelectPropertyValue CreateMultiSelectProperty() =>
            new MultiSelectPropertyValue()
            {
                MultiSelect = new List<SelectOption>()
                {
                    new SelectOption()
                    {
                        Color = Color.Blue,
                        Name = "New"
                    }
                }
            };

        private static RelationPropertyValue CreateRelationProperty(string id) =>
            new RelationPropertyValue()
            {
                Relation = new List<ObjectId>()
                {
                    new ObjectId()
                    {
                        Id = id
                    }
                }
            };


    }
}
