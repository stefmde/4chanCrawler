using _4chanCrawler.Helpers;
using _4chanCrawler.Models;

var configuration = ConfigurationHelper.Get();

var globalResults = new List<Result>();

Console.WriteLine(" 4Chan Crawler");
Console.WriteLine("###############");
Console.WriteLine();

await ResultsHelper.ReadFromFile();
if (ResultsHelper.Results.Any())
{
	Console.WriteLine("Previous Results:");
	foreach (var result in ResultsHelper.Results)
	{
		if (!globalResults.Any(x => x.BoardKey == result.BoardKey && x.ThreadId == result.ThreadId && x.ReplyId == result.ReplyId))
		{
			globalResults.Add(result);
			PrintHelper.PrintResults(ResultsHelper.Results, DateTime.Now.ToString(Constants.DateTimeFormat), false);
		}
	}
	Console.WriteLine();
}

while (true)
{
	var roundtripStart = DateTime.UtcNow;
	TimeHelper.CalculateTime(configuration);

	try
	{
		await CheckHelper.CheckBoards(configuration);
	}
	catch (Exception e)
	{
		Console.WriteLine($"Connection Error ({DateTime.Now.ToString(Constants.DateTimeFormat)}): {e}");
		Thread.Sleep( configuration.TimeoutBetweenLoopsMinutes * 1000 * 60);
		continue;
	}

	var uniqueResults = new List<Result>();
	foreach (var result in ResultsHelper.Results)
	{
		if (!globalResults.Any(x => x.BoardKey == result.BoardKey && x.ThreadId == result.ThreadId && x.ReplyId == result.ReplyId))
		{
			uniqueResults.Add(result);
			globalResults.Add(result);
		}
	}
	PrintHelper.PrintCompleteResults(uniqueResults, roundtripStart);
	Console.WriteLine("Waiting for next loop...");
	Console.WriteLine();
	Console.WriteLine();
	Thread.Sleep(configuration.TimeoutBetweenLoopsMinutes * 1000 * 60);
}
