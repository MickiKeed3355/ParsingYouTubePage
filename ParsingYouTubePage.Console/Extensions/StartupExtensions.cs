using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParsingYouTubePage.Notion;
using ParsingYouTubePage.Telegram;
using ParsingYouTubePage.Video.Options;

namespace ParsingYouTubePage.Console.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<TelegramBotManager>();
            services.AddScoped<NotionUploader>();
            services.AddScoped<NotionGeter>();
            services.Configure<ProjectOptions>(configuration.GetSection(nameof(ProjectOptions)));


            var token = configuration.GetSection(nameof(ProjectOptions) + ":" + nameof(ProjectOptions.NotionToken)).Value;
            services.AddNotionClient(opt =>
            {
                opt.AuthToken = token;
            });
            return services;
        }
    }
}
