using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace HomeWork_09
{
    public static class Services
    {
        private static TelegramBotClient bot;   // Телеграм бот.
        private static int indexVoice;          // Используется для имени голосвых сообщений.
        private static int indexPhoto;          // Используется для имени фото.
        private static string dirName;          // Имя каталога, где будут храниться файлы.
        private static StringBuilder dirsAndFiles = new();  // Папки и файлы, хранящиейся на сервере.
        private static int countFiles;          // Количество хранящихся файлов.
        // private static Dictionary<string, string> files = new();

        #region Download Files

        /// <summary>
        /// Загрузка файлов.
        /// </summary>
        /// <param name="args">Аргументы сообщения.</param>
        public static async void DownloadFilesAsync(TelegramBotClient client, MessageEventArgs args)
        {
            bot = client;
            dirName = @"files\";
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            switch (args.Message.Type)
            {
                case MessageType.Text:
                    break;
                case MessageType.Photo:
                    await DownloadPhotoAsync(args);
                    break;
                case MessageType.Audio:
                    await DownloadAudioAsync(args);
                    break;
                case MessageType.Video:
                    await DownloadVideoAsync(args);
                    break;
                case MessageType.Voice:
                    await DownloadVoiceAsync(args);
                    break;
                case MessageType.Document:
                    await DownloadDocumentAsync(args);
                    break;
                default:
                    Console.WriteLine("Такой тип файла невозможно загрузить.");
                    break;
            }
        }

        /// <summary>
        /// Загрузить фото.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        /// <returns></returns>
        private static async Task DownloadPhotoAsync(MessageEventArgs args)
        {
            dirName += @"Photo\";
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            else
                indexPhoto = Directory.GetFiles(dirName).Length;

            Console.WriteLine("Получено фото:");
            var photos = args.Message.Photo;
            Console.WriteLine($"Размер фото: {photos[^1].FileSize / 1024:N0} кб.");
            var file = await bot.GetFileAsync(photos[^1].FileId);
            var fs = new FileStream($"{dirName}photo_{++indexPhoto:0000}.jpg", FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            await fs.DisposeAsync();
            int countFiles = Directory.GetFiles(dirName).Length;
            bot.SendTextMessageAsync(args.Message.Chat.Id, $"Количество фото в хранилище: {countFiles}.");
        }

        /// <summary>
        /// Скачать аудио.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task DownloadAudioAsync(MessageEventArgs args)
        {
            dirName += @"Audio\";
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            Console.WriteLine("Получено аудио:");
            Console.WriteLine($"Название аудио: {args.Message.Audio.FileName}");
            Console.WriteLine($"Длина аудио: {args.Message.Audio.Duration} сек.");
            Console.WriteLine($"Размер аудио: {args.Message.Audio.FileSize / 1024:N0} кб.");
            var file = await bot.GetFileAsync(args.Message.Audio.FileId);
            var fs = new FileStream($"{dirName}audio_{args.Message.Audio.FileName}", FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            await fs.DisposeAsync();
            int countFiles = Directory.GetFiles(dirName).Length;
            bot.SendTextMessageAsync(args.Message.Chat.Id, $"Количество аудио в хранилище: {countFiles}.");
        }

        /// <summary>
        /// Скачать видео.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        /// <returns></returns>
        private static async Task DownloadVideoAsync(MessageEventArgs args)
        {
            dirName += @"Video\";
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            Console.WriteLine("Получено видео:");
            Console.WriteLine($"Название видео: {args.Message.Video.FileName}");
            Console.WriteLine($"Размер видео: {args.Message.Video.Width} х {args.Message.Video.Height}");
            Console.WriteLine($"Длина видео: {args.Message.Video.Duration} сек.");
            Console.WriteLine($"Размер видео: {args.Message.Video.FileSize / 1024:N0} кб.");
            var file = await bot.GetFileAsync(args.Message.Video.FileId);
            var fs = new FileStream($"{dirName}video_{args.Message.Video.FileName}", FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            await fs.DisposeAsync();
            int countFiles = Directory.GetFiles(dirName).Length;
            bot.SendTextMessageAsync(args.Message.Chat.Id, $"Количество видео в хранилище: {countFiles}.");
        }

        /// <summary>
        /// Скачать голосовое сообщение.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        /// <returns></returns>
        private static async Task DownloadVoiceAsync(MessageEventArgs args)
        {
            dirName += @"Voice\";
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            else
                indexVoice = Directory.GetFiles(dirName).Length;

            Console.WriteLine("Получено голосовое сообщение:");
            Console.WriteLine($"Длина сообщения: {args.Message.Voice.Duration} сек.");
            Console.WriteLine($"Размер сообщения: {args.Message.Voice.FileSize:N0} байт.");
            var file = await bot.GetFileAsync(args.Message.Voice.FileId);
            var fs = new FileStream($"{dirName}voice_{++indexVoice:0000}.ogg", FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            await fs.DisposeAsync();
            int countFiles = Directory.GetFiles(dirName).Length;
            bot.SendTextMessageAsync(args.Message.Chat.Id, $"Количество голосовых сообщений " +
                $"в хранилище: {countFiles}.");
        }

        /// <summary>
        /// Скачать документ.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        /// <returns></returns>
        private static async Task DownloadDocumentAsync(MessageEventArgs args)
        {
            dirName += @"Documents\";
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            Console.WriteLine("Получен документ:");
            Console.WriteLine($"Название документа: {args.Message.Document.FileName}.");
            Console.WriteLine($"Размер документа: {args.Message.Document.FileSize / 1024:N0} кб.");
            var file = await bot.GetFileAsync(args.Message.Document.FileId);
            var fs = new FileStream($"{dirName}doc_{args.Message.Document.FileName}", FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            await fs.DisposeAsync();
            int countFiles = Directory.GetFiles(dirName).Length;
            bot.SendTextMessageAsync(args.Message.Chat.Id, $"Количество документов в хранилище: {countFiles}.");
        }

        #endregion

        /// <summary>
        /// Просмотр списка файлов.
        /// </summary>
        /// <param name="client">Бот.</param>
        /// <param name="args">Аргументы сообщения.</param>
        public static void PrintAllFiles(TelegramBotClient client, MessageEventArgs args)
        {
            bot = client;
            dirName = @"files\";

            FillDirsAndFiles(dirName);
            dirsAndFiles.Append($"\nВсего количество файлов в хранилище: {countFiles}.\n");
            dirsAndFiles.Append("\nДля скачивания введите команду /download \"filename\".");
            bot.SendTextMessageAsync(args.Message.Chat.Id, dirsAndFiles.ToString());
        }

        /// <summary>
        /// Заполнение данных о папках и файлах.
        /// </summary>
        /// <param name="path">Путь к папке с файлами.</param>
        /// <param name="trim">Пробелы.</param>
        private static void FillDirsAndFiles(string path, string trim = "")
        {
            var info = new DirectoryInfo(path);
            var dirs = info.GetDirectories();
            foreach (var dir in dirs)
            {
                dirsAndFiles.Append(trim + dir.Name + '\n');
                FillDirsAndFiles(Path.Combine(path, dir.Name), trim + "    ");
            }

            var files = info.GetFiles();
            foreach (var file in files)
            {
                dirsAndFiles.Append(trim + file.Name + '\n');
                countFiles++;
            }
        }

        /// <summary>
        /// Отправить файл пользователю.
        /// </summary>
        /// <param name="client">Бот.</param>
        /// <param name="e">Аргументы сообщения.</param>
        public static async Task UploadFile(TelegramBotClient client, MessageEventArgs e)
        {
            bot = client;
            dirName = @"files\";
            FillDirsAndFiles(dirName);
            var tmp = e.Message.Text;
            var fileName = tmp.Substring(10);
            var fullPath = GetFullPathFile(fileName);
            if (fullPath == null)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Файл не найден.");
                return;
            }

            await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Файл найден.");

            var typeFile = fullPath.Split('\\')[1];
            var fs = new FileStream(fullPath, FileMode.Open);
            InputOnlineFile file = new InputMedia(fs, fullPath);
            Console.WriteLine($"Отправляется файл: {fileName}");

            switch (typeFile)
            {
                case "Documents":
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Это документ.");
                    await bot.SendDocumentAsync(e.Message.Chat.Id, file);
                    break;
                case "Video":
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Это видео.");
                    await bot.SendVideoAsync(e.Message.Chat.Id, file);
                    break;
                case "Audio":
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Это аудио.");
                    await bot.SendAudioAsync(e.Message.Chat.Id, file);
                    break;
                case "Voice":
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Это голосовое сообщение.");
                    await bot.SendVoiceAsync(e.Message.Chat.Id, file);
                    break;
                case "Photo":
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Это фото.");
                    await bot.SendPhotoAsync(e.Message.Chat.Id, file);
                    break;
            }

            fs.Close();
            await fs.DisposeAsync();
        }

        /// <summary>
        /// Получить полный путь к файлу из хранилища.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Полный путь к файлу.</returns>
        private static string GetFullPathFile(string fileName)
        {
            if (fileName.IndexOf('_') == -1)
                return null;

            var typeFile = fileName.Substring(0, fileName.IndexOf('_'));
            switch (typeFile)
            {
                case "video":
                    return $@"files\Video\{fileName}";
                case "audio":
                    return $@"files\Audio\{fileName}";
                case "voice":
                    return $@"files\Voice\{fileName}";
                case "doc":
                    return $@"files\Documents\{fileName}";
                case "photo":
                    return $@"files\Photo\{fileName}";
                default:
                    return null;
            }
        }
    }
}
