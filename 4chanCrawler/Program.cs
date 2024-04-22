using _4chanCrawler.Helpers;
using _4chanCrawler.Models;

var configuration = ConfigurationHelper.Get();

var globalResults = new List<Result>();

Console.WriteLine(" 4Chan Crawler");
Console.WriteLine("###############");
Console.WriteLine();

while (true)
{
	var roundtripStart = DateTime.UtcNow;
	List<Result> results;
	TimeHelper.CalculateTime(configuration);

	try
	{
		results = await CheckHelper.CheckBoards(configuration);
	}
	catch (Exception e)
	{
		Console.WriteLine($"Connection Error ({DateTime.Now.ToString(Constants.DateTimeFormat)}): {e}");
		Thread.Sleep( configuration.TimeoutBetweenLoopsMinutes * 1000 * 60);
		continue;
	}

	var uniqueResults = new List<Result>();
	foreach (var result in results)
	{
		if (!globalResults.Any(x => x.BoardKey == result.BoardKey && x.ThreadId == result.ThreadId && x.ReplyId == result.ReplyId))
		{
			uniqueResults.Add(result);
			globalResults.Add(result);
		}
	}
	PrintHelper.PrintResults(uniqueResults, roundtripStart);
	Thread.Sleep(configuration.TimeoutBetweenLoopsMinutes * 1000 * 60);
}
