using _4chanCrawler.Models;

namespace _4chanCrawler.Helpers;

public static class PrintHelper
{
	public static void PrintResults(List<Result> results, DateTime roundtripStart)
	{
		Console.WriteLine();
		var dateAndTime = $"({DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} | HH:MM:SS {DateTime.UtcNow - roundtripStart})";
		
		if (results is null || !results.Any())
		{
			Console.WriteLine($"Nothing (new) found {dateAndTime}");
			return;
		}
		
		Console.WriteLine($"NEW RESULTS {dateAndTime}");
		foreach (var result in results)
		{
			Console.WriteLine(result);
		}
	}
}