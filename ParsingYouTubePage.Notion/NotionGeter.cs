using Microsoft.Extensions.Options;
using Notion.Client;
using ParsingYouTubePage.Notion.Exceptions;
using ParsingYouTubePage.Video.Options;

namespace ParsingYouTubePage.Notion
{
    public class NotionGeter
    {
        private readonly INotionClient _client;
        private readonly ProjectOptions _projectOptions;

        public NotionGeter(
            INotionClient client,
           IOptions<ProjectOptions> projectOptions)
        {
            _client = client;
            _projectOptions = projectOptions.Value;
        }

        public async Task<string> GetRelationId()
        {
            var filter = GetrCurrentDayFilte();
            var db = await _client.Databases.QueryAsync(_projectOptions.BlogPageId, filter);

            return db.Results.Select(x => x.Id).FirstOrDefault() ?? throw new ExceptionBlog("Необходимо создать день в Blog-е");
        }

        private DatabasesQueryParameters GetrCurrentDayFilte()
        {
            return new DatabasesQueryParameters()
            {
                Filter = new DateFilter("День", equal: DateTime.UtcNow.Date)
            };
        }
    }
}
