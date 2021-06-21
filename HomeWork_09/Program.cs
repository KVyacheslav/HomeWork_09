using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace HomeWork_09
{
    public class Program
    {
        /// <summary>
        /// Телеграм бот для получения и хранения данных.
        /// </summary>
        private static TelegramBotClient bot;

        private static bool isStarted;      // Запущена работа функций программы?

        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Параметры запуска.</param>
        public static void Main(string[] args)
        {
            Initialize();

            Console.ReadLine();
        }

        /// <summary>
        /// Инициализировать бота.
        /// </summary>
        private static void Initialize()
        {
            var pathToken = "token.txt";
            if (File.Exists(pathToken))
            {
                Console.WriteLine("Файл токена найден.");
                var token = File.ReadAllText(pathToken);
                bot = new TelegramBotClient(token);
                bot.OnMessage += MessageListener;
                Console.WriteLine("Запуск работы...");
                Start();
                Console.WriteLine("Программа работает и ждет дальнейших команд...");
            }
            else
                Console.WriteLine("Файла токена не существует!");
        }

        /// <summary>
        /// Запустить работу программы.
        /// </summary>
        private static void Start()
        {
            bot.StartReceiving();
        }

        /// <summary>
        /// Обработка получения сообщений.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="e">Аргументы сообщения.</param>    
        private static void MessageListener(object sender, MessageEventArgs e)
        {
            var text = String.Empty;

            switch (isStarted)
            {
                case false when e.Message.Text == "/start":
                    InfoStart(e);
                    return;
                case false:
                    InfoTryStart(e);
                    return;
                case true when e.Message.Text == "/stop":
                    InfoStop(e);
                    return;
            }

            text = $"[{DateTime.Now.ToLongTimeString()}] {e.Message.Chat.Username}: " +
                       $"{e.Message.Text ?? e.Message.Caption}";
            Console.WriteLine(text);

            Services.DownloadFilesAsync(bot, e);
            if (e.Message.Text == null) return;

            switch (e.Message.Text.Split(' ')[0])
            {
                case "/printallfiles":
                    Services.PrintAllFiles(bot, e);
                    return;
                case "/download":
                    Services.UploadFile(bot, e);
                    return;
            }

            var messageText = e.Message.Text;

            bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messageText}");
        }

        /// <summary>
        /// Отправка информации о завершении работы бота.
        /// </summary>
        /// <param name="e">Аргументы сообщения.</param>
        private static void InfoStop(MessageEventArgs e)
        {
            isStarted = false;
            bot.SendTextMessageAsync(e.Message.Chat.Id,
                "Прощайте, повелитель. Вы отключили мою работу.\nДля включения, введите /start.");
            Console.WriteLine($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName} отключил работу.");
        }

        /// <summary>
        /// Отправка информации о попытке связаться с не работающим ботом.
        /// </summary>
        /// <param name="e">Аргументы сообщения.</param>
        private static void InfoTryStart(MessageEventArgs e)
        {
            bot.SendTextMessageAsync(e.Message.Chat.Id,
                "Бот не будет работать, пока не введете /start.");
            Console.WriteLine($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName} спамит.");
        }

        /// <summary>
        /// Отправка информации о запуске бота.
        /// </summary>
        /// <param name="e">Аргументы сообщения.</param>
        private static void InfoStart(MessageEventArgs e)
        {
            isStarted = true;
            var text = $"Приветствую Вас, {e.Message.From.FirstName}. Вы запустили мою работу.\n" +
                   $"Для отключения, введите /stop.\n\n" +
                   "Я пока что умею хранить файлы у себя, просто отправь мне, а я скачаю!\n" +
                   "Повторять твои сообщения и знаю команды:\n/start - для начала моей работы.\n" +
                   "/stop - для окончания моей работы.\n/printallfiles - просмотр всех файлов у меня." +
                   "\n/download \"имя файла\" - для скачивания выбранного файла.";
            bot.SendTextMessageAsync(e.Message.Chat.Id, text);

            Console.WriteLine($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName} запустил работу.");
        }
    }
}
