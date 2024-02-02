using Microsoft.Extensions.Options;
using ParsingYouTubePage.Notion;
using ParsingYouTubePage.Notion.Exceptions;
using ParsingYouTubePage.Video;
using ParsingYouTubePage.Video.Options;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ParsingYouTubePage.Telegram
{
    public class TelegramBotManager
    {
        private readonly ProjectOptions _projectOptions;
        private readonly NotionUploader _notionUploader;
        private readonly NotionGeter _notionGeter;
        public TelegramBotManager(
            IOptions<ProjectOptions> projectOptions,
            NotionUploader notionUploader,
            NotionGeter notionGeter)
        {
            _projectOptions = projectOptions.Value;
            _notionUploader = notionUploader;
            _notionGeter = notionGeter;
        }

        public async Task Start()
        {
            var botClient = new TelegramBotClient(_projectOptions.TelegramToken);

            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            var relationId = _notionGeter.GetRelationId();

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token);

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();

            Console.ReadKey();

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                if (!TryGetChatId(update, out var chatId))
                    return;

                try
                {
                    var address = update?.Message?.Text;

                    if (!Uri.IsWellFormedUriString(address, UriKind.RelativeOrAbsolute))
                    {
                        await botClient.SendTextMessageAsync(chatId, "Введенный текст не является ссылкой");

                        return;
                    }

                    if (!address.Contains("youtube"))
                    {
                        await botClient.SendTextMessageAsync(chatId, "Введенный текст не является ссылкой на ютуб");

                        return;
                    }

                    var cells = await VideoParser.GetDataPageYouTube(address);

                    var videoInfo = VideoInfo.SetData(cells);

                    var result = await _notionUploader.AddRow(videoInfo, await relationId);

                    await botClient.SendTextMessageAsync(chatId, "Запись добавлена");

                    return;
                }
                catch (ExceptionBlog exB)
                {
                    await botClient.SendTextMessageAsync(chatId, exB.Message);
                }
                catch (Exception ex)
                {
                    // ignore
                }
            }

            async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };
                Console.WriteLine(ErrorMessage);
                return;
            }
        }

        private bool TryGetChatId(Update update, [NotNullWhen(true)] out long? chatId)
        {
            chatId = update?.Message?.Chat?.Id ?? update?.CallbackQuery?.From?.Id;
            return chatId.HasValue;
        }
    }
}