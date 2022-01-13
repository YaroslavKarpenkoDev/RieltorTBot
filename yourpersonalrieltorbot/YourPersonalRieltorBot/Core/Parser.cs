using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YourPersonalRieltorBot.Core
{
	public class Parser : TelegramBot
	{
		public Parser()
		{
			StartParsing();
		}

		public List<string> result = new List<string>();

		public async void StartParsing()
		{
			List<string> result = await ParsingAsync(AppSettings.url);
		}

		#region Methods
		public async Task<List<string>> ParsingAsync(string url)
		{
			try
			{
				if (IsWorking)
				{
					if (result == null)
					{
						result = new();
					}

					using (HttpClientHandler handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None })
					{
						using (var client = new HttpClient(handler))
						{
							using (var response = await client.GetAsync(url))
							{
								if (response.IsSuccessStatusCode)
								{
									var html = await response.Content.ReadAsStringAsync();
									if (!string.IsNullOrEmpty(html))
									{
										HtmlAgilityPack.HtmlDocument htmlDocument = new();
										htmlDocument.LoadHtml(html);

										var ads = htmlDocument.DocumentNode.SelectNodes("//a[@class='css-rc5s2u']");
										if (ads != null)
										{
											foreach (var item in ads)
											{
												var ad = item.OuterHtml.Split('"');
												if (ad != null)
												{
													if (!result.Contains($"https://www.olx.ua{ad[3]}"))
													{
														result.Add($"https://www.olx.ua{ad[3]}");
														await telegramBot.clientTelegram.SendTextMessageAsync(GettedMessage.Chat.Id, $"https://www.olx.ua{ad[3]}");
													}
												}
											}
											await Task.Delay(TimeSpan.FromSeconds(1));
											Console.WriteLine("Iteration:" + DateTime.Now.ToString("HH:mm:ss"));
											await ParsingAsync(url);
										}
										else
										{
											Console.WriteLine("ads == null");
											await ParsingAsync(url);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				await ParsingAsync(url);
			}

			return null;
		}
		#endregion
	}
}
