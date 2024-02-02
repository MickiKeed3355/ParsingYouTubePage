using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParsingYouTubePage.Console.Extensions;
using ParsingYouTubePage.Telegram;

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddCommonServices(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

var telegramBotManager = serviceProvider.GetRequiredService<TelegramBotManager>();
await telegramBotManager.Start();