using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using UtilityBotMessage.Services;
using System.Runtime.Remoting.Messaging;
using static System.Net.Mime.MediaTypeNames;

namespace UtilityBotMessage.Controllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;



        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {

            if (message.Text == "/start")
            {
                // Объект, представляющий кноки
                var buttons = new List<InlineKeyboardButton[]>();
                buttons.Add(new[]
                {
                        InlineKeyboardButton.WithCallbackData($" Ввод текста" , $"text"),
                        InlineKeyboardButton.WithCallbackData($" Ввод чисел" , $"number")
                    });

                // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Наш бот имеет две функции:</b> {Environment.NewLine}" +
                    $" - подсчёт количества символов в тексте;{Environment.NewLine} - вычисление суммы чисел, которые вы ему отправляете {Environment.NewLine}(одним сообщением через пробел)."
                    , cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                return;

            }

            switch (_memoryStorage.GetSession(message.Chat.Id).CodeMessage)
            {

                case "text":
                    Console.WriteLine($"Получено сообщение: {message.Text}!  $\"Длина сообщения: {message.Text.Length} знаков\"");
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"В вашем сообщении {message.Text.Length} символов", cancellationToken: ct);
                    break;
                case "number":
                    char[] delimiterChars = { ' ' };
                    string[] words = message.Text.Split(delimiterChars);
                    int sum = 0;
                    foreach (var word in words)
                    {
                        try
                        {
                            Console.WriteLine($"<{word}>");
                            sum += Int32.Parse(word);
                        }
                        catch (FormatException)
                        {
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Получено текстовое сообщение не соответствующее заданному действию!" +
                                                    $" {Environment.NewLine}Повторите ввод или смените операцию!", cancellationToken: ct);
                        }
                    }
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Cумма чисел {sum}!", cancellationToken: ct);
                    break;
                default:
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Получено текстовое сообщение не соответствующее заданному действию!" +
                        $" {Environment.NewLine}Повторите ввод или смените операцию!", cancellationToken: ct);
                    Console.WriteLine($"_memoryStorage.GetSession(_callbackQuery.From.Id).CodeMessage; =  {_memoryStorage.GetSession(message.Chat.Id).CodeMessage}");

                    break;
            }

            /*
                        switch (message.Text)
                        {
                            case "/start":

                                // Объект, представляющий кноки
                                var buttons = new List<InlineKeyboardButton[]>();
                                buttons.Add(new[]
                                {
                                    InlineKeyboardButton.WithCallbackData($" Ввод текста" , $"text"),
                                    InlineKeyboardButton.WithCallbackData($" Ввод чисел" , $"number")
                                });

                                // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Наш бот имеет две функции:</b> {Environment.NewLine}" +
                                    $" - подсчёт количества символов в тексте;{Environment.NewLine} - вычисление суммы чисел, которые вы ему отправляете {Environment.NewLine}(одним сообщением через пробел)."
                                    , cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

                                break;
                            default:
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Получено текстовое сообщение не соответствующее заданному действию!" + 
                                    $" {Environment.NewLine}Повторите ввод или смените операцию!", cancellationToken: ct);
                                    Console.WriteLine($"_memoryStorage.GetSession(_callbackQuery.From.Id).CodeMessage; =  {_memoryStorage.GetSession(message.Chat.Id).CodeMessage}");

                                break;
                        }*/
        }
    }
}