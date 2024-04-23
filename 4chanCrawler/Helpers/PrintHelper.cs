using _4chanCrawler.Models;

namespace _4chanCrawler.Helpers;

public class PrintHelper
{
	public void PrintCompleteResults(List<Result> results, DateTime roundtripStart)
	{
		var dateAndTime = $"({DateTime.Now.ToString(Constants.DateTimeFormat)} | HH:MM:SS {DateTime.UtcNow - roundtripStart})";

		PrintResults(results, dateAndTime, true);
	}
	
	public void PrintResults(List<Result> results, string dateAndTime, bool isNew)
	{
		if (results is null || !results.Any())
		{
			if (isNew)
			{
				Console.WriteLine($"Nothing (new) found {dateAndTime}");
			}
			return;
		}

		if (isNew)
		{
			Console.WriteLine($"NEW RESULTS {dateAndTime}");
		}

		foreach (var result in results)
		{
			Console.WriteLine(result);
		}
	}
}