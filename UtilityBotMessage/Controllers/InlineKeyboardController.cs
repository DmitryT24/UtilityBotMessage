using System.Threading.Tasks;
using System.Threading;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UtilityBotMessage.Services;

namespace UtilityBotMessage.Controllers
{
    public class InlineKeyboardController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;

        public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            if (callbackQuery?.Data == null)
                return;

            // Обновление пользовательской сессии новыми данными
            _memoryStorage.GetSession(callbackQuery.From.Id).CodeMessage = callbackQuery.Data;

            // Генерим информационное сообщение
            string optionOperations = callbackQuery.Data switch
            {
                "text" => " Ввод текста",
                "number" => " Ввод чисел",
                _ => String.Empty
            };
             
            // Отправляем в ответ уведомление о выборе
            await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $"<b>Вариант действия - {optionOperations}.{Environment.NewLine}</b>" +
                $"{Environment.NewLine}Для смены перейдите в главное меню.", cancellationToken: ct, parseMode: ParseMode.Html);
        }
    }
}