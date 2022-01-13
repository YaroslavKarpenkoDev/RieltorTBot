using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace YourPersonalRieltorBot.Core
{
	public class TelegramBot : Program
	{
		public TelegramBot()
		{
			usersThreads = new();
		}

		#region Propertys&Fields

		

		public TelegramBotClient clientTelegram;
		public static bool IsWorking { get; set; }
		public string _responce;
		public static List<string> usersThreads { get; set; }

		private static int _threadNumber = 0;
		public static Message GettedMessage { get; set; }

		public Thread[] threads = new Thread[999];
		public List<Thread> ThreadsList = new();
		#endregion
		#region Methods
		public async void OnMessageHandler(object sender, MessageEventArgs e)
		{
			try
			{
				var msg = e.Message;
				GettedMessage = msg;
				if (msg != null)
				{
					var user = msg.Chat.Id.ToString();
					int number = 0;
					if (!usersThreads.Contains(user))
					{
						usersThreads.Add(user);
					}

					foreach (var item in usersThreads)
					{
						if (item != user)
							_threadNumber++;
						else
							number = _threadNumber;
					}
					Console.WriteLine($"Пришло сообщение от {msg.Chat.Bio} с текстом: {msg.Text}");
					switch (msg.Text)
					{
						case "Search house":
							await clientTelegram.SendTextMessageAsync(msg.Chat.Id,
								"Searching started!");
							ThreadsList[number] = new Thread(ToStartParsing);
							ThreadsList[number].Start();
							break;

						case "Stop Search":
							ThreadsList[number].ExecutionContext.Dispose();
							ThreadsList[number].Interrupt();
							await clientTelegram.SendTextMessageAsync(msg.Chat.Id,
								"Searching stoped!");
							break;

						case "Cheer up me!":
							await clientTelegram.SendStickerAsync
								(chatId: msg.Chat.Id,
								sticker: "CAACAgIAAxkBAAECyz5hI5NCgAkR2rBFdKP5UnxKDbLTUgACAQ0AAibiiEjzuDNoX0LCWyAE");
							await clientTelegram.SendTextMessageAsync(msg.Chat.Id,
								"Look what a sweet boy! Do not be upset!");
							break;

						case "Share wisdom with me!":
							await clientTelegram.SendStickerAsync
								(chatId: msg.Chat.Id,
								sticker: "CAACAgIAAxkBAAECy0FhI5TYCZp8L2lxWyUoiYkb_Zu1FgACtwADZj9IJkGMuRWO5ygvIAQ");
							await clientTelegram.SendTextMessageAsync(msg.Chat.Id, "Not every slave with a deep ass.");
							break;

						default:
							await clientTelegram.SendStickerAsync(chatId: msg.Chat.Id, sticker: "https://tlgrm.ru/_/stickers/06d/991/06d991f7-564f-47cd-8180-585cd0056a42/5.webp", replyMarkup: GetButtons());
							await clientTelegram.SendTextMessageAsync(msg.Chat.Id, "Are you want me?");
							break;
					}

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return;
			}
		}

		public IReplyMarkup GetButtons()
		{
			return new ReplyKeyboardMarkup
			{
				Keyboard = new List<List<KeyboardButton>>
				{
					new List<KeyboardButton>
					{
						new KeyboardButton { Text = "Search house" },
						new KeyboardButton { Text = "Cheer up me!"},
						new KeyboardButton { Text = "Share wisdom with me!" } },

					new List<KeyboardButton>{ new KeyboardButton { Text = "Stop Search" } }
				}
			};
		}
		public async void ToStartParsing()
		{
			IsWorking = true;
			new Parser();
		}

		public async void ToStopParsing()
		{
			IsWorking = false;
		}
		#endregion
	}
}
