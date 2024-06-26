using _4chanCrawler.Helpers;
using _4chanCrawler.Models;
using _4chanCrawler.Models.Configuration;

namespace _4chanCrawler;

public class App
{
	private readonly CrawlerConfiguration _crawlerConfiguration;
	private readonly CheckHelper _checkHelper;
	private readonly PrintHelper _printHelper;
	private readonly TimeHelper _timeHelper;
	private readonly HttpClientHelper _httpClientHelper;

	public App(CrawlerConfiguration crawlerConfiguration, CheckHelper checkHelper, PrintHelper printHelper, TimeHelper timeHelper, HttpClientHelper httpClientHelper)
	{
		_crawlerConfiguration = crawlerConfiguration ?? throw new ArgumentNullException(nameof(crawlerConfiguration));
		_checkHelper = checkHelper ?? throw new ArgumentNullException(nameof(checkHelper));
		_printHelper = printHelper ?? throw new ArgumentNullException(nameof(printHelper));
		_timeHelper = timeHelper ?? throw new ArgumentNullException(nameof(timeHelper));
		_httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(httpClientHelper));
	}

	public async Task Run()
	{
		Console.WriteLine(" 4Chan Crawler");
		Console.WriteLine("###############");
		Console.WriteLine();

		Console.WriteLine("Checking for previous results...");
		await ResultsHelper.ReadFromFile(_crawlerConfiguration.RemoveUnavailableResults);
		if (ResultsHelper.Results.Any())
		{
			Console.WriteLine();
			_printHelper.PrintResults(ResultsHelper.Results, $"Previous Results ({ResultsHelper.Results.Count}):", DateTime.Now.ToString(Constants.DateTimeFormat));
		}
		Console.WriteLine();

		while (true)
		{
			var roundtripStart = DateTime.UtcNow;
			_timeHelper.CalculateTime(_crawlerConfiguration);

			try
			{
				await _checkHelper.CheckBoards();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Connection Error ({DateTime.Now.ToString(Constants.DateTimeFormat)}): {e}");
				Thread.Sleep( _crawlerConfiguration.TimeoutBetweenLoopsMinutes * 1000 * 60);
				continue;
			}

			var uniqueResults = new List<Result>();
			foreach (var result in ResultsHelper.Results)
			{
				if (!ResultsHelper.Results.Any(x => x.BoardKey == result.BoardKey && x.ThreadId == result.ThreadId && x.ReplyId == result.ReplyId))
				{
					uniqueResults.Add(result);
				}
			}
			_printHelper.PrintCompleteResults(uniqueResults, roundtripStart);
			Console.WriteLine($"Done {_httpClientHelper.WebRequests} WebRequests.");
			Console.WriteLine($"Waiting {_crawlerConfiguration.TimeoutBetweenLoopsMinutes} min. for next loop...");
			Console.WriteLine();
			Console.WriteLine();
			Thread.Sleep(_crawlerConfiguration.TimeoutBetweenLoopsMinutes * 1000 * 60);
		}
	}
}