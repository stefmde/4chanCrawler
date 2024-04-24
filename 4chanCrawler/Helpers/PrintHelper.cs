using _4chanCrawler.Models;

namespace _4chanCrawler.Helpers;

public class PrintHelper
{
	public void PrintCompleteResults(List<Result> newResults, DateTime roundtripStart)
	{
		var dateAndTime = $"({DateTime.Now.ToString(Constants.DateTimeFormat)} | HH:MM:SS {DateTime.UtcNow - roundtripStart})";

		PrintResults(newResults, $"NEW RESULTS", dateAndTime);
		PrintResults(ResultsHelper.Results, $"COMPLETE RESULTS", dateAndTime);
	}
	
	public void PrintResults(List<Result> results, string label, string dateAndTime)
	{
		if (results is null || !results.Any())
		{
			return;
		}
		
		Console.WriteLine($"{label} {dateAndTime}");

		foreach (var result in results)
		{
			Console.WriteLine(result);
		}
	}
}