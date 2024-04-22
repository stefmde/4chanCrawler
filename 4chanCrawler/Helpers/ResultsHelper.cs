using _4chanCrawler.Models;

namespace _4chanCrawler.Helpers;

public class ResultsHelper
{
	public List<Result> Results { get; } = new List<Result>();

	public void AddResult(Result result)
	{
		Results.Add(result);
		WriteToFile();
	}

	private void WriteToFile()
	{
		
	}

	public async Task Check()
	{
		
	}
}